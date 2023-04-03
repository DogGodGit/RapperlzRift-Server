using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Job
{
	private int m_nId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private int m_nMoveSpeed;

	private int m_nWalkMoveSpeed;

	private OffenseType m_offenseType;

	private int m_nElementalId;

	private float m_fRadius;

	private int m_nParentJobId;

	private List<JobSkill> m_skills = new List<JobSkill>();

	private List<JobLevel> m_levels = new List<JobLevel>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public int moveSpeed => m_nMoveSpeed;

	public int walkMoveSpeed => m_nWalkMoveSpeed;

	public OffenseType offenseType => m_offenseType;

	public int elementalId => m_nElementalId;

	public float radius => m_fRadius;

	public int parentJobId => m_nParentJobId;

	public int baseJobId
	{
		get
		{
			if (m_nParentJobId == 0)
			{
				return m_nId;
			}
			return m_nParentJobId;
		}
	}

	public List<JobSkill> skills => m_skills;

	public List<JobLevel> levels => m_levels;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["jobId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_nMoveSpeed = Convert.ToInt32(dr["moveSpeed"]);
		m_nWalkMoveSpeed = Convert.ToInt32(dr["walkMoveSpeed"]);
		m_offenseType = (OffenseType)Convert.ToInt32(dr["offenseType"]);
		m_nElementalId = Convert.ToInt32(dr["elementalId"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반경이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_nParentJobId = Convert.ToInt32(dr["parentJobId"]);
	}

	public void AddSkill(JobSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		if (skill.job != null)
		{
			throw new Exception("이미 직업에 추가된 스킬 입니다.");
		}
		m_skills.Add(skill);
		skill.job = this;
	}

	public JobSkill GetSkill(int nSkillId)
	{
		int nIndex = nSkillId - 1;
		if (nIndex < 0 || nIndex >= m_skills.Count)
		{
			return null;
		}
		return m_skills[nIndex];
	}

	public void AddLevel(JobLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		if (level.job != null)
		{
			throw new Exception("이미 직업에 추가된 직업레벨입니다.");
		}
		m_levels.Add(level);
		level.job = this;
	}

	public JobLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}
}
