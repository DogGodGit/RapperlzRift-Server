using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SeriesMissionStepReward
{
	private SeriesMissionStep m_step;

	private int m_nRewardNo;

	private ItemReward m_itemReward;

	public SeriesMissionStep step
	{
		get
		{
			return m_step;
		}
		set
		{
			m_step = value;
		}
	}

	public int rewardNo => m_nRewardNo;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRewardNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nRewardNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다. m_nRewardNo = " + m_nRewardNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "연속미션보상아이템이 존재하지 않습니다. lnItemRewardId = " + lnItemRewardId);
		}
	}
}
