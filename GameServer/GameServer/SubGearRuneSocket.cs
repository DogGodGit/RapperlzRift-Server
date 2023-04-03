using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class SubGearRuneSocket
{
	private SubGear m_gear;

	private int m_nIndex;

	private int m_nRequiredGearLevel;

	private HashSet<int> m_availableItemTypes = new HashSet<int>();

	public SubGear gear
	{
		get
		{
			return m_gear;
		}
		set
		{
			m_gear = value;
		}
	}

	public int index => m_nIndex;

	public int requiredGearLevel => m_nRequiredGearLevel;

	public HashSet<int> availableItemType => m_availableItemTypes;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nIndex = Convert.ToInt32(dr["socketIndex"]);
		m_nRequiredGearLevel = Convert.ToInt32(dr["requiredSubGearLevel"]);
	}

	public void AddAvailableItemType(int nItemType)
	{
		if (nItemType <= 0)
		{
			throw new ArgumentNullException("itemType");
		}
		m_availableItemTypes.Add(nItemType);
	}

	public bool ContainsAvailableItemType(int nItemType)
	{
		return m_availableItemTypes.Contains(nItemType);
	}
}
