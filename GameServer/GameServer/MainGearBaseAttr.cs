using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class MainGearBaseAttr
{
	private MainGear m_gear;

	private int m_nId;

	private List<MainGearBaseAttrEnchantLevel> m_enchantLevels = new List<MainGearBaseAttrEnchantLevel>();

	public MainGear gear
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

	public int id => m_nId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
	}

	public void AddEnchantLevel(MainGearBaseAttrEnchantLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		if (level.attr != null)
		{
			throw new Exception("이미 메인장비속성에 추가된 강화레벨입니다.");
		}
		m_enchantLevels.Add(level);
		level.attr = this;
	}

	public MainGearBaseAttrEnchantLevel GetEnchantLevel(int nLevel)
	{
		if (nLevel < 0 || nLevel >= m_enchantLevels.Count)
		{
			return null;
		}
		return m_enchantLevels[nLevel];
	}
}
