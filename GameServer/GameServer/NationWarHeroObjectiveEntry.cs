using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarHeroObjectiveEntry
{
	public const int kType_Win = 1;

	public const int kType_Lose = 2;

	public const int kType_KillCount = 3;

	public const int kType_ImmediateRevivalCount = 4;

	public const int kRewardType_ExploitPoint = 1;

	public const int kRewardType_OwnDia = 2;

	private NationWar m_nationWar;

	private int m_nNo;

	private int m_nType;

	private int m_nObjectiveCount;

	private int m_nRewardType;

	private OwnDiaReward m_ownDiaReward;

	private ExploitPointReward m_exploitPointReward;

	public NationWar naionWar => m_nationWar;

	public int no => m_nNo;

	public int type => m_nType;

	public int objectiveCount => m_nObjectiveCount;

	public int rewardType => m_nRewardType;

	public OwnDiaReward ownDiaReward => m_ownDiaReward;

	public int ownDiaRewardValue
	{
		get
		{
			if (m_ownDiaReward == null)
			{
				return 0;
			}
			return m_ownDiaReward.value;
		}
	}

	public ExploitPointReward exploitPointReward => m_exploitPointReward;

	public NationWarHeroObjectiveEntry(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		m_nObjectiveCount = Convert.ToInt32(dr["objectiveCount"]);
		if (m_nObjectiveCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nObjectiveCount = " + m_nObjectiveCount);
		}
		m_nRewardType = Convert.ToInt32(dr["rewardType"]);
		switch (m_nRewardType)
		{
		case 1:
		{
			long lnExploitPointRewardId = Convert.ToInt64(dr["exploitPointRewardId"]);
			if (lnExploitPointRewardId > 0)
			{
				m_exploitPointReward = Resource.instance.GetExploitPointReward(lnExploitPointRewardId);
				if (m_exploitPointReward == null)
				{
					SFLogUtil.Warn(GetType(), "공적점수보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
				}
			}
			else
			{
				SFLogUtil.Warn(GetType(), "공적점수보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
			}
			break;
		}
		case 2:
		{
			long lnOwnDiaRewardId = Convert.ToInt64(dr["ownDiaRewardId"]);
			if (lnOwnDiaRewardId > 0)
			{
				m_ownDiaReward = Resource.instance.GetOwnDiaReward(lnOwnDiaRewardId);
				if (m_ownDiaReward == null)
				{
					SFLogUtil.Warn(GetType(), "귀속다이아보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnOwnDiaRewardId = " + lnOwnDiaRewardId);
				}
			}
			else if (lnOwnDiaRewardId < 0)
			{
				SFLogUtil.Warn(GetType(), "귀속다이아보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnOwnDiaRewardId = " + lnOwnDiaRewardId);
			}
			break;
		}
		default:
			SFLogUtil.Warn(GetType(), "보상타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRewardType = " + m_nRewardType);
			break;
		}
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1 && nType != 2 && nType != 3)
		{
			return nType == 4;
		}
		return true;
	}
}
