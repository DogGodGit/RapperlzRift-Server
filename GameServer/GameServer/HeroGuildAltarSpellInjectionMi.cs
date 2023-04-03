using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroGuildAltarSpellInjectionMission
{
	private Hero m_hero;

	private Timer m_timer;

	public bool isRunning => m_timer != null;

	public HeroGuildAltarSpellInjectionMission(Hero hero)
	{
		m_hero = hero;
	}

	public void Start()
	{
		int nDuration = Resource.instance.guildAltar.spellInjectionDuration * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_timer = new Timer(OnTimerTick);
		m_timer.Change(nDuration, -1);
		if (m_hero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildAltarSpellInjectionMissionStarted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
		}
	}

	private void OnTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: true);
	}

	private void ProcessTimerTick()
	{
		if (isRunning)
		{
			DisposeTimer();
			Complete();
		}
	}

	private void Complete()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		GuildAltar altar = Resource.instance.guildAltar;
		m_hero.RefreshGuildMoralPoint(currentDate);
		int nRewardMoralPoint = altar.spellInjectionRewardMoralPoint;
		int nAddedMoralPoint = m_hero.AddGuildMoralPoint(nRewardMoralPoint);
		Guild guild = m_hero.guildMember.guild;
		guild.AddMoralPoint(currentDate, nAddedMoralPoint, m_hero.id);
		m_hero.RemoveGuildAltarSpellInjectionMission();
		Complete_SaveToDB();
		Complete_SaveToGameLogDB(nRewardMoralPoint, nAddedMoralPoint, currentTime);
		ServerEvent.SendGuildAltarSpellInjectionMissionCompleted(m_hero.account.peer, currentDate, m_hero.guildMoralPoint, guild.moralPoint);
		if (m_hero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildAltarSpellInjectionMissionCompleted(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
		}
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildAltarSpellInjectionLog(Guid.NewGuid(), guild.id, m_hero.id, nRewardMoralPoint, nAddedMoralPoint, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void Cancel(bool bSendEventToOwner, bool bSendEventToOthers)
	{
		if (isRunning)
		{
			DisposeTimer();
			if (bSendEventToOwner)
			{
				ServerEvent.SendGuildAltarSpellInjectionMissionCanceled(m_hero.account.peer);
			}
			if (bSendEventToOthers && m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroGuildAltarSpellInjectionMissionCanceled(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.id);
			}
		}
	}

	private void DisposeTimer()
	{
		if (m_timer != null)
		{
			m_timer.Dispose();
			m_timer = null;
		}
	}

	public void Release()
	{
		DisposeTimer();
	}
}
