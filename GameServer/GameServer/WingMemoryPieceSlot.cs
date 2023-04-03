using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceSlot
{
	private Wing m_wing;

	private int m_nIndex;

	private int m_nAttrId;

	private int m_nOpenStep;

	private Dictionary<int, WingMemoryPieceSlotStep> m_steps = new Dictionary<int, WingMemoryPieceSlotStep>();

	public Wing wing => m_wing;

	public int index => m_nIndex;

	public int attrId => m_nAttrId;

	public int openStep => m_nOpenStep;

	public WingMemoryPieceSlot(Wing wing)
	{
		if (wing == null)
		{
			throw new ArgumentNullException("wing");
		}
		m_wing = wing;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nIndex = Convert.ToInt32(dr["slotIndex"]);
		if (m_nIndex < 0)
		{
			SFLogUtil.Warn(GetType(), "인덱스가 유효하지 않습니다. m_nIndex = " + m_nIndex);
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (!Attr.IsDefined(m_nAttrId))
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nIndex = " + m_nIndex + ", m_nAttrId = " + m_nAttrId);
		}
		m_nOpenStep = Convert.ToInt32(dr["openStep"]);
		if (m_nOpenStep < 0)
		{
			SFLogUtil.Warn(GetType(), "개방단계가 유효하지 않습니다. m_nIndex = " + m_nIndex + ", m_nOpenStep = " + m_nOpenStep);
		}
	}

	public void AddStep(WingMemoryPieceSlotStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step.step, step);
	}

	public WingMemoryPieceSlotStep GetStep(int nStep)
	{
		if (!m_steps.TryGetValue(nStep, out var value))
		{
			return null;
		}
		return value;
	}
}
