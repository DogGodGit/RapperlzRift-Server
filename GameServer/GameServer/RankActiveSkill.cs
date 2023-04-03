using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class RankActiveSkill
{
	public const float kCastRangeFactor = 1.1f;

	public const float kCoolTimeFactor = 0.9f;

	public const int kType_Other = 1;

	public const int kType_Myself = 2;

	private int m_nId;

	private Rank m_requiredRank;

	private float m_fCoolTime;

	private int m_nType;

	private float m_fCastRange;

	private AbnormalState m_abnormalState;

	private List<RankActiveSkillLevel> m_levels = new List<RankActiveSkillLevel>();

	public int id => m_nId;

	public Rank requiredRank => m_requiredRank;

	public float coolTime => m_fCoolTime;

	public int type => m_nType;

	public float castRange => m_fCastRange;

	public AbnormalState abnormalState => m_abnormalState;

	public RankActiveSkillLevel maxLevel => m_levels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nId = Convert.ToInt32(dr["skillId"]);
		int nRequiredRankNo = Convert.ToInt32(dr["requiredRankNo"]);
		if (nRequiredRankNo > 0)
		{
			m_requiredRank = res.GetRank(nRequiredRankNo);
			if (m_requiredRank == null)
			{
				SFLogUtil.Warn(GetType(), "필요계급이 존재하지 않습니다. m_nId = " + m_nId + ", nRequiredRankNo = " + nRequiredRankNo);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "필요계급번호가 유효하지 않습니다. m_nId = " + m_nId + ", nRequiredRankNo = " + nRequiredRankNo);
		}
		m_fCoolTime = Convert.ToSingle(dr["coolTime"]);
		if (m_fCoolTime < 0f)
		{
			SFLogUtil.Warn(GetType(), "쿨타임이 유효하지 않습니다. m_nId = " + m_nId + ", m_fCoolTime = " + m_fCoolTime);
		}
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nType = " + m_nType);
		}
		m_fCastRange = Convert.ToSingle(dr["castRange"]);
		if (m_nType == 1 && m_fCastRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "시전거리가 유효하지 않습니다. m_nId = " + m_nId + ", m_fCastRange = " + m_fCastRange);
		}
		int nAbnormalStateId = Convert.ToInt32(dr["abnormalStateId"]);
		if (nAbnormalStateId > 0)
		{
			m_abnormalState = res.GetAbnormalState(nAbnormalStateId);
			if (m_abnormalState == null)
			{
				SFLogUtil.Warn(GetType(), "상태이상이 존재하지 않습니다. nAbnormalStateId = " + nAbnormalStateId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "상태이상ID가 유효하지 않습니다. m_nId = " + m_nId + ", nAbnormalStateId = " + nAbnormalStateId);
		}
	}

	public void AddLevel(RankActiveSkillLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
	}

	public RankActiveSkillLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
