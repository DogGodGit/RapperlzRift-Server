using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Creature
{
	private int m_nId;

	private CreatureCharacter m_character;

	private CreatureGrade m_grade;

	private int m_nMinQuality;

	private int m_nMaxQuality;

	private Dictionary<int, CreatureBaseAttrValue> m_baseAttrValue = new Dictionary<int, CreatureBaseAttrValue>();

	public int id => m_nId;

	public CreatureCharacter character => m_character;

	public CreatureGrade grade => m_grade;

	public int minQuality => m_nMinQuality;

	public int maxQuality => m_nMaxQuality;

	public Dictionary<int, CreatureBaseAttrValue> baseAttrs => m_baseAttrValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["creatureId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "크리처ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nCharacterId = Convert.ToInt32(dr["creatureCharacterId"]);
		m_character = Resource.instance.GetCreatureCharacter(nCharacterId);
		if (m_character == null)
		{
			SFLogUtil.Warn(GetType(), "크리처캐릭터가 존재하지 않습니다. m_nId = " + m_nId + ", nCharacterId = " + nCharacterId);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetCreatureGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. m_nId = " + m_nId + ", nGrade = " + nGrade);
		}
		m_nMinQuality = Convert.ToInt32(dr["minQuality"]);
		if (m_nMinQuality < 0)
		{
			SFLogUtil.Warn(GetType(), "최소품질이 유효하지 않습니다. m_nMinQuality = " + m_nMinQuality);
		}
		m_nMaxQuality = Convert.ToInt32(dr["maxQuality"]);
		if (m_nMaxQuality < m_nMinQuality)
		{
			SFLogUtil.Warn(GetType(), "최대품질이 유효하지 않습니다. m_nMaxQuality = " + m_nMaxQuality);
		}
	}

	public void AddBaseAttrValue(CreatureBaseAttrValue attrValue)
	{
		if (attrValue == null)
		{
			throw new ArgumentNullException("attrValue");
		}
		m_baseAttrValue.Add(attrValue.attr.attrId, attrValue);
	}

	public CreatureBaseAttrValue GetBaseAttrValue(int nAttrId)
	{
		if (!m_baseAttrValue.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}
}
