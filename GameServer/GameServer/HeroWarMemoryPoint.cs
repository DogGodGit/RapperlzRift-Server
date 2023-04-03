using System;
using ClientCommon;

namespace GameServer;

public class HeroWarMemoryPoint
{
	private Hero m_hero;

	private DateTimeOffset m_lastPointAcquisitionTime = DateTimeOffset.MinValue;

	private int m_nPoint;

	private int m_nRank;

	public Hero hero => m_hero;

	public DateTimeOffset lastPointAcquisitionTime => m_lastPointAcquisitionTime;

	public int point => m_nPoint;

	public int rank
	{
		get
		{
			return m_nRank;
		}
		set
		{
			m_nRank = value;
		}
	}

	public HeroWarMemoryPoint(Hero hero)
	{
		m_hero = hero;
	}

	public void AddPoint(DateTimeOffset time, int nAmount)
	{
		if (nAmount > 0)
		{
			m_lastPointAcquisitionTime = time;
			m_nPoint += nAmount;
		}
	}

	public int CompareTo(HeroWarMemoryPoint other)
	{
		if (other == null)
		{
			return 1;
		}
		int nResult = m_nPoint.CompareTo(other.point);
		if (nResult == 0)
		{
			return -m_lastPointAcquisitionTime.CompareTo(other.lastPointAcquisitionTime);
		}
		return nResult;
	}

	public PDWarMemoryPoint ToPDWarMemoryPoint()
	{
		PDWarMemoryPoint inst = new PDWarMemoryPoint();
		inst.heroId = (Guid)m_hero.id;
		inst.name = m_hero.name;
		inst.point = m_nPoint;
		inst.pointUpdatedTimeTicks = m_lastPointAcquisitionTime.Ticks;
		return inst;
	}

	public PDWarMemoryRanking ToPDWarMemoryRanking()
	{
		PDWarMemoryRanking inst = new PDWarMemoryRanking();
		inst.rank = m_nRank;
		inst.heroId = (Guid)m_hero.id;
		inst.name = m_hero.name;
		inst.jobId = m_hero.jobId;
		inst.point = m_nPoint;
		return inst;
	}

	public static int Compare(HeroWarMemoryPoint x, HeroWarMemoryPoint y)
	{
		if (x == null)
		{
			if (y != null)
			{
				return -1;
			}
			return 0;
		}
		return x.CompareTo(y);
	}
}
