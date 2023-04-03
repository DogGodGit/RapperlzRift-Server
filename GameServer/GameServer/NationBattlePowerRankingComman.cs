using ClientCommon;

namespace GameServer;

public class NationBattlePowerRankingCommandHandler : InGameCommandHandler<NationBattlePowerRankingCommandBody, NationBattlePowerRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		NationInstance nationInst = m_myHero.nationInst;
		Ranking myRanking = nationInst.GetBattlePowerRankingOfHero(m_myHero.id);
		NationBattlePowerRankingResponseBody resBody = new NationBattlePowerRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = nationInst.GetBattlePowerPDRankings(Resource.instance.rankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
