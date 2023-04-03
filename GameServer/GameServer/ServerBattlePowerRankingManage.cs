using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ServerBattlePowerRankingManager
{
	private int m_nRankingNo;

	private List<Ranking> m_rankings = new List<Ranking>();

	private Dictionary<Guid, Ranking> m_rankingsByHero = new Dictionary<Guid, Ranking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static ServerBattlePowerRankingManager s_instance = new ServerBattlePowerRankingManager();

	public static ServerBattlePowerRankingManager instance => s_instance;

	private ServerBattlePowerRankingManager()
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
			nLastRankingNo = GameDac.LastServerBattlePowerRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.ServerBattlePowerRankings(conn, null, nLastRankingNo);
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
		m_rankings.Clear();
		m_rankingsByHero.Clear();
	}

	private void AddRanking(Ranking ranking)
	{
		m_rankings.Add(ranking);
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

	public List<PDRanking> GetPDRankings(int nCount)
	{
		List<PDRanking> insts = new List<PDRanking>();
		int nLoopCount = Math.Min(nCount, m_rankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_rankings[i].ToPDRanking());
		}
		return insts;
	}
}
