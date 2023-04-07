using System;
using System.Data;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class WisdomTemplePuzzleRewardObjectOffset
{
	private WisdomTemple m_wisdomTemple;

	private int m_nRewardCount;

	private int m_nNo;

	private Vector3 m_offset = Vector3.zero;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int rewardCount => m_nRewardCount;

	public int no => m_nNo;

	public Vector3 offset => m_offset;

	public WisdomTemplePuzzleRewardObjectOffset(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRewardCount = Convert.ToInt32(dr["rewardCount"]);
		m_nNo = Convert.ToInt32(dr["offsetNo"]);
		m_offset.x = Convert.ToSingle(dr["xOffset"]);
		m_offset.y = Convert.ToSingle(dr["yOffset"]);
		m_offset.z = Convert.ToSingle(dr["zOffset"]);
	}
}
public class WisdomTemplePuzzleRewardObjectInstance : WisdomTempleObjectInstance
{
	private Vector3 m_position = Vector3.zero;

	public override int type => 2;

	public override Vector3 position => m_position;

	public override float interactionDuration => m_currentPlace.wisdomTemple.puzzleRewardObjectInerationDuration;

	public void Init(WisdomTempleInstance currentPlace, Vector3 position)
	{
		if (currentPlace == null)
		{
			throw new ArgumentNullException("currentPlace");
		}
		m_position = position;
		InitObject(currentPlace);
	}

	public override bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		WisdomTemple wisdomTemple = m_currentPlace.wisdomTemple;
		return MathUtil.CircleContains(m_position, wisdomTemple.puzzleRewardObjectInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}

	public PDWisdomTemplePuzzleRewardObjectInstance ToPDWisdomTemplePuzzleRewardObjectInstance()
	{
		PDWisdomTemplePuzzleRewardObjectInstance inst = new PDWisdomTemplePuzzleRewardObjectInstance();
		inst.instanceId = m_lnInstanceId;
		inst.position = m_position;
		return inst;
	}
}
public class WisdomTemplePuzzleRewardObjectInteractionStartCommandHandler : InGameCommandHandler<WisdomTemplePuzzleRewardObjectInteractionStartCommandBody, WisdomTemplePuzzleRewardObjectInteractionStartResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_Dead = 102;

	public const short kResult_UnableInteractionPositionWithPuzzleRewardObject = 103;

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
		if (currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		long lnInstanceId = m_body.instanceId;
		WisdomTemplePuzzleRewardObjectInstance objectInst = currentPlace.GetPuzzleRewardObject(lnInstanceId);
		if (objectInst == null)
		{
			throw new CommandHandleException(1, "오브젝트 인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은 상태입니다.");
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
			throw new CommandHandleException(103, "해당 오브젝트와 상호작용할 수없는 위치입니다.");
		}
		m_myHero.StartWisdomTempleObjectInteraction(objectInst);
		SendResponseOK(null);
	}
}
