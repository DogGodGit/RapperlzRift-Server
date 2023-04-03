using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestMonsterInstance : ContinentMonsterInstance
{
	private TreatOfFarmQuestMonsterArrange m_arrange;

	private Guid m_questHeroId = Guid.Empty;

	private string m_sQuestHeroName;

	private Guid m_questMissionInstanceId = Guid.Empty;

	private DateTimeOffset m_spawnTime = DateTimeOffset.MinValue;

	private float m_fRemainingLifetime;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.TreatOfFarmQuestMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public override bool isExclusive => true;

	public override Guid exclusiveHeroId => m_questHeroId;

	public override string exclusiveHeroName => m_sQuestHeroName;

	public Guid questHeroId => m_questHeroId;

	public Guid questMissionInstanceId => m_questMissionInstanceId;

	public float remainingLifetime => m_fRemainingLifetime;

	public void Init(ContinentInstance continentInstance, TreatOfFarmQuestMonsterArrange arrange, Hero questHero, Guid questMissionInstanceId, DateTimeOffset currentTime)
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
		m_questHeroId = questHero.id;
		m_sQuestHeroName = questHero.name;
		m_questMissionInstanceId = questMissionInstanceId;
		m_fRemainingLifetime = GetRemainingLifetime(currentTime);
		InitMonsterInstance(continentInstance, m_arrange.SelectPosition(), m_arrange.SelectRotationY());
	}

	private float GetRemainingLifetime(DateTimeOffset time)
	{
		TimeSpan elapsedTime = time - m_spawnTime;
		int nLifetime = Resource.instance.treatOfFarmQuest.monsterKillLimitTime;
		return Math.Max((float)nLifetime - (float)elapsedTime.TotalSeconds, 0f);
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
		Hero hero = Cache.instance.GetLoggedInHero(m_questHeroId);
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			HeroTreatOfFarmQuest quest = hero.treatOfFarmQuest;
			if (quest != null)
			{
				HeroTreatOfFarmQuestMission mission = quest.currentMission;
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
		Hero hero = Cache.instance.GetLoggedInHero(m_questHeroId);
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			hero.ProcessTreatOfFarmQuestMission(this, time);
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDTreatOfFarmQuestMonsterInstance();
	}
}
