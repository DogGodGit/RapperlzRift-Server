using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Accomplishment
{
	private int m_nId;

	private int m_nCategoryId;

	private AccomplishmentType m_type;

	private long m_lnObjectiveValue;

	private int m_nPoint;

	private int m_nRewardType;

	private int m_nRewardTitleId;

	private ItemReward m_rewardItem;

	public int id => m_nId;

	public int categoryId => m_nCategoryId;

	public AccomplishmentType type => m_type;

	public long objectiveValue => m_lnObjectiveValue;

	public int point => m_nPoint;

	public int rewardType => m_nRewardType;

	public int rewardTitle => m_nRewardTitleId;

	public ItemReward rewardItem => m_rewardItem;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["accomplishmentId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "업적ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nCategoryId = Convert.ToInt32(dr["categoryId"]);
		if (m_nCategoryId <= 0)
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nCategoryId = " + m_nCategoryId);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(AccomplishmentType), nType))
		{
			SFLogUtil.Warn(GetType(), "업적타입이 유효하지 않습니다. nType = " + nType);
		}
		m_type = (AccomplishmentType)nType;
		m_lnObjectiveValue = Convert.ToInt64(dr["objectiveValue"]);
		if (m_lnObjectiveValue <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표값이 유효하지 않습니다. m_nId = " + m_nId + ", m_lnObjectiveValue = " + m_lnObjectiveValue);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "업적점수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
		m_nRewardType = Convert.ToInt32(dr["rewardType"]);
		if (m_nRewardType < 0)
		{
			SFLogUtil.Warn(GetType(), "보상타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRewardType = " + m_nRewardType);
		}
		m_nRewardTitleId = Convert.ToInt32(dr["rewardTitleId"]);
		if (m_nRewardTitleId < 0)
		{
			SFLogUtil.Warn(GetType(), "칭호보상이 존재하지 않습니다. m_nId = " + m_nId + ", m_nRewardTitleId = " + m_nRewardTitleId);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_rewardItem = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_rewardItem == null)
		{
			SFLogUtil.Warn(GetType(), "아이템 보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
