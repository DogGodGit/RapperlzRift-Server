using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ServerIllustratedBookRankingCommandHandler : InGameCommandHandler<ServerIllustratedBookRankingCommandBody, ServerIllustratedBookRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerIllustratedBookRankingManager manager = ServerIllustratedBookRankingManager.instance;
		IllustratedBookRanking myRanking = manager.GetRankingOfHero(m_myHero.id);
		ServerIllustratedBookRankingResponseBody resBody = new ServerIllustratedBookRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = manager.GetPDRankings(Resource.instance.rankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
