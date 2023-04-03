using System;
using System.Collections.Generic;

namespace GameServer;

public class ProofOfValorCreatureCardPool
{
	private ProofOfValor m_ProofOfValor;

	private int m_nId;

	private List<ProofOfValorCreatureCardPoolEntry> m_entries = new List<ProofOfValorCreatureCardPoolEntry>();

	private int m_nCreatureCardPoolEntryTotalPoint;

	public ProofOfValor proofOfValor => m_ProofOfValor;

	public int id => m_nId;

	public ProofOfValorCreatureCardPool(ProofOfValor proofOfValor, int nId)
	{
		m_ProofOfValor = proofOfValor;
		m_nId = nId;
	}

	public void AddEntry(ProofOfValorCreatureCardPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nCreatureCardPoolEntryTotalPoint += entry.point;
	}

	public CreatureCard SelectCreatureCard()
	{
		return SelectEntry().creatureCard;
	}

	private ProofOfValorCreatureCardPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nCreatureCardPoolEntryTotalPoint);
	}
}
