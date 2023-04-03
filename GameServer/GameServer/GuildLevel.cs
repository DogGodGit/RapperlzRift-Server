using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildLevel
{
	private int m_nLevel;

	private int m_nMaxMemberCount;

	private ItemReward m_dailyItemReward;

	private ItemReward m_altarItemReward;

	public int level => m_nLevel;

	public int maxMemberCount => m_nMaxMemberCount;

	public ItemReward dailyItemReward => m_dailyItemReward;

	public ItemReward altarItemReawrd => m_altarItemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nMaxMemberCount = Convert.ToInt32(dr["maxMemberCount"]);
		if (m_nMaxMemberCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "최대멤버수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nMaxMemberCount = " + m_nMaxMemberCount);
		}
		long lnDailyItemRewardId = Convert.ToInt64(dr["dailyItemRewardId"]);
		m_dailyItemReward = Resource.instance.GetItemReward(lnDailyItemRewardId);
		if (m_dailyItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "일일보상아이템이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnDailyItemRewardId = " + lnDailyItemRewardId);
		}
		long lnAltarItemRewardId = Convert.ToInt64(dr["altarItemRewardId"]);
		m_altarItemReward = Resource.instance.GetItemReward(lnAltarItemRewardId);
		if (m_altarItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "제단보상아이템이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnAltarItemRewardId = " + lnAltarItemRewardId);
		}
	}
}
