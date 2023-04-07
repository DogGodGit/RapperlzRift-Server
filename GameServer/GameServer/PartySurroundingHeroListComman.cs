using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class PartySurroundingHeroListCommandHandler : InGameCommandHandler<PartySurroundingHeroListCommandBody, PartySurroundingHeroListResponseBody>
{
	protected override void HandleInGameCommand()
	{
		List<PDSimpleHero> results = new List<PDSimpleHero>();
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			foreach (Sector sector in currentPlace.GetInterestSectors(m_myHero.sector))
			{
				foreach (Hero hero in sector.heroes.Values)
				{
					lock (hero.syncObject)
					{
						if (!(hero.id == m_myHero.id) && hero.isReal && hero.nationId == m_myHero.nationId && hero.partyMember == null)
						{
							results.Add(hero.ToPDSimpleHero());
						}
					}
				}
			}
		}
		PartySurroundingHeroListResponseBody resBody = new PartySurroundingHeroListResponseBody();
		resBody.heroes = results.ToArray();
		SendResponseOK(resBody);
	}
}
