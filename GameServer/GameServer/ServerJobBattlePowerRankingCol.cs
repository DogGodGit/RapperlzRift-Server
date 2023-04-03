using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class ServerJobBattlePowerRankingCollection
{
	private int m_nJobId;

	private List<Ranking> m_rankings = new List<Ranking>();

	public int jobId => m_nJobId;

	public List<Ranking> rankings => m_rankings;

	public ServerJobBattlePowerRankingCollection(int nJobId)
	{
		m_nJobId = nJobId;
	}

	public void ClearRankings()
	{
		m_rankings.Clear();
	}

	public void AddRanking(Ranking ranking)
	{
		m_rankings.Add(ranking);
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
