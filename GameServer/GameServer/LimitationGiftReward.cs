using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class LimitationGiftReward
{
	private LimitationGiftRewardSchedule m_schedule;

	private int m_nNo;

	private ItemReward m_itemReward;

	public LimitationGiftRewardSchedule schedule => m_schedule;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public LimitationGiftReward(LimitationGiftRewardSchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedule = schedule;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
	}
}
