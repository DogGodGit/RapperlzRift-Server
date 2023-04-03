using System;
using System.Data;

namespace GameServer;

public class HeroAccessReward
{
	private Hero m_hero;

	private DateTime m_date = DateTime.MinValue;

	private int m_nEntryId;

	public Hero hero => m_hero;

	public DateTime date => m_date;

	public int id => m_nEntryId;

	public HeroAccessReward(Hero hero)
		: this(hero, DateTime.MinValue.Date, 0)
	{
	}

	public HeroAccessReward(Hero hero, DateTime date, int nEntry)
	{
		m_hero = hero;
		m_date = date;
		m_nEntryId = nEntry;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_date = Convert.ToDateTime(dr["date"]);
		m_nEntryId = Convert.ToInt32(dr["entryId"]);
	}
}
