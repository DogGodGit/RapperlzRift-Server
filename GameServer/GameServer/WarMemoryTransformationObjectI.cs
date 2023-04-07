using System;
using System.Threading;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WarMemoryTransformationObjectInteractionStartCommandHandler : InGameCommandHandler<WarMemoryTransformationObjectInteractionStartCommandBody, WarMemoryTransformationObjectInteractionStartResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_NotExistTransformationObject = 102;

	public const short kResult_Interaction = 103;

	public const short kResult_Dead = 104;

	public const short kResult_UnableInteractionPositionWithObject = 105;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is WarMemoryInstance currentPlace))
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
		WarMemoryTransformationObjectInstance objectInst = currentPlace.GetTransformationObject(lnInstanceId);
		if (objectInst == null)
		{
			throw new CommandHandleException(102, "오브젝트 인스턴스가 존재하지 않습니다. lnInstanceId = " + lnInstanceId);
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
		m_myHero.StartWarMemoryTransformationObjectInteraction(objectInst);
		SendResponseOK(null);
	}
}
public class WarMemoryTransformationObjectInstance
{
	private long m_lnInstanceId;

	private WarMemoryTransformationObject m_transformationObject;

	private WarMemoryInstance m_warMemoryInst;

	private Vector3 m_position = Vector3.zero;

	private Hero m_interactionHero;

	private Timer m_lifetimeTimer;

	private bool m_bIsReleased;

	public static readonly SFSynchronizedLongFactory instanceIdFactory = new SFSynchronizedLongFactory();

	public long instanceId => m_lnInstanceId;

	public WarMemoryTransformationObject transformationObject => m_transformationObject;

	public WarMemoryInstance warMemoryInst => m_warMemoryInst;

	public Vector3 position => m_position;

	public Hero interactionHero
	{
		get
		{
			return m_interactionHero;
		}
		set
		{
			m_interactionHero = value;
		}
	}

	public float interactionDuration => m_transformationObject.objectInteractionDuration;

	public bool isReleased => m_bIsReleased;

	public WarMemoryTransformationObjectInstance()
	{
		m_lnInstanceId = instanceIdFactory.NewValue();
	}

	public void Init(WarMemoryInstance warMemoryInst, WarMemoryTransformationObject transformationObject)
	{
		if (warMemoryInst == null)
		{
			throw new ArgumentNullException("warMemoryInst");
		}
		if (transformationObject == null)
		{
			throw new ArgumentNullException("transformationObject");
		}
		m_warMemoryInst = warMemoryInst;
		m_transformationObject = transformationObject;
		m_position = m_transformationObject.SelectPosition();
		int nDuration = m_transformationObject.objectLifetime * 1000;
		m_lifetimeTimer = new Timer(OnLifeTimeTimerTick);
		m_lifetimeTimer.Change(nDuration, -1);
	}

	private void OnLifeTimeTimerTick(object state)
	{
		m_warMemoryInst.AddWork(new SFAction(ExpireLifetime), bGlobalLockRequired: false);
	}

	private void ExpireLifetime()
	{
		if (!m_bIsReleased)
		{
			m_warMemoryInst.OnExpireTransformationObjectLifetime(this);
		}
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_transformationObject.objectInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}

	public void Release()
	{
		m_bIsReleased = true;
		DisposeLifeitmeTimer();
	}

	private void DisposeLifeitmeTimer()
	{
		if (m_lifetimeTimer != null)
		{
			m_lifetimeTimer.Dispose();
			m_lifetimeTimer = null;
		}
	}

	public PDWarMemoryTransformationObjectInstance ToPDWarMemoryTransformationObjectInstance()
	{
		PDWarMemoryTransformationObjectInstance inst = new PDWarMemoryTransformationObjectInstance();
		inst.instanceId = m_lnInstanceId;
		inst.objectId = m_transformationObject.id;
		inst.position = m_position;
		return inst;
	}
}
