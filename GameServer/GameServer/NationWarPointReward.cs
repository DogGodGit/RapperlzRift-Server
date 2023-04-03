using System;
using System.Data;

namespace GameServer;

public class NationWarPointReward
{
	private NationWar m_nationWar;

	private int m_nRatingDifference;

	private int m_nWinNationWarPoint;

	private int m_nLoseNationWarPoint;

	public NationWar nationWar => m_nationWar;

	public int ratingDifference => m_nRatingDifference;

	public int winNationWarPoint => m_nWinNationWarPoint;

	public int loseNationWarPoint => m_nLoseNationWarPoint;

	public NationWarPointReward(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRatingDifference = Convert.ToInt32(dr["ratingDifference"]);
		m_nWinNationWarPoint = Convert.ToInt32(dr["winNationWarPoint"]);
		m_nLoseNationWarPoint = Convert.ToInt32(dr["loseNationWarPoint"]);
	}
}
