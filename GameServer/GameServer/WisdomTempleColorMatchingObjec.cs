using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WisdomTempleColorMatchingObject : IPickEntry
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private WisdomTemple m_wisdomTemple;

	private int m_nId;

	private float m_fInteractionDuration;

	private float m_fInteractionMaxRange;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int id => m_nId;

	public float interactionDuration => m_fInteractionDuration;

	public float interactionMaxRange => m_fInteractionMaxRange;

	public int point => 1;

	int IPickEntry.point => point;

	public WisdomTempleColorMatchingObject(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["objectId"]);
		m_fInteractionDuration = Convert.ToSingle(dr["interactionDuration"]);
		if (m_fInteractionDuration < 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_fInteractionDuration = " + m_fInteractionDuration);
		}
		m_fInteractionMaxRange = Convert.ToInt32(dr["interactionMaxRange"]);
		if (m_fInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용최대범위가 유효하지 않습니다. m_nId = " + m_nId + ", m_fInteractionMaxRange = " + m_fInteractionMaxRange);
		}
	}
}
public class WisdomTempleColorMatchingObjectCheckCommandHandler : InGameCommandHandler<WisdomTempleColorMatchingObjectCheckCommandBody, WisdomTempleColorMatchingObjectCheckResponseBody>
{
	public const short kResult_InvalidStep = 101;

	public const short kResult_NotStatusPlaying = 102;

	public const short kResult_NotColorMatching = 103;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is WisdomTempleInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nStepNo = m_body.stepNo;
		if (nStepNo != currentPlace.stepNo)
		{
			throw new CommandHandleException(101, "유효하지 않는 단계입니다. nStepNo = " + nStepNo);
		}
		if (currentPlace.status != 2)
		{
			throw new CommandHandleException(102, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.currentPuzzleId != 1)
		{
			throw new CommandHandleException(103, "현재 색맞추기퍼즐 진행중이 아닙니다.");
		}
		List<PDWisdomTempleColorMatchingObjectInstance> createdColorMatchingObjectInsts = new List<PDWisdomTempleColorMatchingObjectInstance>();
		currentPlace.CheckColorMatchingObject(createdColorMatchingObjectInsts);
		WisdomTempleColorMatchingObjectCheckResponseBody resBody = new WisdomTempleColorMatchingObjectCheckResponseBody();
		resBody.colorMatchingPoint = currentPlace.colorMatchingPoint;
		resBody.createdColorMatchingObjectInsts = createdColorMatchingObjectInsts.ToArray();
		SendResponseOK(resBody);
		currentPlace.CheckColorMatchingPoint();
	}
}
public class WisdomTempleColorMatchingObjectInstance : WisdomTempleObjectInstance
{
	private WisdomTempleColorMatchingObject m_obj;

	private WisdomTempleArrangePosition m_arrangePosition;

	public override int type => 1;

	public WisdomTempleColorMatchingObject obj => m_obj;

	public WisdomTempleArrangePosition arrangePosition => m_arrangePosition;

	public override Vector3 position => m_arrangePosition.position;

	public override float interactionDuration => m_obj.interactionDuration;

	public void Init(WisdomTempleInstance currentPlace, WisdomTempleColorMatchingObject obj, WisdomTempleArrangePosition arrangePosition)
	{
		if (currentPlace == null)
		{
			throw new ArgumentNullException("currentPlace");
		}
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (arrangePosition == null)
		{
			throw new ArgumentNullException("arrangePosition");
		}
		m_obj = obj;
		m_arrangePosition = arrangePosition;
		InitObject(currentPlace);
	}

	public override bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_arrangePosition.position, m_obj.interactionMaxRange * 1.1f + fRadius * 2f, position);
	}

	public PDWisdomTempleColorMatchingObjectInstance ToPDWisdomTempleColorMatchingObjectInstance()
	{
		PDWisdomTempleColorMatchingObjectInstance inst = new PDWisdomTempleColorMatchingObjectInstance();
		inst.instanceId = m_lnInstanceId;
		inst.objectId = m_obj.id;
		inst.row = m_arrangePosition.row;
		inst.col = m_arrangePosition.col;
		inst.position = m_arrangePosition.position;
		return inst;
	}
}
public class WisdomTempleColorMatchingObjectInteractionStartCommandHandler : InGameCommandHandler<WisdomTempleColorMatchingObjectInteractionStartCommandBody, WisdomTempleColorMatchingObjectInteractionStartResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_NotColorMatching = 102;

	public const short kResult_Dead = 103;

	public const short kResult_UnableInteractionPositionWithColorMatchingObject = 104;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is WisdomTempleInstance currentPlace))
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
		if (currentPlace.currentPuzzleId != 1)
		{
			throw new CommandHandleException(102, "현재 색맞추기퍼즐 진행중이 아닙니다.");
		}
		WisdomTempleColorMatchingObjectInstance objectInst = currentPlace.GetColorMatchingObjectInstance_ByInstanceId(lnInstanceId);
		if (objectInst == null)
		{
			throw new CommandHandleException(1, "오브젝트 인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(103, "영웅이 죽은 상태입니다.");
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
			throw new CommandHandleException(104, "해당 오브젝트와 상호작용할 수없는 위치입니다.");
		}
		m_myHero.StartWisdomTempleObjectInteraction(objectInst);
		SendResponseOK(null);
	}
}
