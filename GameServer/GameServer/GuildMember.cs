using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildMember
{
	private Guild m_guild;

	private Hero m_hero;

	private GuildMemberGrade m_memberGrade;

	private int m_nTotalContributionPoint;

	private int m_nContributionPoint;

	private int m_nGuildPoint;

	private Guid m_id = Guid.Empty;

	private string m_sName;

	private int m_nJobId;

	private int m_nLevel;

	private int m_nVipLevel;

	private DateTimeOffset m_logoutTime = DateTimeOffset.MinValue;

	public Guild guild => m_guild;

	public Hero hero => m_hero;

	public bool isLoggedIn => m_hero != null;

	public GuildMemberGrade grade
	{
		get
		{
			return m_memberGrade;
		}
		set
		{
			m_memberGrade = value;
		}
	}

	public bool isMaster => m_memberGrade.id == 1;

	public bool isViceMaster => m_memberGrade.id == 2;

	public bool isLord => m_memberGrade.id == 3;

	public bool isNormal => m_memberGrade.id == 4;

	public int totalContributionPoint
	{
		get
		{
			return m_nTotalContributionPoint;
		}
		set
		{
			m_nTotalContributionPoint = value;
		}
	}

	public int contributionPoint
	{
		get
		{
			return m_nContributionPoint;
		}
		set
		{
			m_nContributionPoint = value;
		}
	}

	public int guildPoint => m_nGuildPoint;

	public Guid id => m_id;

	public string name => m_sName;

	public int jobId => m_nJobId;

	public GuildMember(Guild guild)
	{
		if (guild == null)
		{
			throw new ArgumentNullException("guild");
		}
		m_guild = guild;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nMemberGrade = Convert.ToInt32(dr["guildMemberGrade"]);
		m_memberGrade = Resource.instance.GetGuildMemberGrade(nMemberGrade);
		m_nTotalContributionPoint = Convert.ToInt32(dr["guildTotalContributionPoint"]);
		m_nContributionPoint = Convert.ToInt32(dr["guildContributionPoint"]);
		m_nGuildPoint = Convert.ToInt32(dr["guildPoint"]);
		m_id = (Guid)dr["heroId"];
		m_sName = Convert.ToString(dr["name"]);
		m_nJobId = Convert.ToInt32(dr["jobId"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		int nHeroVipPoint = Convert.ToInt32(dr["vipPoint"]);
		int nAccountVipPoint = Convert.ToInt32(dr["accountVipPoint"]);
		m_nVipLevel = Resource.instance.GetVipLevelByPoint(nHeroVipPoint + nAccountVipPoint).level;
		m_logoutTime = SFDBUtil.ToDateTimeOffset(dr["lastLogoutTime"], DateTimeOffset.MinValue);
	}

	public void Init(int nMemberGrade, Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_hero.guildMember = this;
		m_memberGrade = Resource.instance.GetGuildMemberGrade(nMemberGrade);
		m_id = hero.id;
		m_sName = hero.name;
	}

	public void Init(int nMemberGrade, Guid heroId, string sName, int nJobId, int nLevel, int nVipLevel, DateTimeOffset logoutTime)
	{
		m_memberGrade = Resource.instance.GetGuildMemberGrade(nMemberGrade);
		m_id = heroId;
		m_sName = sName;
		m_nJobId = nJobId;
		m_nLevel = nLevel;
		m_nVipLevel = nVipLevel;
		m_logoutTime = logoutTime;
	}

	public void LogIn(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (isLoggedIn)
		{
			throw new InvalidOperationException("이미 로그인중입니다.");
		}
		m_hero = hero;
		m_hero.guildMember = this;
	}

	public void LogOut()
	{
		if (isLoggedIn)
		{
			m_nJobId = m_hero.jobId;
			m_nLevel = m_hero.level;
			m_nVipLevel = m_hero.vipLevel.level;
			m_logoutTime = (m_hero.lastLoginTime.HasValue ? m_hero.lastLoginTime.Value : DateTimeOffset.MinValue);
			m_hero.guildMember = null;
			m_hero = null;
		}
	}

	public void RemoveHero()
	{
		m_hero = null;
	}

	public float GetLogoutElapsedTime(DateTimeOffset time)
	{
		if (isLoggedIn)
		{
			return 0f;
		}
		return (float)(time - m_logoutTime).TotalSeconds;
	}

	public PDGuildMember ToPDGuildMember(DateTimeOffset time)
	{
		PDGuildMember inst = new PDGuildMember();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		if (m_hero != null)
		{
			lock (m_hero.syncObject)
			{
				inst.jobId = m_hero.jobId;
				inst.level = m_hero.level;
				inst.vipLevel = m_hero.vipLevel.level;
				inst.totalContributionPoint = m_nTotalContributionPoint;
				inst.memberGrade = m_memberGrade.id;
			}
		}
		else
		{
			inst.jobId = m_nJobId;
			inst.level = m_nLevel;
			inst.vipLevel = m_nVipLevel;
			inst.totalContributionPoint = m_nTotalContributionPoint;
			inst.memberGrade = m_memberGrade.id;
		}
		inst.isLoggedIn = isLoggedIn;
		inst.logoutElapsedTime = GetLogoutElapsedTime(time);
		return inst;
	}

	public static List<PDGuildMember> GetPDGuildMembers(IEnumerable<GuildMember> members, DateTimeOffset time)
	{
		List<PDGuildMember> insts = new List<PDGuildMember>();
		foreach (GuildMember member in members)
		{
			insts.Add(member.ToPDGuildMember(time));
		}
		return insts;
	}
}
