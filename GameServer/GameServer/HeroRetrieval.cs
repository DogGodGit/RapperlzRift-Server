using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroRetrieval
{
	private Hero m_hero;

	private int m_nId;

	private int m_nCount;

	public Hero hero => m_hero;

	public int id => m_nId;

	public int count
	{
		get
		{
			return m_nCount;
		}
		set
		{
			m_nCount = value;
		}
	}

	public HeroRetrieval(Hero hero)
		: this(hero, 0)
	{
	}

	public HeroRetrieval(Hero hero, int nId)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_nId = nId;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["retrievalId"]);
		m_nCount = Convert.ToInt32(dr["count"]);
	}

	public PDHeroRetrieval ToPDHeroRetrieval()
	{
		PDHeroRetrieval inst = new PDHeroRetrieval();
		inst.retrievalId = m_nId;
		inst.count = m_nCount;
		return inst;
	}

	public static List<PDHeroRetrieval> ToPDHeroRetrievals(IEnumerable<HeroRetrieval> retrievals)
	{
		List<PDHeroRetrieval> insts = new List<PDHeroRetrieval>();
		foreach (HeroRetrieval retrieval in retrievals)
		{
			insts.Add(retrieval.ToPDHeroRetrieval());
		}
		return insts;
	}
}
