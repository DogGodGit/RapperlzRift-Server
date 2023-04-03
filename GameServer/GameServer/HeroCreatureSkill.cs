using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroCreatureSkill
{
	private HeroCreature m_creature;

	private int m_nSlotIndex;

	private CreatureSkillAttr m_skillAttr;

	public HeroCreature creature => m_creature;

	public int slotIndex => m_nSlotIndex;

	public CreatureSkillAttr skillAttr
	{
		get
		{
			return m_skillAttr;
		}
		set
		{
			m_skillAttr = value;
		}
	}

	public bool isOpened => m_nSlotIndex < Resource.instance.creatureSkillSlotBaseOpenCount + m_creature.additionalOpenSkillSlotCount;

	public HeroCreatureSkill(HeroCreature creature)
		: this(creature, 0)
	{
	}

	public HeroCreatureSkill(HeroCreature creature, int nIndex)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_creature = creature;
		m_nSlotIndex = nIndex;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
		int nSkillId = Convert.ToInt32(dr["skillId"]);
		CreatureSkill m_skill = Resource.instance.GetCreatureSkill(nSkillId);
		if (m_skill == null)
		{
			throw new Exception("존재하지 않는 크리처스킬입니다. m_nSlotIndex = " + m_nSlotIndex + ", nSkillId = " + nSkillId);
		}
		int nGrade = Convert.ToInt32(dr["skillGrade"]);
		m_skillAttr = m_skill.GetAttr(nGrade);
		if (m_skillAttr == null)
		{
			throw new Exception("존재하지 않는크리처스킬속성입니다. nSkillId = " + nSkillId + ", nGrade = " + nGrade);
		}
	}

	public PDHeroCreatureSkill ToPDHeroCreatureSkill()
	{
		PDHeroCreatureSkill inst = new PDHeroCreatureSkill();
		inst.slotIndex = m_nSlotIndex;
		inst.skillId = m_skillAttr.skill.id;
		inst.grade = m_skillAttr.grade.grade;
		return inst;
	}
}
