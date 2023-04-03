using System;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class NationBattlePowerRankingManager
{
	private int m_nRankingNo;

	private SFRunnableStandaloneWork m_updateWork;

	private static NationBattlePowerRankingManager s_instance = new NationBattlePowerRankingManager();

	public static NationBattlePowerRankingManager instance => s_instance;

	private NationBattlePowerRankingManager()
	{
	}

	public void Init()
	{
		Task();
	}

	public void OnUpdate()
	{
		try
		{
			if (m_updateWork == null && Cache.instance.prevUpdateTime.Second != Cache.instance.currentUpdateTime.Second)
			{
				SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
				work.runnable = new SFAction(Task);
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

	private void Task()
	{
		SqlConnection conn = null;
		int nLastRankingNo = 0;
		DataRowCollection drcRankings = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			nLastRankingNo = GameDac.LastNationBattlePowerRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.NationBattlePowerRankings(conn, null, nLastRankingNo);
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
				nationInst2.ClearBattlePowerRankings();
			}
			foreach (DataRow dr in drcRankings)
			{
				Ranking ranking = new Ranking();
				ranking.Init(dr);
				NationInstance nationInst = cache.GetNationInstance(ranking.nationId);
				nationInst.AddBattlePowerRanking(ranking);
			}
			m_nRankingNo = nLastRankingNo;
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
