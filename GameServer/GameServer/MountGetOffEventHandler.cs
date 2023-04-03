using ClientCommon;

namespace GameServer;

public class MountGetOffEventHandler : InGameEventHandler<CEBMountGetOffEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			m_myHero.GetOffMount(bSendEventToMyself: false);
		}
	}
}
