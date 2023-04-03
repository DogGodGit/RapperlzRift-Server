using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TradeShipObjectDestroyerReward : IPickEntry
{
	private TradeShipObject m_obj;

	private int m_nNo;

	private int m_nPoint;

	private ItemReward m_itemReward;

	public TradeShipObject obj => m_obj;

	public int no => m_nNo;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public ItemReward itemReward => m_itemReward;

	public TradeShipObjectDestroyerReward(TradeShipObject obj)
	{
		m_obj = obj;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. difficulty = " + m_obj.difficulty.difficulty + ", objectId = " + m_obj.id + ", m_nNo = " + m_nNo);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. difficulty = " + m_obj.difficulty.difficulty + ", objectId = " + m_obj.id + ", m_nNo = " + m_nNo);
		}
	}
}
