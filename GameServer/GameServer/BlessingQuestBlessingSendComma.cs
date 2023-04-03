using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BlessingQuestBlessingSendCommandHandler : InGameCommandHandler<BlessingQuestBlessingSendCommandBody, BlessingQuestBlessingSendResponseBody>
{
	public const short kResult_QuestNotExist = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kResult_NotEnoughDia = 103;

	public const short kResult_TargetNotLoggedIn = 104;

	public const short kResult_TargetReceivedBlessingCountIsMax = 105;

	public const short kResult_MyOwnerProspectQuestCountIsMax = 106;

	public const short kResult_TargetHeroTargetProspectQuestCountIsMax = 107;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private long m_lnQuestId;

	private HeroBlessingQuest m_quest;

	private Blessing m_blessing;

	private Hero m_target;

	private long m_lnUsedGold;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private HeroBlessing m_heroBlessing;

	private HeroProspectQuest m_prospectQuest;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_lnQuestId = m_body.questId;
		if (m_lnQuestId <= 0)
		{
			throw new CommandHandleException(1, "퀘스트ID가 유효하지 않습니다. m_lnQuestId = " + m_lnQuestId);
		}
		int nBlessingId = m_body.blessingId;
		if (nBlessingId <= 0)
		{
			throw new CommandHandleException(1, "축복ID가 유효하지 않습니다. nBlessingId = " + nBlessingId);
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_quest = m_myHero.GetBlessingQuest(m_lnQuestId);
		if (m_quest == null)
		{
			throw new CommandHandleException(101, "퀘스트가 존재하지 않습니다. m_lnQuestId = " + m_lnQuestId);
		}
		m_blessing = Resource.instance.GetBlessing(nBlessingId);
		if (m_blessing == null)
		{
			throw new CommandHandleException(1, "축복이 존재하지 않습니다. nBlessingId = " + nBlessingId);
		}
		switch (m_blessing.moneyType)
		{
		case BlessingMoneyType.Gold:
			if (m_myHero.gold < m_blessing.moneyAmount)
			{
				throw new CommandHandleException(102, "골드가 부족합니다.");
			}
			break;
		case BlessingMoneyType.Dia:
			if (m_myHero.dia < m_blessing.moneyAmount)
			{
				throw new CommandHandleException(103, "다이아가 부족합니다.");
			}
			break;
		default:
			throw new CommandHandleException(1, "재화타입이 유효하지 않습니다. m_blessing.id = " + m_blessing.id + ", m_blessing.moneyType = " + m_blessing.moneyType);
		}
		if (m_blessing.isProspect && m_myHero.isOwnerProspectQuestListFull)
		{
			throw new CommandHandleException(106, "나의 소유유망자퀘스트수가 최대입니다.");
		}
		m_target = Cache.instance.GetLoggedInHero(m_quest.targetHeroId);
		if (m_target == null)
		{
			throw new CommandHandleException(104, "대상영웅이 로그인중이 아닙니다.");
		}
		lock (m_target.syncObject)
		{
			Process();
		}
	}

	private void Process()
	{
		if (m_target.isReceivedBlessingListFull)
		{
			throw new CommandHandleException(105, "대상영웅의 받은 축복수가 최대입니다.");
		}
		if (m_blessing.isProspect && m_target.isTargetProspectQuestListFull)
		{
			throw new CommandHandleException(107, "대상영웅의 대상유망자퀘스트수가 최대입니다.");
		}
		switch (m_blessing.moneyType)
		{
		case BlessingMoneyType.Gold:
			m_myHero.UseGold(m_blessing.moneyAmount);
			m_lnUsedGold = m_blessing.moneyAmount;
			break;
		case BlessingMoneyType.Dia:
			m_myHero.UseDia(m_blessing.moneyAmount, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
			break;
		}
		m_heroBlessing = m_target.ReceiveBlessing(m_blessing, m_quest.targetLevel, m_myHero.id, m_myHero.name);
		if (m_blessing.isProspect)
		{
			HeroProspectQuest prospectQuest = new HeroProspectQuest();
			prospectQuest.Init(m_myHero, m_target, m_quest.targetLevel, m_currentTime);
			if (!prospectQuest.isFailed)
			{
				m_myHero.StartOwnerProspectQuest(prospectQuest);
				m_target.OnTargetProspectQuestStarted(prospectQuest, m_currentTime);
				Cache.instance.AddProspectQuest(prospectQuest);
				m_prospectQuest = prospectQuest;
			}
		}
		m_itemReward = m_blessing.senderItemReward;
		if (m_itemReward != null)
		{
			int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				m_mail = Mail.Create("MAIL_REWARD_N_27", "MAIL_REWARD_D_27", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
		m_myHero.RemoveBlessingQuest(m_lnQuestId);
		SaveToDB();
		SaveToGameLogDB();
		BlessingQuestBlessingSendResponseBody resBody = new BlessingQuestBlessingSendResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		if (m_prospectQuest != null)
		{
			resBody.newProspectQuest = m_prospectQuest.ToPDHeroProspectQuest(m_currentTime);
		}
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		if (m_prospectQuest != null)
		{
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateContentWork(QueuingWorkContentId.GameDB_ProspectQuest));
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_prospectQuest.targetId));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		if (m_prospectQuest != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroProspectQuest(m_prospectQuest.instanceId, m_prospectQuest.ownerId, m_prospectQuest.targetId, m_prospectQuest.blessingTargetLevel.id, (int)m_prospectQuest.status, m_prospectQuest.regTime, m_prospectQuest.statusUpdateTime));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroBlessingSendingLog(m_heroBlessing.sendingLogId, m_myHero.id, m_quest.targetHeroId, m_quest.targetLevel.id, m_blessing.id, m_lnUsedGold, m_nUsedOwnDia, m_nUsedUnOwnDia, (m_itemReward != null) ? m_itemReward.item.id : 0, m_itemReward != null && m_itemReward.owned, (m_itemReward != null) ? m_itemReward.count : 0, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
