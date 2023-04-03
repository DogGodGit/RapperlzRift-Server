using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimReward
{
	private RuinsReclaim m_ruinsReclaim;

	private int m_nNo;

	private ItemReward m_itemReward;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public RuinsReclaimReward(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
