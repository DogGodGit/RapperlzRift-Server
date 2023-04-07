using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentObjectInteractionStartCommandHandler : InGameCommandHandler<ContinentObjectInteractionStartCommandBody, ContinentObjectInteractionStartResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_NotExistObject = 102;

	public const short kResult_OutOfInteractionRange = 103;

	public const short kResult_NotInteractionEnabled = 104;

	protected override void HandleInGameCommand()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "Body가 null입니다.");
		}
		long lnInstanceId = m_body.instanceId;
		if (lnInstanceId <= 0)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다. lnInstanceId = " + lnInstanceId);
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(1, "영웅이 탈것을 타고있는 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
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
		ContinentObjectInstance objectInst = currentPlace.GetObject(lnInstanceId);
		if (objectInst == null)
		{
			throw new CommandHandleException(102, "해당 오브젝트가 존재하지 않습니다.");
		}
		ContinentObject continentObject = objectInst.obj;
		if (!m_myHero.IsAvailableInteractionObject(continentObject.id, currentTime))
		{
			throw new CommandHandleException(1, "영웅이 목표 오브젝트와 상호작용 할 수 없습니다.");
		}
		if (objectInst.isPublic && !objectInst.isInteractionEnabled)
		{
			throw new CommandHandleException(104, "현재 오브젝트가 상호작용 할 수 있는 상태가 아닙니다.");
		}
		if (!objectInst.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, string.Concat("영웅이 대륙오브젝트의 상호작용 범위 안에 있지 않습니다. heroPosition = ", m_myHero.position, ", objectInstPosition = ", objectInst.position));
		}
		m_myHero.StartContinentObjectInteraction(objectInst);
		SendResponseOK(null);
	}
}
