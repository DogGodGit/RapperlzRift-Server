using System;
using System.Data;

namespace GameServer;

public class FieldOfHonorHeroSkill
{
	private FieldOfHonorHero m_hero;

	private int m_nId;

	private int m_nLevel;

	public FieldOfHonorHero hero => m_hero;

	public int id
	{
		get
		{
			return m_nId;
		}
		set
		{
			m_nId = value;
		}
	}

	public int level
	{
		get
		{
			return m_nLevel;
		}
		set
		{
			m_nLevel = value;
		}
	}

	public FieldOfHonorHeroSkill(FieldOfHonorHero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["skillId"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
	}
}
