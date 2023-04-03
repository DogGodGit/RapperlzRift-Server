using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroCreatureCard
{
	private Hero m_hero;

	private CreatureCard m_card;

	private int m_nCount;

	public Hero hero => m_hero;

	public CreatureCard card => m_card;

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

	public HeroCreatureCard(Hero hero)
		: this(hero, null)
	{
	}

	public HeroCreatureCard(Hero hero, CreatureCard creatureCard)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_card = creatureCard;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
		m_card = Resource.instance.GetCreatureCard(nCreatureCardId);
		if (m_card == null)
		{
			throw new Exception("크리처카드가 존재하지 않습니다. nCreatureCardId = " + nCreatureCardId);
		}
		m_nCount = Convert.ToInt32(dr["count"]);
	}

	public int GetSurplusCount()
	{
		return Math.Max(m_nCount - GetNotActivatedCollectionCount(), 0);
	}

	public int GetNotActivatedCollectionCount()
	{
		int nCount = 0;
		foreach (CreatureCardCollection collection in m_card.relatedCollections.Values)
		{
			if (!m_hero.IsCreatureCardCollectionActivated(collection.id))
			{
				nCount++;
			}
		}
		return nCount;
	}

	public PDHeroCreatureCard ToPDHeroCreatureCard()
	{
		PDHeroCreatureCard inst = new PDHeroCreatureCard();
		inst.creatureCardId = m_card.id;
		inst.count = m_nCount;
		return inst;
	}

	public static List<PDHeroCreatureCard> ToPDHeroCreatureCards(IEnumerable<HeroCreatureCard> cards)
	{
		List<PDHeroCreatureCard> insts = new List<PDHeroCreatureCard>();
		foreach (HeroCreatureCard card in cards)
		{
			insts.Add(card.ToPDHeroCreatureCard());
		}
		return insts;
	}
}
