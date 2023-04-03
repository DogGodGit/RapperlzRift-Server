using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestCartPoolEntry : IPickEntry
{
	private SupplySupportQuestOrder m_order;

	private int m_nNo;

	private int m_nPoint;

	private SupplySupportQuestCart m_acquisitionCart;

	public SupplySupportQuest supplySupportQuest => m_order.supplySupportQuest;

	public SupplySupportQuestOrder order => m_order;

	public int no => m_nNo;

	public int point => m_nPoint;

	public SupplySupportQuestCart acquisitionCart => m_acquisitionCart;

	int IPickEntry.point => m_nPoint;

	public SupplySupportQuestCartPoolEntry(SupplySupportQuestOrder order)
	{
		m_order = order;
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
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. orderId = " + m_order.id + ", m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		int nAcquisitionCartId = Convert.ToInt32(dr["acquisitionCartId"]);
		if (nAcquisitionCartId > 0)
		{
			m_acquisitionCart = supplySupportQuest.GetCart(nAcquisitionCartId);
			if (m_acquisitionCart == null)
			{
				SFLogUtil.Warn(GetType(), "획득수레가 존재하지 않습니다. orderId = " + m_order.id + ", m_nNo = " + m_nNo + ", nAcquisitionCartId = " + nAcquisitionCartId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "획득수레ID가 유효하지 않습니다. orderId = " + m_order.id + ", m_nNo = " + m_nNo + ", nAcquisitionCartId = " + nAcquisitionCartId);
		}
	}
}
