using System.Collections.Generic;

namespace GameServer;

public class AncientRelicStepRewardPoolCollection
{
	private AncientRelicStep m_step;

	private int m_nLevel;

	private List<AncientRelicStepRewardPool> m_pools = new List<AncientRelicStepRewardPool>();

	public AncientRelicStep step => m_step;

	public int level => m_nLevel;

	public int poolCount => m_pools.Count;

	public AncientRelicStepRewardPoolCollection(AncientRelicStep step, int nLevel)
	{
		m_step = step;
		m_nLevel = nLevel;
	}

	public AncientRelicStepRewardPool GetPool(int nPoolId)
	{
		int nIndex = nPoolId - 1;
		if (nIndex < 0 || nIndex >= m_pools.Count)
		{
			return null;
		}
		return m_pools[nIndex];
	}

	public AncientRelicStepRewardPool GetOrCreatePool(int nPoolId)
	{
		AncientRelicStepRewardPool pool = GetPool(nPoolId);
		if (pool == null)
		{
			pool = new AncientRelicStepRewardPool(this, nPoolId);
			m_pools.Add(pool);
		}
		return pool;
	}
}
