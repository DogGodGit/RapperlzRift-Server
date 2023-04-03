using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroWingMemoryPieceSlot : IPickEntry
{
	private HeroWing m_wing;

	private WingMemoryPieceSlot m_slot;

	private int m_nAccAttrValue;

	public HeroWing wing => m_wing;

	public WingMemoryPieceSlot slot => m_slot;

	public WingMemoryPieceSlotStep step => m_slot.GetStep(m_wing.memoryPieceStep);

	public int index => m_slot.index;

	public int accAttrValue => m_nAccAttrValue;

	public bool isOpend => m_slot.openStep <= m_wing.memoryPieceStep;

	public bool isCompleted => m_nAccAttrValue >= step.attrMaxValue;

	public int point => 1;

	public HeroWingMemoryPieceSlot(HeroWing wing)
		: this(wing, null)
	{
	}

	public HeroWingMemoryPieceSlot(HeroWing wing, WingMemoryPieceSlot slot)
	{
		if (wing == null)
		{
			throw new ArgumentNullException("wing");
		}
		m_wing = wing;
		m_slot = slot;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nIndex = Convert.ToInt32(dr["slotIndex"]);
		m_slot = m_wing.wing.GetMemoryPieceSlot(nIndex);
		m_nAccAttrValue = Convert.ToInt32(dr["accAttrValue"]);
	}

	public void IncreaseAccAttrValue(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nAccAttrValue += nAmount;
			int nAttrMaxValue = step.attrMaxValue;
			if (m_nAccAttrValue > nAttrMaxValue)
			{
				m_nAccAttrValue = nAttrMaxValue;
			}
		}
	}

	public void DecreaseAccAttrValue(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nAccAttrValue -= nAmount;
			if (m_nAccAttrValue < 0)
			{
				m_nAccAttrValue = 0;
			}
		}
	}

	public PDHeroWingMemoryPieceSlot ToPDHeroWingMemoryPieceSlot()
	{
		PDHeroWingMemoryPieceSlot inst = new PDHeroWingMemoryPieceSlot();
		inst.index = index;
		inst.accAttrValue = m_nAccAttrValue;
		return inst;
	}

	public static List<PDHeroWingMemoryPieceSlot> ToPDHeroWingMemoryPieceSlots(IEnumerable<HeroWingMemoryPieceSlot> slots)
	{
		List<PDHeroWingMemoryPieceSlot> insts = new List<PDHeroWingMemoryPieceSlot>();
		foreach (HeroWingMemoryPieceSlot slot in slots)
		{
			insts.Add(slot.ToPDHeroWingMemoryPieceSlot());
		}
		return insts;
	}
}
