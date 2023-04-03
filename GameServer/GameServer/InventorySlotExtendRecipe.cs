using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class InventorySlotExtendRecipe
{
	private int m_nSlotCount;

	private int m_nDia;

	public int slotCount => m_nSlotCount;

	public int dia => m_nDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSlotCount = Convert.ToInt32(dr["slotCount"]);
		if (m_nSlotCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "슬롯카운트가 유효하지 않습니다. m_nSlotCount = " + m_nSlotCount);
		}
		m_nDia = Convert.ToInt32(dr["dia"]);
		if (m_nDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "다이아가 유효하지않습니다. m_nSlotCount = " + m_nSlotCount + ", m_nDia = " + m_nDia);
		}
	}
}
