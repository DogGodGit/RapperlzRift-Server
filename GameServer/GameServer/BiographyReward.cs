using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BiographyReward
{
	private Biography m_biography;

	private int m_nNo;

	private ItemReward m_itemReward;

	public Biography biography => m_biography;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public BiographyReward(Biography biography)
	{
		if (biography == null)
		{
			throw new ArgumentNullException("biography");
		}
		m_biography = biography;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo < 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		long lnItemRewardId = Convert.ToInt32(dr["itemRewardId"]);
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
