using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MysteryBoxGrade
{
	public const int kId_Low = 1;

	public const int kId_Normal = 2;

	public const int kId_Rare = 3;

	public const int kId_Epic = 4;

	public const int kId_Legend = 5;

	public const int kCount = 5;

	private int m_nId;

	private ExploitPointReward m_exploitPointReward;

	public int id => m_nId;

	public ExploitPointReward exploitPointReward => m_exploitPointReward;

	public int exploitPointRewardValue
	{
		get
		{
			if (m_exploitPointReward == null)
			{
				return 0;
			}
			return m_exploitPointReward.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["grade"]);
		if (!IsDefined(m_nId))
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. grade = " + m_nId);
		}
		long lnExploitPointRewardId = Convert.ToInt64(dr["exploitPointRewardId"]);
		m_exploitPointReward = Resource.instance.GetExploitPointReward(lnExploitPointRewardId);
		if (m_exploitPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "공적포인트보상이 존재하지 않습니다. grade = " + m_nId + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
		}
	}

	public static bool IsDefined(int nValue)
	{
		if (nValue >= 1)
		{
			return nValue <= 5;
		}
		return false;
	}
}
