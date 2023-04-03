using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarMember
{
	private NationWarDeclaration m_declaration;

	private Guid m_heroId = Guid.Empty;

	private string m_sHeroName;

	private Guid m_guildId = Guid.Empty;

	private string m_sGuildName;

	private int m_nRanking;

	private bool m_bLuckyReward;

	private int m_nKillCount;

	private DateTimeOffset m_lastKillTime = DateTimeOffset.MinValue;

	private int m_nAssistCount;

	private int m_nDeadCount;

	private int m_nImmediateRevivalCount;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private bool m_bRewarded;

	private long m_lnRewardedExp;

	public NationWarDeclaration declaration => m_declaration;

	public Guid heroId => m_heroId;

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

	public Guid guildId
	{
		get
		{
			return m_guildId;
		}
		set
		{
			m_guildId = value;
		}
	}

	public string guildName
	{
		get
		{
			return m_sGuildName;
		}
		set
		{
			m_sGuildName = value;
		}
	}

	public bool luckyReward
	{
		get
		{
			return m_bLuckyReward;
		}
		set
		{
			m_bLuckyReward = value;
		}
	}

	public int killCount
	{
		get
		{
			return m_nKillCount;
		}
		set
		{
			m_nKillCount = value;
		}
	}

	public DateTimeOffset lastKillTime
	{
		get
		{
			return m_lastKillTime;
		}
		set
		{
			m_lastKillTime = value;
		}
	}

	public int assistCount
	{
		get
		{
			return m_nAssistCount;
		}
		set
		{
			m_nAssistCount = value;
		}
	}

	public int deadCount
	{
		get
		{
			return m_nDeadCount;
		}
		set
		{
			m_nDeadCount = value;
		}
	}

	public int immediateRevivalCount
	{
		get
		{
			return m_nImmediateRevivalCount;
		}
		set
		{
			m_nImmediateRevivalCount = value;
		}
	}

	public DateTimeOffset regTime => m_regTime;

	public bool rewarded
	{
		get
		{
			return m_bRewarded;
		}
		set
		{
			m_bRewarded = value;
		}
	}

	public long rewardedExp
	{
		get
		{
			return m_lnRewardedExp;
		}
		set
		{
			m_lnRewardedExp = value;
		}
	}

	public NationWarMember(NationWarDeclaration declaration, Guid heroId, string sHeroName)
		: this(declaration, heroId, sHeroName, DateTimeOffset.MinValue)
	{
	}

	public NationWarMember(NationWarDeclaration declaration, Guid heroId, string sHeroName, DateTimeOffset regTime)
	{
		m_declaration = declaration;
		m_heroId = heroId;
		m_sHeroName = sHeroName;
		m_regTime = regTime;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nKillCount = Convert.ToInt32(dr["killCount"]);
		m_nAssistCount = Convert.ToInt32(dr["assistcount"]);
		m_nDeadCount = Convert.ToInt32(dr["deadCount"]);
		m_nImmediateRevivalCount = Convert.ToInt32(dr["immediateRevivalCount"]);
		m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"]);
		m_bRewarded = Convert.ToBoolean(dr["rewarded"]);
	}

	public PDNationWarRanking ToPDNationWarRanking()
	{
		PDNationWarRanking inst = new PDNationWarRanking();
		inst.ranking = m_nRanking;
		inst.heroId = (Guid)m_heroId;
		inst.heroName = m_sHeroName;
		inst.killCount = m_nKillCount;
		inst.guildId = (Guid)m_guildId;
		inst.guildName = m_sGuildName;
		return inst;
	}

	public int CompareTo(NationWarMember other)
	{
		if (other == null)
		{
			return 1;
		}
		int nResult = m_nKillCount.CompareTo(other.killCount);
		if (nResult == 0)
		{
			return -m_lastKillTime.CompareTo(other.lastKillTime);
		}
		return nResult;
	}

	public static int Compare(NationWarMember x, NationWarMember y)
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
}
