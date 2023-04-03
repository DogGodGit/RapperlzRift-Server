using System;
using System.Data;

namespace GameServer;

public class SubGearSoulstoneSocket
{
	private SubGear m_gear;

	private int m_nIndex;

	private int m_nItemType;

	private int m_nRequiredGrade;

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

	public int itemType => m_nItemType;

	public int requiredGrade => m_nRequiredGrade;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nIndex = Convert.ToInt32(dr["socketIndex"]);
		m_nItemType = Convert.ToInt32(dr["itemType"]);
		m_nRequiredGrade = Convert.ToInt32(dr["requiredSubGearGrade"]);
	}
}
