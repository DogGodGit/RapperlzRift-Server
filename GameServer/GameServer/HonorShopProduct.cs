using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HonorShopProduct
{
	private int m_nId;

	private Item m_item;

	private bool m_bItemOwned;

	private int m_nRequiredHonorPoint;

	public int id => m_nId;

	public Item item => m_item;

	public bool itemOwned => m_bItemOwned;

	public int requiredHonorPoint => m_nRequiredHonorPoint;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "상품ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nId = " + m_nId + ", nItemId = " + nItemId);
		}
		m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
		m_nRequiredHonorPoint = Convert.ToInt32(dr["requiredHonorPoint"]);
		if (m_nRequiredHonorPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요명예포인트가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredHonorPoint = " + m_nRequiredHonorPoint);
		}
	}
}
