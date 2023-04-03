using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardShopFixedProduct
{
	private int m_nId;

	private Item m_item;

	private bool m_bOwned;

	private int m_nSaleSoulPowder;

	public int id => m_nId;

	public Item item => m_item;

	public bool owned => m_bOwned;

	public int saleSoulPowder => m_nSaleSoulPowder;

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
		m_bOwned = Convert.ToBoolean(dr["itemOwned"]);
		m_nSaleSoulPowder = Convert.ToInt32(dr["saleSoulPowder"]);
		if (m_nSaleSoulPowder <= 0)
		{
			SFLogUtil.Warn(GetType(), "판매영혼가루가 유효하지 않습니다. m_nId = " + m_nId + ", m_nSaleSoulPowder = " + m_nSaleSoulPowder);
		}
	}
}
