using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FirstChargeEventReward
{
	private FirstChargeEvent m_evt;

	private int m_nNo;

	private ItemReward m_itemReward;

	public FirstChargeEvent evt => m_evt;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public FirstChargeEventReward(FirstChargeEvent evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException("evt");
		}
		m_evt = evt;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId <= 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			return;
		}
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
