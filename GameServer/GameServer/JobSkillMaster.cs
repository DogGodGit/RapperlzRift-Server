using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobSkillMaster
{
	private int m_nSkillId;

	private int m_nOpenRequiredMainQuestNo;

	private List<JobSkillLevelMaster> m_levels = new List<JobSkillLevelMaster>();

	public int skillId => m_nSkillId;

	public int openRequiredMainQuestNo => m_nOpenRequiredMainQuestNo;

	public List<JobSkillLevelMaster> levels => m_levels;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSkillId = Convert.ToInt32(dr["skillId"]);
		if (m_nSkillId <= 0)
		{
			SFLogUtil.Warn(GetType(), "스킬ID가 유효하지 않습니다. m_nSkillId = " + m_nSkillId);
		}
		m_nOpenRequiredMainQuestNo = Convert.ToInt32(dr["openRequiredMainQuestNo"]);
		if (m_nOpenRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요 메인퀘스트 번호가 유효하지 않습니다. m_nOpenRequiredMainQuestNo = " + m_nOpenRequiredMainQuestNo);
		}
	}

	public void AddLevel(JobSkillLevelMaster level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		if (level.skillMaster != null)
		{
			throw new Exception("이미 직업스킬레벨업레시피 컬렉션에 추가된 직업스킬레벨업레시피입니다.");
		}
		m_levels.Add(level);
		level.skillMaster = this;
	}

	public JobSkillLevelMaster GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}
}
