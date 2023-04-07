using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ServerCreatureCardRankingCommandHandler : InGameCommandHandler<ServerCreatureCardRankingCommandBody, ServerCreatureCardRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerCreatureCardRankingManager manager = ServerCreatureCardRankingManager.instance;
		CreatureCardRanking myRanking = manager.GetRankingOfHero(m_myHero.id);
		ServerCreatureCardRankingResponseBody resBody = new ServerCreatureCardRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = manager.GetPDRankings(Resource.instance.rankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
