using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardCollectionEntry
{
	private CreatureCardCollection m_collection;

	private CreatureCard m_card;

	public CreatureCardCollection collection => m_collection;

	public CreatureCard card => m_card;

	public CreatureCardCollectionEntry(CreatureCardCollection collection)
	{
		m_collection = collection;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nCardId = Convert.ToInt32(dr["creatureCardId"]);
		m_card = Resource.instance.GetCreatureCard(nCardId);
		if (m_card == null)
		{
			SFLogUtil.Warn(GetType(), "크리처카드가 존재하지 않습니다. nCardId = " + nCardId);
		}
	}
}
