using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuestMissionMonsterArrange
{
	private CreatureFarmQuestMission m_mission;

	private int m_nLevel;

	private MonsterArrange m_monsterArrange;

	public CreatureFarmQuestMission mission => m_mission;

	public int level => m_nLevel;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public CreatureFarmQuestMissionMonsterArrange(CreatureFarmQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_mission = mission;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. no = " + m_mission.no + ", m_nLevel = " + m_nLevel);
		}
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
		if (m_monsterArrange == null)
		{
			SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. no = " + m_mission.no + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_mission.targetPosition, m_mission.targetRadius);
	}

	public float SelectRotationY()
	{
		return SFRandom.NextFloat(360f);
	}
}
public class CreatureFarmQuestMissionMonsterInstance : ContinentMonsterInstance
{
	private CreatureFarmQuestMissionMonsterArrange m_arrange;

	private Guid m_questHeroId = Guid.Empty;

	private string m_sQuestHeroName;

	private Guid m_questMissionInstanceId = Guid.Empty;

	private int m_questMissionNo;

	private DateTimeOffset m_spawnTime = DateTimeOffset.MinValue;

	private int m_nTargetAutoCompletionTime;

	private float m_fRemainingLifetime;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.CreatureFarmQuestMissionMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public override bool isExclusive => true;

	public override Guid exclusiveHeroId => m_questHeroId;

	public override string exclusiveHeroName => m_sQuestHeroName;

	public Guid questHeroId => m_questHeroId;

	public Guid questMissionInstanceId => m_questMissionInstanceId;

	public int questMissionNo => m_questMissionNo;

	public float remainingLifetime => m_fRemainingLifetime;

	public void Init(ContinentInstance continentInstance, CreatureFarmQuestMissionMonsterArrange arrange, Hero questHero, Guid questMissionInstanceId, int nQuestMissionNo, int nTargetAutoCompletionTime, DateTimeOffset currentTime)
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
		m_questMissionNo = nQuestMissionNo;
		m_nTargetAutoCompletionTime = nTargetAutoCompletionTime;
		m_fRemainingLifetime = GetRemainingLifetime(currentTime);
		InitMonsterInstance(continentInstance, m_arrange.SelectPosition(), m_arrange.SelectRotationY());
	}

	private float GetRemainingLifetime(DateTimeOffset time)
	{
		float fElapsedTime = (float)(time - m_spawnTime).TotalSeconds;
		return Math.Max((float)m_nTargetAutoCompletionTime - fElapsedTime, 0f);
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
			HeroCreatureFarmQuest quest = hero.creatureFarmQuest;
			if (quest != null && !(quest.instanceId != m_questMissionInstanceId))
			{
				CreatureFarmQuestMission mission = quest.mission;
				if (mission != null && mission.no == m_questMissionNo)
				{
					quest.CompleteMission(m_currentUpdateTime, bSendEvent: true);
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
			hero.ProcessCreatureFarmQuestMission_ExclusivMonster(this, time);
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDCreatureFarmQuestMissionMonsterInstance();
	}
}
