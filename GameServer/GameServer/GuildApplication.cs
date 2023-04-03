using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildApplication
{
	private Guid m_id = Guid.Empty;

	private Guild m_guild;

	private Hero m_hero;

	private Guid m_heroId = Guid.Empty;

	private string m_sHeroName;

	private int m_nHeroJobId;

	private int m_nHeroLevel;

	private int m_nHeroVipLevel;

	private long m_lnHeroBattlePower;

	private DateTimeOffset m_heroLogoutTime = DateTimeOffset.MinValue;

	public Guid id => m_id;

	public Guild guild => m_guild;

	public Hero hero => m_hero;

	public bool isLoggedIn => m_hero != null;

	public Guid heroId => m_heroId;

	public string heroName => m_sHeroName;

	public int heroJobId => m_nHeroJobId;

	public int heroLevel => m_nHeroLevel;

	public int heroVipLevel => m_nHeroVipLevel;

	public long heroBattlePower => m_lnHeroBattlePower;

	public DateTimeOffset heroLogoutTime => m_heroLogoutTime;

	public GuildApplication(Guild guild)
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
		m_id = (Guid)dr["applicationId"];
		m_heroId = (Guid)dr["heroId"];
		m_sHeroName = Convert.ToString(dr["name"]);
		m_nHeroJobId = Convert.ToInt32(dr["jobId"]);
		m_nHeroLevel = Convert.ToInt32(dr["level"]);
		int nHeroVipPoint = Convert.ToInt32(dr["vipPoint"]);
		int nAccountVipPoint = Convert.ToInt32(dr["accountVipPoint"]);
		m_nHeroVipLevel = Resource.instance.GetVipLevelByPoint(nHeroVipPoint + nAccountVipPoint).level;
		m_lnHeroBattlePower = Convert.ToInt64(dr["battlePower"]);
		m_heroLogoutTime = SFDBUtil.ToDateTimeOffset(dr["lastLogoutTime"]);
	}

	public void Init(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_heroId = hero.id;
		m_sHeroName = hero.name;
		m_nHeroJobId = hero.jobId;
		m_nHeroLevel = hero.level;
		m_nHeroVipLevel = hero.vipLevel.level;
		m_lnHeroBattlePower = hero.battlePower;
	}

	private void RefreshMemberInfo()
	{
		if (m_hero == null)
		{
			return;
		}
		lock (m_hero.syncObject)
		{
			m_nHeroJobId = m_hero.jobId;
			m_nHeroLevel = m_hero.level;
			m_nHeroVipLevel = m_hero.vipLevel.level;
			m_lnHeroBattlePower = m_hero.battlePower;
			m_heroLogoutTime = (m_hero.lastLogoutTime.HasValue ? m_hero.lastLogoutTime.Value : DateTimeOffset.MinValue);
		}
	}

	public void LogInHero(Hero hero)
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
		RefreshMemberInfo();
	}

	public void LogOutHero()
	{
		if (isLoggedIn)
		{
			RefreshMemberInfo();
			m_hero = null;
		}
	}

	public PDGuildApplication ToPDGuildApplication()
	{
		RefreshMemberInfo();
		PDGuildApplication inst = new PDGuildApplication();
		inst.id = (Guid)m_id;
		inst.heroId = (Guid)m_heroId;
		inst.heroName = m_sHeroName;
		inst.heroJobId = m_nHeroJobId;
		inst.heroLevel = m_nHeroLevel;
		inst.heroVipLevel = m_nHeroVipLevel;
		inst.heroBattlePower = m_lnHeroBattlePower;
		return inst;
	}

	public PDHeroGuildApplication ToPDHeroGuildApplication()
	{
		PDHeroGuildApplication inst = new PDHeroGuildApplication();
		inst.id = (Guid)m_id;
		inst.guildId = (Guid)m_guild.id;
		inst.guildName = m_guild.name;
		return inst;
	}

	public static List<PDGuildApplication> GetPDGuildApplications(IEnumerable<GuildApplication> applications)
	{
		List<PDGuildApplication> insts = new List<PDGuildApplication>();
		foreach (GuildApplication application in applications)
		{
			insts.Add(application.ToPDGuildApplication());
		}
		return insts;
	}

	public static List<PDHeroGuildApplication> GetPDMyGuildApplications(IEnumerable<GuildApplication> applications)
	{
		List<PDHeroGuildApplication> insts = new List<PDHeroGuildApplication>();
		foreach (GuildApplication application in applications)
		{
			insts.Add(application.ToPDHeroGuildApplication());
		}
		return insts;
	}
}
