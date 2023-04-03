using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class Wing
{
	private int m_nId;

	private string m_sNameKey;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	private bool m_bMemoryPieceInstallationEnabled;

	private List<WingMemoryPieceSlot> m_memoryPieceSlots = new List<WingMemoryPieceSlot>();

	private List<WingMemoryPieceStep> m_memoryPieceSteps = new List<WingMemoryPieceStep>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	public bool memoryPieceInstallationEnabled => m_bMemoryPieceInstallationEnabled;

	public List<WingMemoryPieceSlot> memoryPieceSlots => m_memoryPieceSlots;

	public List<WingMemoryPieceStep> memoryPieceSteps => m_memoryPieceSteps;

	public WingMemoryPieceStep lastMemoryPieceStep => m_memoryPieceSteps.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["wingId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "날개ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성 ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nAttrId = " + m_nAttrId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
		m_bMemoryPieceInstallationEnabled = Convert.ToBoolean(dr["memoryPieceInstallationEnabled"]);
	}

	public void AddMemoryPieceSlot(WingMemoryPieceSlot slot)
	{
		if (slot == null)
		{
			throw new ArgumentNullException("slot");
		}
		m_memoryPieceSlots.Add(slot);
	}

	public WingMemoryPieceSlot GetMemoryPieceSlot(int nSlotIndex)
	{
		if (nSlotIndex < 0 || nSlotIndex >= m_memoryPieceSlots.Count)
		{
			return null;
		}
		return m_memoryPieceSlots[nSlotIndex];
	}

	public void AddMemoryPieceStep(WingMemoryPieceStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_memoryPieceSteps.Add(step);
	}

	public WingMemoryPieceStep GetMemoryPieceStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex > m_memoryPieceSteps.Count)
		{
			return null;
		}
		return m_memoryPieceSteps[nIndex];
	}
}
