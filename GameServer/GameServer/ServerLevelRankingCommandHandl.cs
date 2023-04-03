using ClientCommon;

namespace GameServer;

public class ServerLevelRankingCommandHandler : InGameCommandHandler<ServerLevelRankingCommandBody, ServerLevelRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerLevelRankingManager manager = ServerLevelRankingManager.instance;
		Ranking myRanking = manager.GetRankingOfHero(m_myHero.id);
		ServerLevelRankingResponseBody resBody = new ServerLevelRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = manager.GetPDRankings(Resource.instance.rankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
