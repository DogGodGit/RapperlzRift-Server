using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class DailyFieldOfHonorRankingManager
{
	private int m_nRankingNo;

	private List<DailyFieldOfHonorRanking> m_rankings = new List<DailyFieldOfHonorRanking>();

	private Dictionary<Guid, DailyFieldOfHonorRanking> m_rankingsByHero = new Dictionary<Guid, DailyFieldOfHonorRanking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static DailyFieldOfHonorRankingManager s_instance = new DailyFieldOfHonorRankingManager();

	public int rankingNo => m_nRankingNo;

	public static DailyFieldOfHonorRankingManager instance => s_instance;

	private DailyFieldOfHonorRankingManager()
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
			nLastRankingNo = GameDac.LastDailyFieldOfHonorRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.DailyFieldOfHonorRankings(conn, null, nLastRankingNo);
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
				DailyFieldOfHonorRanking ranking = new DailyFieldOfHonorRanking();
				ranking.Init(dr);
				AddRanking(ranking);
			}
			m_nRankingNo = nLastRankingNo;
			OnRankingUpdated(bSendEvent);
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

	private void OnRankingUpdated(bool bSendEvent)
	{
		if (!bSendEvent)
		{
			return;
		}
		foreach (Hero hero in Cache.instance.heroes.Values)
		{
			if (hero.isLoggedIn)
			{
				DailyFieldOfHonorRanking ranking = GetRankingOfHero(hero.id);
				ServerEvent.SendFieldOfHonorDailyRankingUpdated(hero.account.peer, m_nRankingNo, ranking?.ranking ?? 0);
			}
		}
	}

	private void ClearRankings()
	{
		m_rankings.Clear();
		m_rankingsByHero.Clear();
	}

	private void AddRanking(DailyFieldOfHonorRanking ranking)
	{
		m_rankings.Add(ranking);
		m_rankingsByHero.Add(ranking.heroId, ranking);
	}

	public DailyFieldOfHonorRanking GetRankingOfHero(Guid heroId)
	{
		if (!m_rankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}
}
