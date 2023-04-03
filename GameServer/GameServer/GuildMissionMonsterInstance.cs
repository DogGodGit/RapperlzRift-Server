using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildMissionMonsterInstance : MonsterInstance
{
	private MonsterArrange m_arrange;

	private Guid m_missionHeroId = Guid.Empty;

	private string m_sMissionHeroName;

	private Guid m_questMissionInstanceId = Guid.Empty;

	private int m_nMonsterkillLimitTime;

	private DateTimeOffset m_spawnTime = DateTimeOffset.MinValue;

	private float m_fRemainingLifetime;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.GuildMissionQuestMonster;

	public override Monster monster => m_arrange.monster;

	public override bool isExclusive => true;

	public override Guid exclusiveHeroId => m_missionHeroId;

	public override string exclusiveHeroName => m_sMissionHeroName;

	public Guid missionHeroId => m_missionHeroId;

	public string missionHeroName => m_sMissionHeroName;

	public Guid questMissionInstanceId => m_questMissionInstanceId;

	public float remainingLifetime => m_fRemainingLifetime;

	public void Init(ContinentInstance continentInstance, MonsterArrange arrange, Vector3 position, int nMonsterKillLimitTime, Guid missionHeroId, string sMissionHeroName, Guid questMissionInstanceId, DateTimeOffset currentTime)
	{
		if (continentInstance == null)
		{
			throw new ArgumentNullException("continentInstance");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		m_spawnTime = currentTime;
		m_missionHeroId = missionHeroId;
		m_sMissionHeroName = sMissionHeroName;
		m_questMissionInstanceId = questMissionInstanceId;
		m_nMonsterkillLimitTime = nMonsterKillLimitTime;
		m_fRemainingLifetime = GetRemainingLifetime(currentTime);
		InitMonsterInstance(continentInstance, position, Util.SelectAngle());
	}

	public float GetRemainingLifetime(DateTimeOffset time)
	{
		TimeSpan elapsedTime = time - m_spawnTime;
		return (float)Math.Max((double)m_nMonsterkillLimitTime - elapsedTime.TotalSeconds, 0.0);
	}

	protected override void OnUpdateInternal()
	{
		base.OnUpdateInternal();
		OnUpdate_ManageLifetime();
	}

	private void OnUpdate_ManageLifetime()
	{
		try
		{
			ManageLifetime();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void ManageLifetime()
	{
		if (!(m_fRemainingLifetime <= 0f))
		{
			m_fRemainingLifetime = GetRemainingLifetime(m_currentUpdateTime);
			if (!(m_fRemainingLifetime > 0f))
			{
				AddWork(new SFAction(OnLifetimeEnded), bGlobalLockRequired: true);
			}
		}
	}

	private void OnLifetimeEnded()
	{
		m_currentPlace.RemoveMonster(this, bSendEvent: true);
		Hero hero = Cache.instance.GetLoggedInHero(m_missionHeroId);
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			HeroGuildMissionQuest quest = hero.guildMissionQuest;
			if (quest != null)
			{
				HeroGuildMissionQuestMission mission = quest.currentMission;
				if (mission != null && !(mission.id != m_questMissionInstanceId))
				{
					quest.FailCurrentMission(m_currentUpdateTime, bSendEvent: true);
				}
			}
		}
	}

	protected override void OnDeadAsync(DateTimeOffset time)
	{
		base.OnDeadAsync(time);
		Hero hero = Cache.instance.GetLoggedInHero(m_missionHeroId);
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			hero.ProcessGuildMissionForSummon(this, time);
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDGuildMissionQuestMonsterInstance();
	}
}
