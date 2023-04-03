using System;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class NationGuildRankingManager
{
	private int m_nRankingNo;

	private SFRunnableStandaloneWork m_updateWork;

	private static NationGuildRankingManager s_instance = new NationGuildRankingManager();

	public static NationGuildRankingManager instance => s_instance;

	private NationGuildRankingManager()
	{
	}

	public void Init()
	{
		Task(bInit: true);
	}

	public void OnUpdate()
	{
		try
		{
			if (m_updateWork == null && Cache.instance.prevUpdateTime.Second != Cache.instance.currentUpdateTime.Second)
			{
				SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
				work.runnable = new SFAction<bool>(Task, arg: false);
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

	private void Task(bool bInit)
	{
		SqlConnection conn = null;
		int nLastRankingNo = 0;
		DataRowCollection drcRankings = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			nLastRankingNo = GameDac.LastNationGuildRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.NationGuildRankings(conn, null, nLastRankingNo);
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
				nationInst2.ClearGuildRankings();
			}
			foreach (DataRow dr in drcRankings)
			{
				GuildRanking ranking = new GuildRanking();
				ranking.Init(dr);
				NationInstance nation = cache.GetNationInstance(ranking.nationId);
				nation.AddGuildRanking(ranking);
			}
			m_nRankingNo = nLastRankingNo;
			if (bInit)
			{
				return;
			}
			foreach (NationInstance nationInst in cache.nationInsts.Values)
			{
				nationInst.UpdateKing();
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
