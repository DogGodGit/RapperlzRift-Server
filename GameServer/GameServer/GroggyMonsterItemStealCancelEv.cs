using ClientCommon;

namespace GameServer;

public class GroggyMonsterItemStealCancelEventHandler : InGameEventHandler<CEBGroggyMonsterItemStealCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			m_myHero.CancelGroggyMonsterItemSteal(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
