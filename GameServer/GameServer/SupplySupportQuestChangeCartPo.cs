using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestChangeCartPoolEntry : IPickEntry
{
	private SupplySupportQuestCart m_cart;

	private int m_nNo;

	private int m_nPoint;

	private SupplySupportQuestCart m_acquisitionCart;

	public SupplySupportQuest supplySupportQuest => m_cart.supplySupportQuest;

	public SupplySupportQuestCart cart => m_cart;

	public int no => m_nNo;

	public int point => m_nPoint;

	public SupplySupportQuestCart acquisitionCart => m_acquisitionCart;

	int IPickEntry.point => m_nPoint;

	public SupplySupportQuestChangeCartPoolEntry(SupplySupportQuestCart cart)
	{
		m_cart = cart;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "획득가중치가 유효하지 않습니다. cartId = " + m_cart.id + ", m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		int nAcquisitionCartId = Convert.ToInt32(dr["acquisitionCartId"]);
		if (nAcquisitionCartId > 0)
		{
			m_acquisitionCart = supplySupportQuest.GetCart(nAcquisitionCartId);
			if (m_acquisitionCart == null)
			{
				SFLogUtil.Warn(GetType(), "획득수레가 존재하지 않습니다. cartId = " + m_cart.id + ", m_nNo = " + m_nNo + ", nAcquisitionCartId = " + nAcquisitionCartId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "획득수레ID가 유효하지 않습니다. cartId = " + m_cart.id + ", m_nNo = " + m_nNo + ", nAcquisitionCartId = " + nAcquisitionCartId);
		}
	}
}
