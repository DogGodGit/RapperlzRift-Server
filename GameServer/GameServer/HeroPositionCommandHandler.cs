using System;
using ClientCommon;

namespace GameServer;

public class HeroPositionCommandHandler : InGameCommandHandler<HeroPositionCommandBody, HeroPositionResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다");
		}
		Guid heroId = (Guid)m_body.heroId;
		if (heroId == Guid.Empty)
		{
			throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다.");
		}
		if (m_myHero.id == heroId)
		{
			throw new CommandHandleException(1, "영웅ID가 내 영웅 ID입니다.");
		}
		HeroPositionResponseBody resBody = new HeroPositionResponseBody();
		Hero targetHero = Cache.instance.GetLoggedInHero(heroId);
		if (targetHero != null)
		{
			resBody.isLoggedIn = true;
			lock (targetHero.syncObject)
			{
				Place targetHeroPlace = targetHero.currentPlace;
				if (targetHeroPlace != null)
				{
					resBody.placeInstanceId = (Guid)targetHeroPlace.instanceId;
					resBody.locationId = targetHeroPlace.location.locationId;
					resBody.locationParam = targetHeroPlace.locationParam;
					resBody.position = targetHero.position;
				}
			}
		}
		else
		{
			resBody.isLoggedIn = false;
		}
		SendResponseOK(resBody);
	}
}
