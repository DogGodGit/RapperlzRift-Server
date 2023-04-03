using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CashProduct
{
	private int m_nId;

	private string m_sInAppProductKey;

	private CashProductType m_type;

	private int m_nUnOwnDia;

	private Item m_item;

	private bool m_bItemOwned;

	private int m_nItemCount;

	private int m_nVipPoint;

	private int m_nFirstPurchaseBonusUnOwnDia;

	public int id => m_nId;

	public string inAppProductKey => m_sInAppProductKey;

	public CashProductType type => m_type;

	public int unOwnDia => m_nUnOwnDia;

	public Item item => m_item;

	public bool itemOwned => m_bItemOwned;

	public int itemCount => m_nItemCount;

	public int vipPoint => m_nVipPoint;

	public int firstPurchaseBonusUnOwnDia => m_nFirstPurchaseBonusUnOwnDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		m_sInAppProductKey = Convert.ToString(dr["inAppProductKey"]);
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(CashProductType), nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		m_type = (CashProductType)nType;
		switch (m_type)
		{
		case CashProductType.None:
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
			break;
		case CashProductType.Dia:
			m_nUnOwnDia = Convert.ToInt32(dr["unOwnDia"]);
			if (m_nUnOwnDia <= 0)
			{
				SFLogUtil.Warn(GetType(), "비귀속다이아가 유효하지 않습니다. m_nId = " + m_nId + ", m_nUnOwnDia = " + m_nUnOwnDia);
			}
			break;
		case CashProductType.Item:
		{
			int nItemId = Convert.ToInt32(dr["itemId"]);
			if (nItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템ID가 유효하지 않습니다. m_nId = " + m_nId + ", nItemId = " + nItemId);
				break;
			}
			m_item = Resource.instance.GetItem(nItemId);
			if (m_item == null)
			{
				SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nId = " + m_nId + ", nItemId = " + nItemId);
				break;
			}
			m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
			m_nItemCount = Convert.ToInt32(dr["itemCount"]);
			if (m_nItemCount <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nItemCount = " + m_nItemCount);
			}
			break;
		}
		}
		m_nVipPoint = Convert.ToInt32(dr["vipPoint"]);
		if (m_nVipPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "VIP포인트가 유효하지 않습니다. m_nId = " + m_nId + ", m_nVipPoint = " + m_nVipPoint);
		}
		m_nFirstPurchaseBonusUnOwnDia = Convert.ToInt32(dr["firstPurchaseBonusUnOwnDia"]);
	}
}
