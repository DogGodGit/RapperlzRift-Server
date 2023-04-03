using ClientCommon;

namespace GameServer;

public class AutoHuntStartEventHandler : InGameEventHandler<CEBAutoHuntStartEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace == null)
		{
			throw new EventHandleException("현재 장소에서 사용할 수 없는 이벤트 입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new EventHandleException("영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new EventHandleException("카트에 탑승중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new EventHandleException("다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		m_myHero.StartAutoHunt();
	}
}
