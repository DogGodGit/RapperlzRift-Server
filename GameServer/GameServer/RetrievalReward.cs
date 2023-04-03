using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RetrievalReward
{
	private Retrieval m_retrieval;

	private int m_nLevel;

	private ExpReward m_goldExpReward;

	private ItemReward m_goldItemReward;

	private ExpReward m_diaExpReward;

	private ItemReward m_diaItemReward;

	public Retrieval retrieval => m_retrieval;

	public int level => m_nLevel;

	public ExpReward goldExpReward => m_goldExpReward;

	public long goldExpRewardValue
	{
		get
		{
			if (m_goldExpReward == null)
			{
				return 0L;
			}
			return m_goldExpReward.value;
		}
	}

	public ItemReward goldItemReward => m_goldItemReward;

	public ExpReward diaExpReward => m_diaExpReward;

	public long diaExpRewardValue
	{
		get
		{
			if (m_diaExpReward == null)
			{
				return 0L;
			}
			return m_diaExpReward.value;
		}
	}

	public ItemReward diaItemReward => m_diaItemReward;

	public RetrievalReward(Retrieval retrieval)
	{
		if (retrieval == null)
		{
			throw new ArgumentNullException("retrieval");
		}
		m_retrieval = retrieval;
	}

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
		switch (m_retrieval.type)
		{
		case RetrievalRewardDisplayType.ExpReward:
		{
			long lnGoldExpRewardId = Convert.ToInt64(dr["goldExpRewardId"]);
			if (lnGoldExpRewardId > 0)
			{
				m_goldExpReward = Resource.instance.GetExpReward(lnGoldExpRewardId);
				if (m_goldExpReward == null)
				{
					SFLogUtil.Warn(GetType(), "골드보상경험치가 존재하지 않습니다. lnGoldExpRewardId = " + lnGoldExpRewardId);
				}
			}
			long lnDiaExpRewardId = Convert.ToInt64(dr["diaExpRewardId"]);
			if (lnDiaExpRewardId > 0)
			{
				m_diaExpReward = Resource.instance.GetExpReward(lnDiaExpRewardId);
				if (m_diaExpReward == null)
				{
					SFLogUtil.Warn(GetType(), "다이아회수보상경험치가 존재하지 않습니다. lnDiaExpRewardId = " + lnDiaExpRewardId);
				}
			}
			break;
		}
		case RetrievalRewardDisplayType.ItemReward:
		{
			long lnGoldItemRewardId = Convert.ToInt64(dr["goldItemRewardId"]);
			if (lnGoldItemRewardId > 0)
			{
				m_goldItemReward = Resource.instance.GetItemReward(lnGoldItemRewardId);
				if (m_goldItemReward == null)
				{
					SFLogUtil.Warn(GetType(), "골드회수보상아이템이 존재하지 않습니다. lnGoldItemRewardId = " + lnGoldItemRewardId);
				}
			}
			long lnDiaItemRewardId = Convert.ToInt64(dr["diaItemRewardId"]);
			if (lnDiaItemRewardId > 0)
			{
				m_diaItemReward = Resource.instance.GetItemReward(lnDiaItemRewardId);
				if (m_diaItemReward == null)
				{
					SFLogUtil.Warn(GetType(), "다이아회수보상아이템이 존재하지 않습니다. lnDiaItemRewardId = " + lnDiaItemRewardId);
				}
			}
			break;
		}
		}
	}
}
