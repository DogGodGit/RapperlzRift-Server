using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildWeeklyObjective
{
	private int m_nId;

	private int m_nCompletionMemberCount;

	private ItemReward m_itemReward1;

	private ItemReward m_itemReward2;

	private ItemReward m_itemReward3;

	public int id => m_nId;

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
		m_nId = Convert.ToInt32(dr["objectiveId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nCompletionMemberCount = Convert.ToInt32(dr["completionMemberCount"]);
		if (m_nCompletionMemberCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "완료멤버수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nCompletionMemberCount = " + m_nCompletionMemberCount);
		}
		long lnItemReward1Id = Convert.ToInt64(dr["itemReward1Id"]);
		if (lnItemReward1Id > 0)
		{
			m_itemReward1 = Resource.instance.GetItemReward(lnItemReward1Id);
			if (m_itemReward1 == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상1이 존재하지 않습니다. m_nId = " + m_nId + ", lnItemReward1Id = " + lnItemReward1Id);
			}
		}
		long lnItemReward2Id = Convert.ToInt64(dr["itemReward2Id"]);
		if (lnItemReward2Id > 0)
		{
			m_itemReward2 = Resource.instance.GetItemReward(lnItemReward2Id);
			if (m_itemReward2 == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상2이 존재하지 않습니다. m_nId = " + m_nId + ", lnItemReward2Id = " + lnItemReward2Id);
			}
		}
		long lnItemReward3Id = Convert.ToInt64(dr["itemReward3Id"]);
		if (lnItemReward3Id > 0)
		{
			m_itemReward3 = Resource.instance.GetItemReward(lnItemReward3Id);
			if (m_itemReward3 == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상3이 존재하지 않습니다. m_nId = " + m_nId + ", lnItemReward3Id = " + lnItemReward3Id);
			}
		}
	}
}
