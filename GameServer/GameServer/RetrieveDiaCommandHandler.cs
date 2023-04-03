using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RetrieveDiaCommandHandler : InGameCommandHandler<RetrieveDiaCommandBody, RetrieveDiaResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	public const short kResult_NotAvailableRetrieval = 102;

	public const short kResult_NotEnoughRetrievalCount = 103;

	private HeroRetrieval m_heroRetrieval;

	private int m_nRequiredDia;

	private long m_lnExpReward;

	private ItemReward m_itemReward;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		DateTime yesterday = currentDate.AddDays(-1.0);
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nRetrievalId = m_body.retrievalId;
		if (nRetrievalId <= 0)
		{
			throw new CommandHandleException(1, "회수ID가 유효하지 않습니다. nRetrievalId = " + nRetrievalId);
		}
		Retrieval retrielval = Resource.instance.GetRetrieval(nRetrievalId);
		if (retrielval == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 회수입니다. nRetrievalId = " + nRetrievalId);
		}
		m_nRequiredDia = retrielval.diaRetrievalRequiredDia;
		if (m_myHero.dia < m_nRequiredDia)
		{
			throw new CommandHandleException(101, "다이아가 부족합니다.");
		}
		if (!m_myHero.GetAvailableRetrieval(nRetrievalId))
		{
			throw new CommandHandleException(102, "아직 개방되지않은 회수 목록입니다.");
		}
		m_myHero.RefreshRetrievals(currentDate);
		HeroRetrievalProgressCountCollection colleciton = m_myHero.GetOrCreateRetrivalProgressCountCollection(yesterday);
		HeroRetrievalProgressCount prevProgressCount = colleciton.GetProgressCount(nRetrievalId);
		m_heroRetrieval = m_myHero.GetRetrieval(nRetrievalId);
		int nTargetCount = m_myHero.GetRetrievalTargetCount(nRetrievalId);
		int nPrevProgressCount = prevProgressCount?.progressCount ?? 0;
		int nRetrievalCount = ((m_heroRetrieval != null) ? m_heroRetrieval.count : 0);
		if (nPrevProgressCount + nRetrievalCount >= nTargetCount)
		{
			throw new CommandHandleException(103, "회수 횟수가 부족합니다.");
		}
		if (m_heroRetrieval == null)
		{
			m_heroRetrieval = new HeroRetrieval(m_myHero, nRetrievalId);
			m_myHero.AddRetrieval(m_heroRetrieval);
		}
		RetrievalReward retrievalReward = retrielval.GetReward(m_myHero.level);
		int nOldLevel = m_myHero.level;
		if (retrievalReward != null)
		{
			m_lnExpReward = retrievalReward.diaExpRewardValue;
			m_itemReward = retrievalReward.diaItemReward;
		}
		m_myHero.UseDia(m_nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		if (m_lnExpReward > 0)
		{
			m_myHero.AddExp(m_lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		if (m_itemReward != null)
		{
			int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				m_mail = Mail.Create("MAIL_REWARD_N_22", "MAIL_REWARD_D_22", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
		m_heroRetrieval.count++;
		SaveToDB();
		SaveToLogDB(nOldLevel);
		RetrieveDiaResponseBody resBody = new RetrieveDiaResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.retrievalCount = m_heroRetrieval.count;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.acquiredExp = m_lnExpReward;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroRetrieval(m_heroRetrieval.hero.id, m_heroRetrieval.hero.retrievalDate, m_heroRetrieval.id, m_heroRetrieval.count));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldLevel)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRetrievalLogs(logId, m_myHero.id, m_heroRetrieval.id, 1, nOldLevel, m_myHero.vipLevel.level, 2, 0L, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			int nRewardItemId = ((m_itemReward != null) ? m_itemReward.item.id : 0);
			bool bRewardItemOwned = m_itemReward != null && m_itemReward.owned;
			int nRewardItemCount = ((m_itemReward != null) ? m_itemReward.count : 0);
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRetrievalDetailLog(Guid.NewGuid(), logId, m_lnExpReward, nRewardItemId, bRewardItemOwned, nRewardItemCount));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
