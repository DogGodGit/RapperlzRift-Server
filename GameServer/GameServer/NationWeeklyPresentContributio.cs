using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationWeeklyPresentContributionPointRankingRewardReceiveCommandHandler : InGameCommandHandler<NationWeeklyPresentContributionPointRankingRewardReceiveCommandBody, NationWeeklyPresentContributionPointRankingRewardReceiveResponseBody>
{
	public const short kResult_AlreadyReceived = 101;

	public const short kResult_NotRanker = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nRankingNo;

	private int m_nRanking;

	private WeeklyPresentContributionPointRankingRewardGroup m_rewardGroup;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		NationWeeklyPresentContributionPointRankingManager manager = NationWeeklyPresentContributionPointRankingManager.instance;
		m_nRankingNo = manager.rankingNo;
		if (m_myHero.rewardedNationWeeklyPresentContributionPointRankingNo >= m_nRankingNo)
		{
			throw new CommandHandleException(101, "이미 보상을 받았습니다.");
		}
		m_nRanking = m_myHero.nationInst.GetWeeklyPresentContributionPointRankingValueOfHero(m_myHero.id);
		if (m_nRanking <= 0)
		{
			throw new CommandHandleException(102, "랭킹에 진입하지 못했습니다.");
		}
		m_rewardGroup = Resource.instance.GetWeeklyPresentContributionPointRankingRewardGroupByRanking(m_nRanking);
		if (m_rewardGroup != null)
		{
			foreach (WeeklyPresentContributionPointRankingReward reward in m_rewardGroup.rewards.Values)
			{
				ItemReward itemReward = reward.itemReward;
				int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
				if (nRemainingCount > 0)
				{
					if (m_mail == null)
					{
						m_mail = Mail.Create("MAIL_REWARD_N_31", "MAIL_REWARD_D_31", m_currentTime);
					}
					m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
				}
			}
		}
		m_myHero.rewardedNationWeeklyPresentContributionPointRankingNo = m_nRankingNo;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToGameLogDB();
		NationWeeklyPresentContributionPointRankingRewardReceiveResponseBody resBody = new NationWeeklyPresentContributionPointRankingRewardReceiveResponseBody();
		resBody.rewardedRankingNo = m_nRankingNo;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWeeklyPresentContributionPointRankingReward(m_myHero.id, m_myHero.rewardedNationWeeklyPresentContributionPointRankingNo));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyPresentContributionPointRankingRewardLog(logId, m_myHero.id, m_nRankingNo, m_nRanking, m_currentTime));
			if (m_rewardGroup != null)
			{
				foreach (WeeklyPresentContributionPointRankingReward reward in m_rewardGroup.rewards.Values)
				{
					ItemReward itemReward = reward.itemReward;
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyPresentContributionPointRankingRewardDetailLog(Guid.NewGuid(), logId, itemReward.item.id, itemReward.owned, itemReward.count));
				}
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
public class NationWeeklyPresentContributionPointRankingCommandHandler : InGameCommandHandler<NationWeeklyPresentContributionPointRankingCommandBody, NationWeeklyPresentContributionPointRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		NationInstance nationInst = m_myHero.nationInst;
		PresentContributionPointRanking myRanking = nationInst.GetWeeklyPresentContributionPointRankingOfHero(m_myHero.id);
		NationWeeklyPresentContributionPointRankingResponseBody resBody = new NationWeeklyPresentContributionPointRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = nationInst.GetWeeklyPDPresentContributionPointRankings(Resource.instance.presentContributionPointRankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
public class NationWeeklyPresentContributionPointRankingManager
{
	private int m_nRankingNo;

	private SFRunnableStandaloneWork m_updateWork;

	private static NationWeeklyPresentContributionPointRankingManager s_instance = new NationWeeklyPresentContributionPointRankingManager();

	public int rankingNo => m_nRankingNo;

	public static NationWeeklyPresentContributionPointRankingManager instance => s_instance;

	private NationWeeklyPresentContributionPointRankingManager()
	{
	}

	public void Init()
	{
		Task(bSendEvent: false);
	}

	public void OnUpdate()
	{
		try
		{
			if (m_updateWork == null && Cache.instance.prevUpdateTime.Second != Cache.instance.currentUpdateTime.Second)
			{
				SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
				work.runnable = new SFAction<bool>(Task, arg: true);
				work.finishHandler = TaskFinished;
				work.Schedule();
				m_updateWork = work;
			}
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void Task(bool bSendEvent)
	{
		SqlConnection conn = null;
		int nLastRankingNo = 0;
		DataRowCollection drcRankings = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			nLastRankingNo = GameDac.LastNationWeeklyPresentContributionPointRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.NationWeeklyPresentContributionPointRankings(conn, null, nLastRankingNo);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
		Cache cache = Cache.instance;
		lock (Global.syncObject)
		{
			foreach (NationInstance nationInst2 in cache.nationInsts.Values)
			{
				nationInst2.ClearWeeklyPresentContributionPointRankings();
			}
			foreach (DataRow dr in drcRankings)
			{
				PresentContributionPointRanking ranking = new PresentContributionPointRanking();
				ranking.Init(dr);
				NationInstance nationInst = cache.GetNationInstance(ranking.nationId);
				nationInst.AddWeeklyPresentContributionPointRanking(ranking);
			}
			m_nRankingNo = nLastRankingNo;
			OnRankingUpdated(bSendEvent);
		}
	}

	private void OnRankingUpdated(bool bSendEvent)
	{
		if (!bSendEvent)
		{
			return;
		}
		foreach (NationInstance nationInst in Cache.instance.nationInsts.Values)
		{
			foreach (Hero hero in nationInst.heroes.Values)
			{
				int nRanking = nationInst.GetWeeklyPresentContributionPointRankingValueOfHero(hero.id);
				ServerEvent.SendNationWeeklyPresentContributionPointRankingUpdated(hero.account.peer, m_nRankingNo, nRanking);
			}
		}
	}

	private void TaskFinished(SFWork work, Exception error)
	{
		lock (Global.syncObject)
		{
			m_updateWork = null;
		}
		if (error != null)
		{
			SFLogUtil.Error(GetType(), null, error);
		}
	}
}
