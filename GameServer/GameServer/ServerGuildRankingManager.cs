using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ServerGuildRankingManager
{
	private int m_nRankingNo;

	private List<GuildRanking> m_rankings = new List<GuildRanking>();

	private Dictionary<Guid, GuildRanking> m_rankingsOfGuild = new Dictionary<Guid, GuildRanking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static ServerGuildRankingManager s_instance = new ServerGuildRankingManager();

	public static ServerGuildRankingManager instance => s_instance;

	private ServerGuildRankingManager()
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
			nLastRankingNo = GameDac.LastServerGuildRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.ServerGuildRankings(conn, null, nLastRankingNo);
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
				GuildRanking ranking = new GuildRanking();
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
		m_rankingsOfGuild.Clear();
	}

	private void AddRanking(GuildRanking ranking)
	{
		m_rankings.Add(ranking);
		m_rankingsOfGuild.Add(ranking.guildId, ranking);
	}

	public GuildRanking GetGuildRankingOfGuild(Guid guildId)
	{
		if (!m_rankingsOfGuild.TryGetValue(guildId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDGuildRanking> GetPDRankings(int nCount)
	{
		List<PDGuildRanking> insts = new List<PDGuildRanking>();
		int nLoopCount = Math.Min(nCount, m_rankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_rankings[i].ToPDGuildRanking());
		}
		return insts;
	}
}
