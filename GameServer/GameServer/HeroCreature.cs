using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroCreature
{
	private Guid m_instanceId = Guid.Empty;

	private Hero m_hero;

	private Creature m_creature;

	private int m_nLevel;

	private int m_nAdditionalOpenSkillSlotCount;

	private int m_nExp;

	private int m_nInjectionLevel;

	private int m_nInjectionExp;

	private int m_nInjectionItemCount;

	private int m_nQuality;

	private bool m_bCheered;

	private Dictionary<int, HeroCreatureBaseAttr> m_baseAttrs = new Dictionary<int, HeroCreatureBaseAttr>();

	private List<HeroCreatureAdditionalAttr> m_additionalAttrs = new List<HeroCreatureAdditionalAttr>();

	private HeroCreatureSkill[] m_skills = new HeroCreatureSkill[Resource.instance.creatureSkillSlotMaxCount];

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	public Guid instanceId => m_instanceId;

	public Hero hero => m_hero;

	public Creature creature => m_creature;

	public int level => m_nLevel;

	public int additionalOpenSkillSlotCount
	{
		get
		{
			return m_nAdditionalOpenSkillSlotCount;
		}
		set
		{
			m_nAdditionalOpenSkillSlotCount = value;
		}
	}

	public int exp => m_nExp;

	public int injectionLevel => m_nInjectionLevel;

	public int injectionExp => m_nInjectionExp;

	public int injectionItemCount
	{
		get
		{
			return m_nInjectionItemCount;
		}
		set
		{
			m_nInjectionItemCount = value;
		}
	}

	public int quality
	{
		get
		{
			return m_nQuality;
		}
		set
		{
			m_nQuality = value;
		}
	}

	public bool cheered
	{
		get
		{
			return m_bCheered;
		}
		set
		{
			m_bCheered = value;
		}
	}

	public bool participated => m_instanceId == m_hero.participationCreatureId;

	public Dictionary<int, HeroCreatureBaseAttr> baseAttrs => m_baseAttrs;

	public List<HeroCreatureAdditionalAttr> additionalAttrs => m_additionalAttrs;

	public HeroCreatureSkill[] skills => m_skills;

	public int skillCount
	{
		get
		{
			int nCount = 0;
			HeroCreatureSkill[] array = m_skills;
			foreach (HeroCreatureSkill skill in array)
			{
				if (skill.isOpened && skill.skillAttr != null)
				{
					nCount++;
				}
			}
			return nCount;
		}
	}

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public HeroCreature(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		for (int i = 0; i < m_skills.Length; i++)
		{
			m_skills[i] = new HeroCreatureSkill(this, i);
		}
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_instanceId = (Guid)dr["heroCreatureId"];
		int nCreautreId = Convert.ToInt32(dr["creatureId"]);
		m_creature = Resource.instance.GetCreature(nCreautreId);
		if (m_creature == null)
		{
			throw new Exception(string.Concat("크리처가 존재하지 않습니다. m_instanceId = ", m_instanceId, ", nCreautreId = ", nCreautreId));
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_nAdditionalOpenSkillSlotCount = Convert.ToInt32(dr["additionalOpenSkillSlotCount"]);
		m_nExp = Convert.ToInt32(dr["exp"]);
		m_nInjectionLevel = Convert.ToInt32(dr["injectionLevel"]);
		m_nInjectionExp = Convert.ToInt32(dr["injectionExp"]);
		m_nInjectionItemCount = Convert.ToInt32(dr["injectionItemCount"]);
		m_nQuality = Convert.ToInt32(dr["quality"]);
		m_bCheered = Convert.ToBoolean(dr["cheered"]);
	}

	public void Init(Creature creature)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_instanceId = Guid.NewGuid();
		m_creature = creature;
		m_nLevel = 1;
		m_nAdditionalOpenSkillSlotCount = 0;
		m_nExp = 0;
		m_nInjectionLevel = 1;
		m_nInjectionExp = 0;
		m_bCheered = false;
		m_nQuality = SFRandom.Next(m_creature.minQuality, m_creature.maxQuality + 1);
		foreach (CreatureBaseAttrValue attr2 in m_creature.baseAttrs.Values)
		{
			int nValue = SFRandom.Next(attr2.minAttrValue, attr2.maxAttrValue + 1);
			HeroCreatureBaseAttr baseAttr = new HeroCreatureBaseAttr(this, attr2, nValue);
			AddBaseAttr(baseAttr);
		}
		int nAttrNo = 1;
		foreach (CreatureAdditionalAttr attr in Resource.instance.SelectCreatureAdditionalAttrs())
		{
			HeroCreatureAdditionalAttr additionalAttr = new HeroCreatureAdditionalAttr(this, nAttrNo++, attr);
			AddAdditionalAttr(additionalAttr);
		}
		CreatureCharacter character = m_creature.character;
		CreatureSkillCountPoolEntry skillCountEntry = Resource.instance.SelectCreatureSkillCountPoolEntry();
		for (int i = 0; i < skillCountEntry.skillCount; i++)
		{
			CreatureCharacterSkillPoolEntry entry = character.SelectSkill();
			CreatureSkillGrade grade = Resource.instance.SelectCreatureSkillGrade();
			CreatureSkillAttr skillAttr = entry.skill.GetAttr(grade.grade);
			HeroCreatureSkill creatureSkill = GetSkill(i);
			creatureSkill.skillAttr = skillAttr;
		}
	}

	public void AddBaseAttr(HeroCreatureBaseAttr baseAttr)
	{
		if (baseAttr == null)
		{
			throw new ArgumentNullException("baseAttr");
		}
		m_baseAttrs.Add(baseAttr.attr.attr.attrId, baseAttr);
	}

	public HeroCreatureBaseAttr GetBaseAttr(int nAttrId)
	{
		if (!m_baseAttrs.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveBaseAttrs()
	{
		m_baseAttrs.Clear();
	}

	public void AddAdditionalAttr(HeroCreatureAdditionalAttr attr)
	{
		if (m_additionalAttrs == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_additionalAttrs.Add(attr);
	}

	public void RemoveAdditionAttrs()
	{
		m_additionalAttrs.Clear();
	}

	public List<int> GetAdditionalAttrIds()
	{
		List<int> additionalAttrIds = new List<int>();
		foreach (HeroCreatureAdditionalAttr attr in m_additionalAttrs)
		{
			additionalAttrIds.Add(attr.attr.attrId);
		}
		return additionalAttrIds;
	}

	public void AddSkill(HeroCreatureSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_skills[skill.slotIndex] = skill;
	}

	public HeroCreatureSkill GetSkill(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_skills.Length)
		{
			return null;
		}
		return m_skills[nIndex];
	}

	public List<PDHeroCreatureSkill> GetPDHeroCreatureSkills()
	{
		List<PDHeroCreatureSkill> insts = new List<PDHeroCreatureSkill>();
		HeroCreatureSkill[] array = m_skills;
		foreach (HeroCreatureSkill skill in array)
		{
			if (skill.isOpened && skill.skillAttr != null)
			{
				insts.Add(skill.ToPDHeroCreatureSkill());
			}
		}
		return insts;
	}

	public void AddExp(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		m_nExp += nAmount;
		CreatureLevel creatureLevel = Resource.instance.GetCreatureLevel(m_nLevel);
		int nNextLevelUpRequiredExp = 0;
		do
		{
			nNextLevelUpRequiredExp = creatureLevel.nextLevelUpRequiredExp;
			if (m_nExp < nNextLevelUpRequiredExp)
			{
				break;
			}
			m_nExp -= nNextLevelUpRequiredExp;
			m_nLevel++;
			creatureLevel = Resource.instance.GetCreatureLevel(m_nLevel);
		}
		while (!creatureLevel.isLastLevel);
		if (creatureLevel.isLastLevel)
		{
			m_nExp = 0;
		}
	}

	public long GetAccumulationExp()
	{
		long lnAccumulationExp = 0L;
		for (int nLevel = 1; nLevel < m_nLevel; nLevel++)
		{
			CreatureLevel creatureLevel = Resource.instance.GetCreatureLevel(nLevel);
			lnAccumulationExp += creatureLevel.nextLevelUpRequiredExp;
		}
		return lnAccumulationExp + m_nExp;
	}

	public void AddInjectionExp(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		m_nInjectionExp += nAmount;
		CreatureInjectionLevel injectionLevel = Resource.instance.GetCreatureInjectionLevel(m_nInjectionLevel);
		CreatureLevel creatureLevel = Resource.instance.GetCreatureLevel(m_nLevel);
		int nNextLevelUpRequiredExp = 0;
		do
		{
			nNextLevelUpRequiredExp = injectionLevel.nextLevelUpRequiredExp;
			if (m_nInjectionExp < nNextLevelUpRequiredExp)
			{
				break;
			}
			m_nInjectionExp -= nNextLevelUpRequiredExp;
			m_nInjectionLevel++;
			injectionLevel = Resource.instance.GetCreatureInjectionLevel(m_nInjectionLevel);
		}
		while (m_nInjectionLevel < creatureLevel.maxInjectionLevel);
		if (m_nInjectionLevel >= creatureLevel.maxInjectionLevel)
		{
			m_nInjectionExp = 0;
		}
	}

	public long GetAccumulationInjectionExp()
	{
		long lnAccumulationInjectionExp = 0L;
		for (int nLevel = 1; nLevel < m_nInjectionLevel; nLevel++)
		{
			CreatureInjectionLevel creatureInjectionLevel = Resource.instance.GetCreatureInjectionLevel(m_nInjectionLevel);
			lnAccumulationInjectionExp += creatureInjectionLevel.nextLevelUpRequiredExp;
		}
		return lnAccumulationInjectionExp + m_nInjectionExp;
	}

	public void RetrievalInjecitonLevel()
	{
		m_nInjectionLevel = 1;
		m_nInjectionExp = 0;
		m_nInjectionItemCount = 0;
	}

	public PDHeroCreature ToPDHeroCreature()
	{
		PDHeroCreature inst = new PDHeroCreature();
		inst.instanceId = (Guid)m_instanceId;
		inst.creatureId = m_creature.id;
		inst.level = m_nLevel;
		inst.additionalOpenSkillSlotCount = m_nAdditionalOpenSkillSlotCount;
		inst.exp = m_nExp;
		inst.injectionLevel = m_nInjectionLevel;
		inst.injectionExp = m_nInjectionExp;
		inst.injectionItemCount = m_nInjectionItemCount;
		inst.quality = m_nQuality;
		inst.cheered = m_bCheered;
		inst.baseAttrs = HeroCreatureBaseAttr.ToPDHeroCreatureBaseAttrs(m_baseAttrs.Values).ToArray();
		inst.additionalAttrIds = GetAdditionalAttrIds().ToArray();
		inst.skills = GetPDHeroCreatureSkills().ToArray();
		return inst;
	}

	public static List<PDHeroCreature> ToPDHeroCreatures(IEnumerable<HeroCreature> creatures)
	{
		List<PDHeroCreature> insts = new List<PDHeroCreature>();
		foreach (HeroCreature creature in creatures)
		{
			insts.Add(creature.ToPDHeroCreature());
		}
		return insts;
	}
}
