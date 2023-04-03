using ClientCommon;

namespace GameServer;

public class AutoHuntEndEventHandler : InGameEventHandler<CEBAutoHuntEndEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace == null)
		{
			throw new EventHandleException("현재 장소에서 사용할 수 없는 이벤트 입니다.");
		}
		m_myHero.EndAutoHunt();
	}
}
