using System;
using System.Collections.Generic;

namespace GameServer;

public class TodayMissionPool
{
	private int m_nLevel;

	private List<TodayMission> m_missions = new List<TodayMission>();

	private int m_nTotalPoint;

	public int level => m_nLevel;

	public List<TodayMission> missions => m_missions;

	public TodayMissionPool(int nLevel)
	{
		m_nLevel = nLevel;
	}

	public void Add(TodayMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission);
		m_nTotalPoint += mission.point;
	}

	public List<TodayMission> Select(int nCount)
	{
		return Util.SelectPickEntries(m_missions, m_nTotalPoint, nCount, bDuplicated: false);
	}
}
