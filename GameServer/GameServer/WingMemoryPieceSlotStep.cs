using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceSlotStep
{
	private WingMemoryPieceSlot m_slot;

	private int m_nStep;

	private int m_nAttrMaxValue;

	private int m_nAttrIncBaseValue;

	private int m_nAttrDecValue;

	public WingMemoryPieceSlot slot => m_slot;

	public int step => m_nStep;

	public int attrMaxValue => m_nAttrMaxValue;

	public int attrIncBaseValue => m_nAttrIncBaseValue;

	public int attrDecValue => m_nAttrDecValue;

	public WingMemoryPieceSlotStep(WingMemoryPieceSlot slot)
	{
		if (slot == null)
		{
			throw new ArgumentNullException("slot");
		}
		m_slot = slot;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep < 0)
		{
			SFLogUtil.Warn(GetType(), "단계가 유효하지 않습니다. m_nStep = " + m_nStep);
		}
		m_nAttrMaxValue = Convert.ToInt32(dr["attrMaxValue"]);
		if (m_nAttrMaxValue < 0)
		{
			SFLogUtil.Warn(GetType(), "속성최대값이 유효하지 않습니다. m_nStep = " + m_nStep + ", m_nAttrMaxValue = " + m_nAttrMaxValue);
		}
		m_nAttrIncBaseValue = Convert.ToInt32(dr["attrIncBaseValue"]);
		if (m_nAttrIncBaseValue < 0)
		{
			SFLogUtil.Warn(GetType(), "속성증가기본값이 유효하지 않습니다. m_nStep = " + m_nStep + ", m_nAttrIncBaseValue = " + m_nAttrIncBaseValue);
		}
		m_nAttrDecValue = Convert.ToInt32(dr["attrDecValue"]);
		if (m_nAttrDecValue < 0)
		{
			SFLogUtil.Warn(GetType(), "속성감소값이 유효하지 않습니다. m_nStep = " + m_nStep + ", m_nAttrDecValue = " + m_nAttrDecValue);
		}
	}
}
