using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCharacter
{
	private int m_nId;

	private int m_nRequiredHeroLevel;

	private List<CreatureCharacterSkillPoolEntry> m_skillPool = new List<CreatureCharacterSkillPoolEntry>();

	private int m_nTotalSkillPickPoint;

	public int id => m_nId;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["creatureCharacterId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "크리처캐릭터 ID가 유효하지 않습니다.m_nId = " + m_nId);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
	}

	public void AddSkillPoolEntry(CreatureCharacterSkillPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_skillPool.Add(entry);
		m_nTotalSkillPickPoint += entry.point;
	}

	public CreatureCharacterSkillPoolEntry SelectSkill()
	{
		return Util.SelectPickEntry(m_skillPool, m_nTotalSkillPickPoint);
	}

	public List<CreatureCharacterSkillPoolEntry> SelectSkills(int nCount)
	{
		return Util.SelectPickEntries(m_skillPool, m_nTotalSkillPickPoint, nCount, bDuplicated: true);
	}
}
