using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RookieGiftReward
{
	private RookieGift m_gift;

	private int m_nNo;

	private ItemReward m_itemReward;

	public RookieGift gift => m_gift;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public RookieGiftReward(RookieGift gift)
	{
		if (gift == null)
		{
			throw new ArgumentNullException("gift");
		}
		m_gift = gift;
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
		long lnItemRewardItemId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardItemId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "보상아이템이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardItemId = " + lnItemRewardItemId);
		}
	}
}
