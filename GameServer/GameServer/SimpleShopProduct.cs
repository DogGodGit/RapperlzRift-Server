using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SimpleShopProduct
{
	private int m_nId;

	private Item m_item;

	private bool m_bOwned;

	private int m_nSaleGold;

	public int id => m_nId;

	public Item item => m_item;

	public bool owned => m_bOwned;

	public int saleGold => m_nSaleGold;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "간이상점상품 ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nProductId = " + m_nId + ", nItemId = " + nItemId);
		}
		m_bOwned = Convert.ToBoolean(dr["itemOwned"]);
		m_nSaleGold = Convert.ToInt32(dr["saleGold"]);
		if (m_nSaleGold <= 0)
		{
			SFLogUtil.Warn(GetType(), "판매가격이 유효하지 않습니다. m_nProductId = " + m_nId + ", m_nSaleGold = " + m_nSaleGold);
		}
	}
}
