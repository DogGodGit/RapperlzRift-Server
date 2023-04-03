using System;
using System.Data;

namespace GameServer;

public class HeroCreatureCardCollection
{
	private Hero m_hero;

	private CreatureCardCollection m_colletion;

	public Hero hero => m_hero;

	public CreatureCardCollection collection => m_colletion;

	public HeroCreatureCardCollection(Hero hero)
		: this(hero, null)
	{
	}

	public HeroCreatureCardCollection(Hero hero, CreatureCardCollection collection)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_colletion = collection;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nCollectionId = Convert.ToInt32(dr["collectionId"]);
		m_colletion = Resource.instance.GetCreatureCardCollection(nCollectionId);
		if (m_colletion == null)
		{
			throw new Exception("크리처카드컬렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId);
		}
	}
}
