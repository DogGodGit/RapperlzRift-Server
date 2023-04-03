using System;
using System.Collections.Generic;

namespace GameServer;

public class JobAbnormalState
{
	private AbnormalState m_abnormalState;

	private Job m_job;

	private List<AbnormalStateJobSkillLevel> m_abnormalStateJobSkillLevels = new List<AbnormalStateJobSkillLevel>();

	public AbnormalState abnormalState
	{
		get
		{
			return m_abnormalState;
		}
		set
		{
			m_abnormalState = value;
		}
	}

	public Job job => m_job;

	public JobAbnormalState(Job job)
	{
		if (job == null)
		{
			throw new ArgumentNullException("job");
		}
		m_job = job;
	}

	public void AddAbnormalStateJobSkillLevel(AbnormalStateJobSkillLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_abnormalStateJobSkillLevels.Add(level);
	}

	public AbnormalStateLevel GetAbnormalStateJobSkillLevel(int nlevel)
	{
		int nIndex = nlevel - 1;
		if (nIndex < 0 || nIndex >= m_abnormalStateJobSkillLevels.Count)
		{
			return null;
		}
		return m_abnormalStateJobSkillLevels[nIndex];
	}
}
