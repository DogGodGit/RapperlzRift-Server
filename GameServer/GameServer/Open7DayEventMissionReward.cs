using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Open7DayEventMissionReward
{
	private Open7DayEventMission m_mission;

	private int m_nNo;

	private ItemReward m_itemReward;

	public Open7DayEventMission mission => m_mission;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public Open7DayEventMissionReward(Open7DayEventMission mission)
	{
		m_mission = mission;
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
			SFLogUtil.Warn(GetType(), "번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상아이템이 존재하지 않습니다.  m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
	}
}
