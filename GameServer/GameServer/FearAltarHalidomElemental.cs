using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltarHalidomElemental
{
	private FearAltar m_fearAltar;

	private int m_nId;

	private ItemReward m_collectionItemReward;

	public FearAltar fearAltar => m_fearAltar;

	public int id => m_nId;

	public ItemReward collectionItemRewrad => m_collectionItemReward;

	public FearAltarHalidomElemental(FearAltar fearAltar)
	{
		m_fearAltar = fearAltar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["halidomElementalId"]);
		long lnCollectionItemRewardId = Convert.ToInt64(dr["collectionItemRewardId"]);
		if (lnCollectionItemRewardId > 0)
		{
			m_collectionItemReward = Resource.instance.GetItemReward(lnCollectionItemRewardId);
			if (m_collectionItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "수집아이템이 존재하지 않습니다. m_nId = " + m_nId + ", lnCollectionItemRewardId = " + lnCollectionItemRewardId);
			}
		}
		else if (lnCollectionItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "수집아이템보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnCollectionItemRewardId = " + lnCollectionItemRewardId);
		}
	}
}
