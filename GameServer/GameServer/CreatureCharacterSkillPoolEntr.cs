using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCharacterSkillPoolEntry : IPickEntry
{
	private CreatureCharacter m_character;

	private int m_nNo;

	private int m_nPoint;

	private CreatureSkill m_skill;

	public CreatureCharacter character => m_character;

	public int no => m_nNo;

	public int point => m_nPoint;

	public CreatureSkill skill => m_skill;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		int nSkillId = Convert.ToInt32(dr["skillId"]);
		m_skill = Resource.instance.GetCreatureSkill(nSkillId);
		if (m_skill == null)
		{
			SFLogUtil.Warn(GetType(), "크리처스킬이 존재하지 않습니다. m_nNo = " + m_nNo + ", nSkillId = " + nSkillId);
		}
	}
}
