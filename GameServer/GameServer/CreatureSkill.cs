using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureSkill
{
	private int m_nId;

	private int m_nAttrId;

	private Dictionary<int, CreatureSkillAttr> m_attrs = new Dictionary<int, CreatureSkillAttr>();

	public int id => m_nId;

	public int attrId => m_nAttrId;

	public Dictionary<int, CreatureSkillAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["skillId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "스킬ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nAttrId = " + m_nAttrId);
		}
	}

	public void AddAttr(CreatureSkillAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr.grade.grade, attr);
	}

	public CreatureSkillAttr GetAttr(int nGrade)
	{
		if (!m_attrs.TryGetValue(nGrade, out var value))
		{
			return null;
		}
		return value;
	}
}
