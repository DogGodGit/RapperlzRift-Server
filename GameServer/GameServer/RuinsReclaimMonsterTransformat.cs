using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimMonsterTransformationCancelObjectInstance : RuinsReclaimObjectInstance
{
	private RuinsReclaimStepWaveSkill m_skill;

	private Vector3 m_position = Vector3.zero;

	private Timer m_lifetimeTimer;

	public override int type => 2;

	public override Vector3 position => m_position;

	public override float interactionDuration => m_skill.objectInteractionDuration;

	public RuinsReclaimStepWaveSkill skill => m_skill;

	public void Init(RuinsReclaimInstance ruinsReclaimInst, RuinsReclaimStepWaveSkill skill, Vector3 position)
	{
		if (ruinsReclaimInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimInst");
		}
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_skill = skill;
		m_position = position;
		InitObject(ruinsReclaimInst);
		int nDuration = m_skill.objectLifetime * 1000;
		m_lifetimeTimer = new Timer(OnLifeTimeTimerTick);
		m_lifetimeTimer.Change(nDuration, -1);
	}

	private void OnLifeTimeTimerTick(object state)
	{
		m_currentPlace.AddWork(new SFAction(ExpireLifetime), bGlobalLockRequired: false);
	}

	private void ExpireLifetime()
	{
		if (!m_bIsReleased)
		{
			m_currentPlace.OnExpireMonsterTransformationCancelObjectLifetime(this);
		}
	}

	public override bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_skill.objectInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}

	protected override void ReleaseInternal()
	{
		DisposeLifetimeTimer();
		base.ReleaseInternal();
	}

	private void DisposeLifetimeTimer()
	{
		if (m_lifetimeTimer != null)
		{
			m_lifetimeTimer.Dispose();
			m_lifetimeTimer = null;
		}
	}

	public PDRuinsReclaimMonsterTransformationCancelObjectInstance ToPDRuinsReclaimMonsterTransformationCancelObjectInstnace()
	{
		PDRuinsReclaimMonsterTransformationCancelObjectInstance inst = new PDRuinsReclaimMonsterTransformationCancelObjectInstance();
		inst.instanceId = m_lnInstanceId;
		inst.position = m_position;
		return inst;
	}
}
public class RuinsReclaimMonsterTransformationCancelObjectInteractionStartCommandHandler : InGameCommandHandler<RuinsReclaimMonsterTransformationCancelObjectInteractionStartCommandBody, RuinsReclaimMonsterTransformationCancelObjectInteractionStartResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_NotWaveStep = 102;

	public const short kResult_Interaction = 103;

	public const short kResult_Dead = 104;

	public const short kResult_UnableInteractionPositionWithObject = 105;

	public const short kResult_NotTransformationMonster = 106;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is RuinsReclaimInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnInstanceId = m_body.instanceId;
		if (currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		RuinsReclaim ruinsReclaim = currentPlace.ruinsReclaim;
		RuinsReclaimStep step = ruinsReclaim.GetStep(currentPlace.stepNo);
		if (step.type != 3)
		{
			throw new CommandHandleException(102, "현재 웨이브단계가 아닙니다.");
		}
		RuinsReclaimMonsterTransformationCancelObjectInstance objectInst = currentPlace.GetMonsterTransformationCancelObject(lnInstanceId);
		if (objectInst == null)
		{
			throw new CommandHandleException(1, "오브젝트 인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		if (objectInst.interactionHero != null)
		{
			throw new CommandHandleException(103, "오브젝트가 상호작용중입니다");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(104, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.moving)
		{
			throw new CommandHandleException(1, "영웅이 이동중입니다.");
		}
		if (m_myHero.autoHunting)
		{
			throw new CommandHandleException(1, "영웅이 자동사냥중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new CommandHandleException(1, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		if (!objectInst.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(105, "해당 오브젝트와 상호작용할 수없는 위치입니다.");
		}
		if (!m_myHero.isTransformRuinsReclaimMonster)
		{
			throw new CommandHandleException(106, "영웅이 몬스터로 변신하지 않았습니다.");
		}
		m_myHero.StartRuinsReclaimObjectInteraction(objectInst);
		SendResponseOK(null);
	}
}
