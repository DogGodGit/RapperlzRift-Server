using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class FieldOfHonorRankerInfoCommandHandler : InGameCommandHandler<FieldOfHonorRankerInfoCommandBody, FieldOfHonorRankerInfoResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid heroId = (Guid)m_body.heroId;
		FieldOfHonorHero hero = Cache.instance.GetFieldOfHonorHeroOfHeroId(heroId);
		FieldOfHonorRankerInfoResponseBody resBody = new FieldOfHonorRankerInfoResponseBody();
		resBody.ranker = hero?.ToPDFieldOfHonorHero();
		SendResponseOK(resBody);
	}
}
