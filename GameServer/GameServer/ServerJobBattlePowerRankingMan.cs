using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class ServerJobBattlePowerRankingManager
{
	private int m_nRankingNo;

	private Dictionary<int, ServerJobBattlePowerRankingCollection> m_rankingCollections = new Dictionary<int, ServerJobBattlePowerRankingCollection>();

	private Dictionary<Guid, Ranking> m_rankingsByHero = new Dictionary<Guid, Ranking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static ServerJobBattlePowerRankingManager s_instance = new ServerJobBattlePowerRankingManager();

	public static ServerJobBattlePowerRankingManager instance => s_instance;

	private ServerJobBattlePowerRankingManager()
	{
	}

	public void Init()
	{
		foreach (Job job in Resource.instance.jobs.Values)
		{
			AddRankingCollection(new ServerJobBattlePowerRankingCollection(job.id));
		}
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
			nLastRankingNo = GameDac.LastServerJobBattlePowerRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.ServerJobBattlePowerRankings(conn, null, nLastRankingNo);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
		lock (Global.syncObject)
		{
			ClearRankings();
			foreach (DataRow dr in drcRankings)
			{
				Convert.ToInt32(dr["jobId"]);
				Ranking ranking = new Ranking();
				ranking.Init(dr);
				AddRanking(ranking);
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

	private void ClearRankings()
	{
		foreach (ServerJobBattlePowerRankingCollection collection in m_rankingCollections.Values)
		{
			collection.ClearRankings();
		}
		m_rankingsByHero.Clear();
	}

	private void AddRankingCollection(ServerJobBattlePowerRankingCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_rankingCollections.Add(collection.jobId, collection);
	}

	public ServerJobBattlePowerRankingCollection GetRankingCollection(int nJobId)
	{
		if (!m_rankingCollections.TryGetValue(nJobId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddRanking(Ranking ranking)
	{
		ServerJobBattlePowerRankingCollection collection = GetRankingCollection(ranking.jobId);
		collection.AddRanking(ranking);
		m_rankingsByHero.Add(ranking.heroId, ranking);
	}

	public Ranking GetRankingOfHero(Guid heroId)
	{
		if (!m_rankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}
}
