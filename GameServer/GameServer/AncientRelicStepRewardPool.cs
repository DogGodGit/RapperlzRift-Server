using System;
using System.Collections.Generic;

namespace GameServer;

public class AncientRelicStepRewardPool
{
	private AncientRelicStepRewardPoolCollection m_collection;

	private int m_nId;

	private List<AncientRelicStepRewardPoolEntry> m_entries = new List<AncientRelicStepRewardPoolEntry>();

	private int m_nEntryTotalPoint;

	public AncientRelic ancientRelic => step.ancientRelic;

	public AncientRelicStep step => m_collection.step;

	public AncientRelicStepRewardPoolCollection collection => m_collection;

	public int id => m_nId;

	public AncientRelicStepRewardPool(AncientRelicStepRewardPoolCollection collection, int nId)
	{
		m_collection = collection;
		m_nId = nId;
	}

	public void AddEntry(AncientRelicStepRewardPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nEntryTotalPoint += entry.point;
	}

	public AncientRelicStepRewardPoolEntry GetEntry(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_entries.Count)
		{
			return null;
		}
		return m_entries[nIndex];
	}

	public ItemReward SelectItemReward()
	{
		return SelectEntry().itemReward;
	}

	private AncientRelicStepRewardPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nEntryTotalPoint);
	}
}
