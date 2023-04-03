using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ServerNationPowerRankingManager
{
	private int m_nRankingNo;

	private SFRunnableStandaloneWork m_updateWork;

	private static ServerNationPowerRankingManager s_instance = new ServerNationPowerRankingManager();

	public static ServerNationPowerRankingManager instance => s_instance;

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
			nLastRankingNo = GameDac.LastServerNationPowerRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.ServerNationPowerRankings(conn, null, nLastRankingNo);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
		Cache cache = Cache.instance;
		lock (Global.syncObject)
		{
			foreach (DataRow dr in drcRankings)
			{
				NationPowerRanking ranking = new NationPowerRanking();
				ranking.Init(dr);
				NationInstance nationInst = cache.GetNationInstance(ranking.nationId);
				nationInst.SetNationPowerRanking(ranking);
			}
			m_nRankingNo = nLastRankingNo;
			OnRankingUpdate(bSendEvent);
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

	private void OnRankingUpdate(bool bSendEvent)
	{
		Cache chace = Cache.instance;
		if (!bSendEvent)
		{
			return;
		}
		List<PDNationPowerRanking> rankings = new List<PDNationPowerRanking>();
		foreach (NationInstance nationInst in chace.nationInsts.Values)
		{
			if (nationInst.nationPowerRanking != null)
			{
				rankings.Add(nationInst.nationPowerRanking.ToPDNationPowerRanking());
				if (nationInst.nationPowerRanking.ranking == 1)
				{
					Cache.instance.CancelNationAllianceApplicationOfNationInstance(nationInst, DateTimeUtil.currentTime, Guid.Empty);
				}
			}
		}
		ServerEvent.SendDailyServerNationPowerRankingUpdated(chace.GetClientPeers(Guid.Empty), rankings.ToArray());
	}
}
