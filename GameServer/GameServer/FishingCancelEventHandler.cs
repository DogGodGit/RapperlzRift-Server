using ClientCommon;

namespace GameServer;

public class FishingCancelEventHandler : InGameEventHandler<CEBFishingCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			m_myHero.CancelFishing(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
