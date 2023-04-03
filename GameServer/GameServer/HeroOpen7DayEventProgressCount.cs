using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroOpen7DayEventProgressCount
{
	private Hero m_hero;

	private int m_nType;

	private int m_nAccProgressCount;

	public Hero hero => m_hero;

	public int type => m_nType;

	public int accProgressCount => m_nAccProgressCount;

	public HeroOpen7DayEventProgressCount(Hero hero)
		: this(hero, 0)
	{
	}

	public HeroOpen7DayEventProgressCount(Hero hero, int nType)
	{
		m_hero = hero;
		m_nType = nType;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nType = Convert.ToInt32(dr["type"]);
		m_nAccProgressCount = Convert.ToInt32(dr["accProgressCount"]);
	}

	public void IncreaseProgressCount()
	{
		m_nAccProgressCount++;
		ServerEvent.SendOpen7DayEventProgressCountUpdated(m_hero.account.peer, ToPDHeroOpen7DayEventProgressCount());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroOpen7DayEventProgressCount(m_hero.id, m_nType, m_nAccProgressCount));
		dbWork.Schedule();
	}

	public PDHeroOpen7DayEventProgressCount ToPDHeroOpen7DayEventProgressCount()
	{
		PDHeroOpen7DayEventProgressCount inst = new PDHeroOpen7DayEventProgressCount();
		inst.type = m_nType;
		inst.accProgressCount = m_nAccProgressCount;
		return inst;
	}

	public static List<PDHeroOpen7DayEventProgressCount> ToPDHeroOpen7DayEventProgressCounts(IEnumerable<HeroOpen7DayEventProgressCount> progressCounts)
	{
		List<PDHeroOpen7DayEventProgressCount> insts = new List<PDHeroOpen7DayEventProgressCount>();
		foreach (HeroOpen7DayEventProgressCount progressCount in progressCounts)
		{
			insts.Add(progressCount.ToPDHeroOpen7DayEventProgressCount());
		}
		return insts;
	}
}
