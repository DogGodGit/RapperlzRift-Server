using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class FieldOfHonorTopRankingListCommandHandler : InGameCommandHandler<FieldOfHonorTopRankingListCommandBody, FieldOfHonorTopRankingListResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		List<PDFieldOfHonorRanking> rankings = new List<PDFieldOfHonorRanking>();
		Cache cache = Cache.instance;
		for (int i = 1; i <= 50; i++)
		{
			PDFieldOfHonorRanking hero = cache.GetPDFieldOfHonorRanking(i);
			if (hero != null)
			{
				rankings.Add(hero);
			}
		}
		FieldOfHonorTopRankingListResponseBody resBody = new FieldOfHonorTopRankingListResponseBody();
		resBody.myRanking = m_myHero.fieldOfHonorRanking;
		resBody.rankings = rankings.ToArray();
		SendResponseOK(resBody);
	}
}
