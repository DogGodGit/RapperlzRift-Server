using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class DailyServerLevelRankingManager
{
	private int m_nRankingNo;

	private Dictionary<Guid, Ranking> m_rankingsByHero = new Dictionary<Guid, Ranking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static DailyServerLevelRankingManager s_instance = new DailyServerLevelRankingManager();

	public int rankingNo => m_nRankingNo;

	public static DailyServerLevelRankingManager instance => s_instance;

	private DailyServerLevelRankingManager()
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
			nLastRankingNo = GameDac.LastDailyServerLevelRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.DailyServerLevelRankings(conn, null, nLastRankingNo);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
		Cache cache = Cache.instance;
		lock (Global.syncObject)
		{
			ClearRankings();
			foreach (DataRow dr in drcRankings)
			{
				Ranking ranking = new Ranking();
				ranking.Init(dr);
				AddRanking(ranking);
				if (ranking.ranking == 1)
				{
					cache.UpdateServerMaxLevel(ranking.level, bSendEvent);
				}
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
		foreach (Hero hero in Cache.instance.heroes.Values)
		{
			if (hero.isLoggedIn)
			{
				Ranking ranking = GetRankingOfHero(hero.id);
				ServerEvent.SendDailyServerLevelRankingUpdated(hero.account.peer, m_nRankingNo, ranking?.ranking ?? 0);
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

	private void ClearRankings()
	{
		m_rankingsByHero.Clear();
	}

	private void AddRanking(Ranking ranking)
	{
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
