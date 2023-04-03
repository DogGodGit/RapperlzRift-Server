using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ServerPresentPopularityPointRankingCommandHandler : InGameCommandHandler<ServerPresentPopularityPointRankingCommandBody, ServerPresentPopularityPointRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerPresentPopularityPointRankingManager manager = ServerPresentPopularityPointRankingManager.instance;
		PresentPopularityPointRanking myRanking = manager.GetRankingOfHero(m_myHero.id);
		ServerPresentPopularityPointRankingResponseBody resBody = new ServerPresentPopularityPointRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = manager.GetPDRankings(Resource.instance.presentPopularityPointRankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
public class ServerPresentPopularityPointRankingManager
{
	private int m_nRankingNo;

	private List<PresentPopularityPointRanking> m_rankings = new List<PresentPopularityPointRanking>();

	private Dictionary<Guid, PresentPopularityPointRanking> m_rankingsByHero = new Dictionary<Guid, PresentPopularityPointRanking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static ServerPresentPopularityPointRankingManager s_instance = new ServerPresentPopularityPointRankingManager();

	public static ServerPresentPopularityPointRankingManager instance => s_instance;

	private ServerPresentPopularityPointRankingManager()
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
			nLastRankingNo = GameDac.LastServerPresentPopularityPointRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.ServerPresentPopularityPointRankings(conn, null, nLastRankingNo);
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
				PresentPopularityPointRanking ranking = new PresentPopularityPointRanking();
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

	private void AddRanking(PresentPopularityPointRanking ranking)
	{
		m_rankings.Add(ranking);
		m_rankingsByHero.Add(ranking.heroId, ranking);
	}

	public PresentPopularityPointRanking GetRankingOfHero(Guid heroId)
	{
		if (!m_rankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDPresentPopularityPointRanking> GetPDRankings(int nCount)
	{
		List<PDPresentPopularityPointRanking> insts = new List<PDPresentPopularityPointRanking>();
		int nLoopCount = Math.Min(nCount, m_rankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_rankings[i].ToPDRanking());
		}
		return insts;
	}
}
