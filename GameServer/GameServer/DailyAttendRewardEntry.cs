using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DailyAttendRewardEntry
{
	private int m_nDay;

	private ItemReward m_itemReward;

	public int day => m_nDay;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDay = Convert.ToInt32(dr["day"]);
		if (m_nDay <= 0)
		{
			SFLogUtil.Warn(GetType(), "출석 일이 유효하지 않습니다. m_nDay = " + m_nDay);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템 보상이 존재하지 않습니다. m_nDay = " + m_nDay + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
