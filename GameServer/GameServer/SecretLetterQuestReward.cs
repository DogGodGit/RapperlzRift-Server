using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SecretLetterQuestReward
{
	private int m_nGrade;

	private int m_nLevel;

	private ExpReward m_expReward;

	public int grade => m_nGrade;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public long expRewardValue
	{
		get
		{
			if (m_expReward == null)
			{
				return 0L;
			}
			return m_expReward.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (!MysteryBoxGrade.IsDefined(m_nGrade))
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nGrade = " + m_nGrade);
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nGrade = " + m_nGrade + ", m_nLevel = " + m_nLevel);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
		if (m_expReward == null)
		{
			SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nGrade = " + m_nGrade + ", m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
		}
	}
}
