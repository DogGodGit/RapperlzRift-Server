using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TimeDesignationEventReward
{
	private TimeDesignationEvent m_designationEvent;

	private int m_nNo;

	private ItemReward m_itemReward;

	public TimeDesignationEvent designationEvent => m_designationEvent;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public TimeDesignationEventReward(TimeDesignationEvent designationEvent)
	{
		if (designationEvent == null)
		{
			throw new ArgumentNullException("designationEvent");
		}
		m_designationEvent = designationEvent;
	}

	public void Set(DataRow dr)
	{
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
