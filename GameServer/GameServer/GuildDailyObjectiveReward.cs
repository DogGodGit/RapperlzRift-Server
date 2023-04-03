using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildDailyObjectiveReward
{
	private int m_nNo;

	private int m_nCompletionMemberCount;

	private ItemReward m_itemReward1;

	private ItemReward m_itemReward2;

	private ItemReward m_itemReward3;

	public int no => m_nNo;

	public int completionMemberCount => m_nCompletionMemberCount;

	public ItemReward itemReward1 => m_itemReward1;

	public ItemReward itemReward2 => m_itemReward2;

	public ItemReward itemReward3 => m_itemReward3;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상No가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nCompletionMemberCount = Convert.ToInt32(dr["completionMemberCount"]);
		if (m_nCompletionMemberCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "완료멤버수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nCompletionMemberCount = " + m_nCompletionMemberCount);
		}
		long lnItemReward1Id = Convert.ToInt64(dr["itemReward1Id"]);
		if (lnItemReward1Id > 0)
		{
			m_itemReward1 = Resource.instance.GetItemReward(lnItemReward1Id);
			if (m_itemReward1 == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상1이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemReward1Id = " + lnItemReward1Id);
			}
		}
		long lnItemReward2Id = Convert.ToInt64(dr["itemReward2Id"]);
		if (lnItemReward2Id > 0)
		{
			m_itemReward2 = Resource.instance.GetItemReward(lnItemReward2Id);
			if (m_itemReward2 == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상2이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemReward2Id = " + lnItemReward2Id);
			}
		}
		long lnItemReward3Id = Convert.ToInt64(dr["itemReward3Id"]);
		if (lnItemReward3Id > 0)
		{
			m_itemReward3 = Resource.instance.GetItemReward(lnItemReward3Id);
			if (m_itemReward3 == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상3이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemReward3Id = " + lnItemReward3Id);
			}
		}
	}
}
