using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class PartySurroundingPartyListCommandHandler : InGameCommandHandler<PartySurroundingPartyListCommandBody, PartySurroundingPartyListResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		Party myParty = m_myHero.partyMember?.party;
		List<PDSimpleParty> results = new List<PDSimpleParty>();
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			foreach (Sector sector in currentPlace.GetInterestSectors(m_myHero.sector))
			{
				foreach (Hero hero in sector.heroes.Values)
				{
					lock (hero.syncObject)
					{
						if (hero.id == m_myHero.id || !hero.isReal || hero.nationId != m_myHero.nationId)
						{
							continue;
						}
						PartyMember partyMember = hero.partyMember;
						if (partyMember != null && partyMember.isMaster && partyMember.isLoggedIn)
						{
							Party party = partyMember.party;
							if (!party.isMemberFull && (myParty == null || !(party.id == myParty.id)))
							{
								results.Add(party.ToPDSimpleParty());
							}
						}
					}
				}
			}
		}
		PartySurroundingPartyListResponseBody resBody = new PartySurroundingPartyListResponseBody();
		resBody.parties = results.ToArray();
		SendResponseOK(resBody);
	}
}
