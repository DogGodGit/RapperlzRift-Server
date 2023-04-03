using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AncientRelicStepRewardPoolEntry : IPickEntry
{
	private AncientRelicStepRewardPool m_pool;

	private int m_nId;

	private int m_nPoint;

	private ItemReward m_itemReward;

	public AncientRelic ancientRelic => m_pool.ancientRelic;

	public AncientRelicStep step => m_pool.step;

	public AncientRelicStepRewardPoolCollection poolCollection => m_pool.collection;

	public AncientRelicStepRewardPool pool => m_pool;

	public int id => m_nId;

	public int point => m_nPoint;

	public ItemReward itemReward => m_itemReward;

	int IPickEntry.point => m_nPoint;

	public AncientRelicStepRewardPoolEntry(AncientRelicStepRewardPool pool)
	{
		m_pool = pool;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["entryId"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. stepNo = " + step.no + ", level = " + poolCollection.level + ", poolId = " + pool.id + ", m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. stepNo = " + step.no + ", level = " + poolCollection.level + ", poolId = " + pool.id + ", m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
