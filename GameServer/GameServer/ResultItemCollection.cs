using System;
using System.Collections.Generic;

namespace GameServer;

public class ResultItemCollection
{
	private List<ResultItem> m_resultItems = new List<ResultItem>();

	public List<ResultItem> resultItems => m_resultItems;

	private ResultItem GetResultItem(Item item, bool bOwned)
	{
		for (int i = 0; i < m_resultItems.Count; i++)
		{
			ResultItem reward = m_resultItems[i];
			if (item.id == reward.item.id && bOwned == reward.owned)
			{
				return reward;
			}
		}
		return null;
	}

	private ResultItem GetOrCreateResultItem(Item item, bool bOwned)
	{
		ResultItem reward = GetResultItem(item, bOwned);
		if (reward == null)
		{
			reward = new ResultItem(item, bOwned);
			m_resultItems.Add(reward);
		}
		return reward;
	}

	public void AddResultItemCount(Item item, bool bOwned, int nCount)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (nCount != 0)
		{
			GetOrCreateResultItem(item, bOwned).count += nCount;
		}
	}

	public void Clear()
	{
		m_resultItems.Clear();
	}
}
