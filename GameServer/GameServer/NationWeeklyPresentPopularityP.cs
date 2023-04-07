using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class NationWeeklyPresentPopularityPointRankingCommandHandler : InGameCommandHandler<NationWeeklyPresentPopularityPointRankingCommandBody, NationWeeklyPresentPopularityPointRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		NationInstance nationInst = m_myHero.nationInst;
		PresentPopularityPointRanking myRanking = nationInst.GetWeeklyPresentPopularityPointRankingOfHero(m_myHero.id);
		NationWeeklyPresentPopularityPointRankingResponseBody resBody = new NationWeeklyPresentPopularityPointRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = nationInst.GetWeeklyPDPresentPopularityPointRankings(Resource.instance.presentPopularityPointRankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
public class NationWeeklyPresentPopularityPointRankingRewardReceiveCommandHandler : InGameCommandHandler<NationWeeklyPresentPopularityPointRankingRewardReceiveCommandBody, NationWeeklyPresentPopularityPointRankingRewardReceiveResponseBody>
{
	public const short kResult_AlreadyReceived = 101;

	public const short kResult_NotRanker = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nRankingNo;

	private int m_nRanking;

	private WeeklyPresentPopularityPointRankingRewardGroup m_rewardGroup;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		NationWeeklyPresentPopularityPointRankingManager manager = NationWeeklyPresentPopularityPointRankingManager.instance;
		m_nRankingNo = manager.rankingNo;
		if (m_myHero.rewardedNationWeeklyPresentPopularityPointRankingNo >= m_nRankingNo)
		{
			throw new CommandHandleException(101, "이미 보상을 받았습니다.");
		}
		m_nRanking = m_myHero.nationInst.GetWeeklyPresentPopularityPointRankingValueOfHero(m_myHero.id);
		if (m_nRanking <= 0)
		{
			throw new CommandHandleException(102, "랭킹에 진입하지 못했습니다.");
		}
		m_rewardGroup = Resource.instance.GetWeeklyPresentPopularityPointRankingRewardGroupByRanking(m_nRanking);
		if (m_rewardGroup != null)
		{
			foreach (WeeklyPresentPopularityPointRankingReward reward in m_rewardGroup.rewards.Values)
			{
				ItemReward itemReward = reward.itemReward;
				int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
				if (nRemainingCount > 0)
				{
					if (m_mail == null)
					{
						m_mail = Mail.Create("MAIL_REWARD_N_30", "MAIL_REWARD_D_30", m_currentTime);
					}
					m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
				}
			}
		}
		m_myHero.rewardedNationWeeklyPresentPopularityPointRankingNo = m_nRankingNo;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToGameLogDB();
		NationWeeklyPresentPopularityPointRankingRewardReceiveResponseBody resBody = new NationWeeklyPresentPopularityPointRankingRewardReceiveResponseBody();
		resBody.rewardedRankingNo = m_nRankingNo;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWeeklyPresentPopularityPointRankingReward(m_myHero.id, m_myHero.rewardedNationWeeklyPresentPopularityPointRankingNo));
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyPresentPopularityPointRankingRewardLog(logId, m_myHero.id, m_nRankingNo, m_nRanking, m_currentTime));
			if (m_rewardGroup != null)
			{
				foreach (WeeklyPresentPopularityPointRankingReward reward in m_rewardGroup.rewards.Values)
				{
					ItemReward itemReward = reward.itemReward;
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyPresentPopularityPointRankingRewardDetailLog(Guid.NewGuid(), logId, itemReward.item.id, itemReward.owned, itemReward.count));
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
public class NationWeeklyPresentPopularityPointRankingManager
{
	private int m_nRankingNo;

	private SFRunnableStandaloneWork m_updateWork;

	private static NationWeeklyPresentPopularityPointRankingManager s_instance = new NationWeeklyPresentPopularityPointRankingManager();

	public int rankingNo => m_nRankingNo;

	public static NationWeeklyPresentPopularityPointRankingManager instance => s_instance;

	private NationWeeklyPresentPopularityPointRankingManager()
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
			nLastRankingNo = GameDac.LastNationWeeklyPresentPopularityPointRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.NationWeeklyPresentPopularityPointRankings(conn, null, nLastRankingNo);
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
				nationInst2.ClearWeeklyPresentPopularityPointRankings();
			}
			foreach (DataRow dr in drcRankings)
			{
				PresentPopularityPointRanking ranking = new PresentPopularityPointRanking();
				ranking.Init(dr);
				NationInstance nationInst = cache.GetNationInstance(ranking.nationId);
				nationInst.AddWeeklyPresentPopularityPointRanking(ranking);
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
				int nRanking = nationInst.GetWeeklyPresentPopularityPointRankingValueOfHero(hero.id);
				ServerEvent.SendNationWeeklyPresentPopularityPointRankingUpdated(hero.account.peer, m_nRankingNo, nRanking);
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
