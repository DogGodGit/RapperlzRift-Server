using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroSearchForFriendApplicationCommandHandler : InGameCommandHandler<HeroSearchForFriendApplicationCommandBody, HeroSearchForFriendApplicationResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null 입니다.");
		}
		string sText = m_body.text;
		if (string.IsNullOrEmpty(sText))
		{
			throw new CommandHandleException(1, "검색어가 없습니다.");
		}
		List<PDSearchHero> results = new List<PDSearchHero>();
		foreach (Hero hero in Cache.instance.SearchHeroesByName(sText, m_myHero.id))
		{
			if (!m_myHero.IsFriend(hero.id))
			{
				lock (hero.syncObject)
				{
					results.Add(hero.ToPDSearchHero());
				}
			}
		}
		HeroSearchForFriendApplicationResponseBody resBody = new HeroSearchForFriendApplicationResponseBody();
		resBody.results = results.ToArray();
		SendResponseOK(resBody);
	}
}
