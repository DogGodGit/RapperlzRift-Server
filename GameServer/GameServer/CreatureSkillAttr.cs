using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureSkillAttr
{
	private CreatureSkill m_skill;

	private CreatureSkillGrade m_grade;

	private AttrValue m_attrValue;

	public CreatureSkill skill => m_skill;

	public CreatureSkillGrade grade => m_grade;

	public AttrValue attrValue => m_attrValue;

	public CreatureSkillAttr(CreatureSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_skill = skill;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nGrade = Convert.ToInt32(dr["skillGrade"]);
		m_grade = Resource.instance.GetCreatureSkillGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. nGrade = " + nGrade);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. nGrade = " + nGrade + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
