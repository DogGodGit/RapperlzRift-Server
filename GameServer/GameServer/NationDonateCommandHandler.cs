using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationDonateCommandHandler : InGameCommandHandler<NationDonateCommandBody, NationDonateResponseBody>
{
	public const short kResult_DailyDonationCountOverflowed = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kResult_NotEnoughDia = 103;

	public const short kResult_NotCompletedRequiredMainQuest = 104;

	private NationInstance m_myNationInst;

	private long m_lnUsedGold;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private int m_nRewardExploitPoint;

	private int m_nAcquiredExploitPoint;

	private long m_lnRewardNationFund;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nEntryId = m_body.entryId;
		if (nEntryId < 0)
		{
			throw new CommandHandleException(1, "유효하지 않은 항목ID 입니다.");
		}
		NationDonationEntry entry = Resource.instance.GetNationDonationEntry(nEntryId);
		if (entry == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 국가기부항목입니다. nEntryId = " + nEntryId);
		}
		if (!m_myHero.IsMainQuestCompleted(Resource.instance.rankOpenRequiredMainQuestNo))
		{
			throw new CommandHandleException(104, "필요한 메인퀘스트를 완료하지 않았습니다.");
		}
		m_myHero.RefreshDailyNationDonationCount(m_currentTime.Date);
		DateValuePair<int> dailyNationDonationCount = m_myHero.dailyNationDonationCount;
		if (dailyNationDonationCount.value >= m_myHero.vipLevel.nationDonationMaxCount)
		{
			throw new CommandHandleException(101, "일일횟수가 최대횟수를 넘어갑니다.");
		}
		switch (entry.moneyType)
		{
		case NationDonationEntryMoneyType.Gold:
		{
			long lnPrice = entry.moneyAmount;
			if (m_myHero.gold < lnPrice)
			{
				throw new CommandHandleException(102, "골드가 부족합니다.");
			}
			m_myHero.UseGold(lnPrice);
			m_lnUsedGold = lnPrice;
			break;
		}
		case NationDonationEntryMoneyType.Dia:
		{
			int nPrice = (int)entry.moneyAmount;
			if (m_myHero.dia < nPrice)
			{
				throw new CommandHandleException(103, "다이아가 부족합니다.");
			}
			m_myHero.UseDia(nPrice, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
			break;
		}
		}
		m_nRewardExploitPoint = entry.exploitPointRewardValue;
		m_nAcquiredExploitPoint = m_myHero.AddExploitPoint(m_nRewardExploitPoint, m_currentTime, bSaveToDB: false);
		m_myNationInst = m_myHero.nationInst;
		m_lnRewardNationFund = entry.nationFundRewardValue;
		m_myNationInst.AddFund(m_lnRewardNationFund, m_myHero.id);
		dailyNationDonationCount.value++;
		SaveToDB();
		SaveToGameLogDB(nEntryId);
		NationDonateResponseBody resBody = new NationDonateResponseBody();
		resBody.date = (DateTime)dailyNationDonationCount.date;
		resBody.dailyNationDonationCount = dailyNationDonationCount.value;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.gold = m_myHero.gold;
		resBody.acquiredExploitPoint = m_nAcquiredExploitPoint;
		resBody.exploitPoint = m_myHero.exploitPoint;
		resBody.dailyExploitPoint = m_myHero.dailyExploitPoint.value;
		resBody.nationFund = m_myNationInst.fund;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(m_myNationInst.nationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationDonateDateCount(m_myHero.id, m_myHero.dailyNationDonationCount.date, m_myHero.dailyNationDonationCount.value));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(m_myNationInst.nationId, m_myNationInst.fund));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nEntryId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationDonationLog(Guid.NewGuid(), m_myHero.id, nEntryId, m_lnUsedGold, m_nUsedOwnDia, m_nUsedUnOwnDia, m_nRewardExploitPoint, m_nAcquiredExploitPoint, m_nRewardExploitPoint));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
