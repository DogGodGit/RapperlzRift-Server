using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ServerPresentContributionPointRankingCommandHandler : InGameCommandHandler<ServerPresentContributionPointRankingCommandBody, ServerPresentContributionPointRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerPresentContributionPointRankingManager manager = ServerPresentContributionPointRankingManager.instance;
		PresentContributionPointRanking myRanking = manager.GetRankingOfHero(m_myHero.id);
		ServerPresentContributionPointRankingResponseBody resBody = new ServerPresentContributionPointRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = manager.GetPDRankings(Resource.instance.presentContributionPointRankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
public class ServerPresentContributionPointRankingManager
{
	private int m_nRankingNo;

	private List<PresentContributionPointRanking> m_rankings = new List<PresentContributionPointRanking>();

	private Dictionary<Guid, PresentContributionPointRanking> m_rankingsByHero = new Dictionary<Guid, PresentContributionPointRanking>();

	private SFRunnableStandaloneWork m_updateWork;

	private static ServerPresentContributionPointRankingManager s_instance = new ServerPresentContributionPointRankingManager();

	public static ServerPresentContributionPointRankingManager instance => s_instance;

	private ServerPresentContributionPointRankingManager()
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
			nLastRankingNo = GameDac.LastServerPresentContributionPointRankingNo(conn, null);
			if (m_nRankingNo == nLastRankingNo)
			{
				return;
			}
			drcRankings = GameDac.ServerPresentContributionPointRankings(conn, null, nLastRankingNo);
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
				PresentContributionPointRanking ranking = new PresentContributionPointRanking();
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

	private void AddRanking(PresentContributionPointRanking ranking)
	{
		m_rankings.Add(ranking);
		m_rankingsByHero.Add(ranking.heroId, ranking);
	}

	public PresentContributionPointRanking GetRankingOfHero(Guid heroId)
	{
		if (!m_rankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDPresentContributionPointRanking> GetPDRankings(int nCount)
	{
		List<PDPresentContributionPointRanking> insts = new List<PDPresentContributionPointRanking>();
		int nLoopCount = Math.Min(nCount, m_rankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_rankings[i].ToPDRanking());
		}
		return insts;
	}
}
