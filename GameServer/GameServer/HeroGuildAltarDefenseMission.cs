using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroGuildAltarDefenseMission
{
	private Hero m_hero;

	private GuildAltarMonsterInstance m_monsterInst;

	private Timer m_limitTimeTimer;

	public Hero hero => m_hero;

	public bool isRunning => m_limitTimeTimer != null;

	public HeroGuildAltarDefenseMission(Hero hero, GuildAltarMonsterInstance monsterInst)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		m_hero = hero;
		m_monsterInst = monsterInst;
	}

	public void Start()
	{
		int nDuration = Resource.instance.guildAltar.defenseLimitTime * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
		m_limitTimeTimer.Change(nDuration, -1);
	}

	private void OnLimitTimeTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessLimitTimeTimerTick), bGlobalLockRequired: true);
	}

	private void ProcessLimitTimeTimerTick()
	{
		if (isRunning)
		{
			DisposeLimitTimeTimer();
			Fail(bSendEventToMySelf: true);
		}
	}

	public void Fail(bool bSendEventToMySelf)
	{
		Place monsterPlace = m_monsterInst.currentPlace;
		lock (monsterPlace.syncObject)
		{
			monsterPlace.RemoveMonster(m_monsterInst, bSendEvent: true);
		}
		m_hero.RemoveGuildAltarDefenseMission();
		if (bSendEventToMySelf)
		{
			ServerEvent.SendGuildAltarDefenseMissionFailed(m_hero.account.peer);
		}
	}

	public void Complete()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		GuildAltar altar = Resource.instance.guildAltar;
		m_hero.RefreshGuildMoralPoint(currentDate);
		int nRewardMoralPoint = altar.defenseRewardMoralPoint;
		int nAddedMoralPoint = m_hero.AddGuildMoralPoint(nRewardMoralPoint);
		Guild guild = m_hero.guildMember.guild;
		guild.AddMoralPoint(currentDate, nAddedMoralPoint, m_hero.id);
		m_hero.RemoveGuildAltarDefenseMission();
		Complete_SaveToDB();
		Complete_SaveToGameLogDB(nRewardMoralPoint, nAddedMoralPoint, currentTime);
		ServerEvent.SendGuildAltarDefenseMissionCompleted(m_hero.account.peer, currentDate, m_hero.guildMoralPoint, guild.moralPoint);
		if (m_hero.guildMoralPoint >= altar.dailyHeroMaxMoralPoint)
		{
			m_hero.CompleteGuildAltar(currentTime);
			m_hero.ProcessTodayTask(21, currentDate);
			guild.CompleteGuildDailyObjective(currentDate, 3, m_hero.guildMember);
		}
	}

	private void Complete_SaveToDB()
	{
		Guild guild = m_hero.guildMember.guild;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_MoralPoint(guild.id, guild.moralPointDate, guild.moralPoint));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildMoralPoint(m_hero.id, m_hero.guildMoralPointDate, m_hero.guildMoralPoint));
		dbWork.Schedule();
	}

	private void Complete_SaveToGameLogDB(int nRewardMoralPoint, int nAddedMoralPoint, DateTimeOffset time)
	{
		try
		{
			Guild guild = m_hero.guildMember.guild;
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildAltarDefenseLog(Guid.NewGuid(), guild.id, m_hero.id, m_monsterInst.missionHeroLevel, guild.level, nRewardMoralPoint, nAddedMoralPoint, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void DisposeLimitTimeTimer()
	{
		if (m_limitTimeTimer != null)
		{
			m_limitTimeTimer.Dispose();
			m_limitTimeTimer = null;
		}
	}

	public void Release()
	{
		DisposeLimitTimeTimer();
	}
}
