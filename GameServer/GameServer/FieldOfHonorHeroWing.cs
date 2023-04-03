using System;
using System.Data;

namespace GameServer;

public class FieldOfHonorHeroWing
{
	private FieldOfHonorHero m_hero;

	private int m_nId;

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

	public FieldOfHonorHeroWing(FieldOfHonorHero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["wingId"]);
	}
}
