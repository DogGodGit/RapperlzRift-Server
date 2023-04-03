using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DailyFieldOfHonorRanking
{
	private int m_nRankingNo;

	private int m_nRanking;

	private Guid m_heroId = Guid.Empty;

	public int rankingNo => m_nRankingNo;

	public int ranking => m_nRanking;

	public Guid heroId => m_heroId;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRankingNo = Convert.ToInt32(dr["rankingNo"]);
		m_nRanking = Convert.ToInt32(dr["ranking"]);
		m_heroId = SFDBUtil.ToGuid(dr["heroId"]);
	}
}
