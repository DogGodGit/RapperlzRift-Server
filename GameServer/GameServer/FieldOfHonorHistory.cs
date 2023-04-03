using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorHistory
{
	public const int kRecodeDisplayCount = 3;

	public const int kType_Challenged = 1;

	public const int kType_BeChallenged = 2;

	private Hero m_hero;

	private Guid m_id = Guid.Empty;

	private int m_nType;

	private Guid m_targetHeroId = Guid.Empty;

	private string m_sTargetName;

	private int m_nOldRanking;

	private int m_nRanking;

	private bool m_bIsWin;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Hero hero => m_hero;

	public Guid id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public int type
	{
		get
		{
			return m_nType;
		}
		set
		{
			m_nType = value;
		}
	}

	public Guid targetHeroId
	{
		get
		{
			return m_targetHeroId;
		}
		set
		{
			m_targetHeroId = value;
		}
	}

	public string targetName
	{
		get
		{
			return m_sTargetName;
		}
		set
		{
			m_sTargetName = value;
		}
	}

	public int oldRanking
	{
		get
		{
			return m_nOldRanking;
		}
		set
		{
			m_nOldRanking = value;
		}
	}

	public int ranking
	{
		get
		{
			return m_nRanking;
		}
		set
		{
			m_nRanking = value;
		}
	}

	public bool isWin
	{
		get
		{
			return m_bIsWin;
		}
		set
		{
			m_bIsWin = value;
		}
	}

	public DateTimeOffset regTime
	{
		get
		{
			return m_regTime;
		}
		set
		{
			m_regTime = value;
		}
	}

	public FieldOfHonorHistory(Hero hero)
	{
		m_hero = hero;
	}

	public FieldOfHonorHistory(Hero hero, Guid id)
	{
		m_hero = hero;
		m_id = id;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = SFDBUtil.ToGuid(dr["historyId"]);
		m_nType = Convert.ToInt32(dr["type"]);
		m_targetHeroId = SFDBUtil.ToGuid(dr["targetHeroId"]);
		m_sTargetName = Convert.ToString(dr["targetName"]);
		m_nOldRanking = Convert.ToInt32(dr["oldRanking"]);
		m_nRanking = Convert.ToInt32(dr["ranking"]);
		m_bIsWin = Convert.ToBoolean(dr["isWin"]);
		m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"], DateTimeOffset.MinValue);
	}

	public int CompareTo(FieldOfHonorHistory other)
	{
		if (other == null)
		{
			return 1;
		}
		return m_regTime.CompareTo(other.regTime);
	}

	public PDFieldOfHonorHistory ToPDFieldOfHonorHistory()
	{
		PDFieldOfHonorHistory inst = new PDFieldOfHonorHistory();
		inst.isChallenged = m_nType == 1;
		inst.oldRanking = m_nOldRanking;
		inst.ranking = m_nRanking;
		inst.isWin = m_bIsWin;
		inst.targetHeroId = (Guid)m_targetHeroId;
		inst.targetHeroName = m_sTargetName;
		inst.regTime = (DateTimeOffset)m_regTime;
		return inst;
	}

	public static int Compare(FieldOfHonorHistory x, FieldOfHonorHistory y)
	{
		if (x == null)
		{
			if (y != null)
			{
				return -1;
			}
			return 0;
		}
		return x.CompareTo(y);
	}

	public static List<PDFieldOfHonorHistory> ToPDFieldOfHonorHistories(IEnumerable<FieldOfHonorHistory> histories)
	{
		List<PDFieldOfHonorHistory> insts = new List<PDFieldOfHonorHistory>();
		foreach (FieldOfHonorHistory history in histories)
		{
			insts.Add(history.ToPDFieldOfHonorHistory());
		}
		return insts;
	}
}
