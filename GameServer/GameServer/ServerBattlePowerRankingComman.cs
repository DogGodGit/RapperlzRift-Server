using ClientCommon;

namespace GameServer;

public class ServerBattlePowerRankingCommandHandler : InGameCommandHandler<ServerBattlePowerRankingCommandBody, ServerBattlePowerRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		ServerBattlePowerRankingManager manager = ServerBattlePowerRankingManager.instance;
		Ranking myRanking = manager.GetRankingOfHero(m_myHero.id);
		ServerBattlePowerRankingResponseBody resBody = new ServerBattlePowerRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = manager.GetPDRankings(Resource.instance.rankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
