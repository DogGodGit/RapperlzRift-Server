using System;
using ClientCommon;

namespace GameServer;

public class MoveStartEventHandler : InGameEventHandler<CEBMoveStartEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			if (m_body == null)
			{
				throw new EventHandleException("바디가 null입니다.");
			}
			Guid placeInstanceId = (Guid)m_body.placeInstanceId;
			if (placeInstanceId == Guid.Empty)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다.");
			}
			if (placeInstanceId != currentPlace.instanceId)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다. placeInstanceId = " + placeInstanceId);
			}
			Guid heroId = (Guid)m_body.heroId;
			bool bIsWalking = m_body.isWalking;
			bool bIsManualMoving = m_body.isManualMoving;
			Hero targetHero = ((heroId == m_myHero.id) ? m_myHero : currentPlace.GetHero(heroId));
			if (targetHero == null)
			{
				throw new EventHandleException("대상영웅이 존재하지 않습니다. heroId = " + heroId);
			}
			if (targetHero.controller.id != m_myHero.id)
			{
				throw new EventHandleException("대상영웅의 제어자가 아닙니다. heroId = " + heroId);
			}
			if (!targetHero.moveEnabled)
			{
				throw new EventHandleException("대상영웅이 움직일수 있는 상태가 아닙니다. heroId = " + heroId);
			}
			if (targetHero.isRidingCart)
			{
				throw new EventHandleException("카트에 탑승중입니다.");
			}
			HeroExclusiveAction currentExclusiveAction = targetHero.currentExclusiveAction;
			if (currentExclusiveAction != 0)
			{
				throw new EventHandleException("다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
			}
			targetHero.StartMove(bIsWalking, bIsManualMoving);
		}
	}
}
