using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class JobChangeQuestMonsterInstance : MonsterInstance
{
	private MonsterArrange m_arrange;

	private Guid m_questHeroId = Guid.Empty;

	private string m_sQuestHeroName;

	private Guid m_questInstanceId = Guid.Empty;

	private DateTimeOffset m_spawnTime = DateTimeOffset.MinValue;

	private float m_fMonsterLifeTime;

	private float m_fRemainingLifetime;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.JobChangeQuestMonster;

	public override Monster monster => m_arrange.monster;

	public override bool isExclusive => true;

	public override Guid exclusiveHeroId => m_questHeroId;

	public override string exclusiveHeroName => m_sQuestHeroName;

	public Guid questHeroId => m_questHeroId;

	public Guid questInstanceId => m_questInstanceId;

	public float remainingLifetime => m_fRemainingLifetime;

	public void Init(Place placeInstance, MonsterArrange arrange, Vector3 selectedPosition, float fSeletedRotationY, Hero questHero, Guid questInstanceId, float fMonsterLifeTime, DateTimeOffset currentTime)
	{
		if (placeInstance == null)
		{
			throw new ArgumentNullException("placeInstance");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		m_spawnTime = currentTime;
		m_questHeroId = questHero.id;
		m_sQuestHeroName = questHero.name;
		m_questInstanceId = questInstanceId;
		m_fMonsterLifeTime = fMonsterLifeTime;
		m_fRemainingLifetime = GetRemainingLifetime(currentTime);
		InitMonsterInstance(placeInstance, selectedPosition, fSeletedRotationY);
	}

	private float GetRemainingLifetime(DateTimeOffset time)
	{
		float fElapsedTime = (float)(time - m_spawnTime).TotalSeconds;
		return Math.Max(m_fMonsterLifeTime - fElapsedTime, 0f);
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
			HeroJobChangeQuest quest = hero.jobChangeQuest;
			if (quest != null && !(quest.instanceId != m_questInstanceId))
			{
				quest.monsterInst = null;
				quest.Fail(bSendEvent: true, m_currentUpdateTime);
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
			hero.ProcessJobChangeQuestForExclusiveMonster(this, time);
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDJobChangeQuestMonsterInstance();
	}
}
