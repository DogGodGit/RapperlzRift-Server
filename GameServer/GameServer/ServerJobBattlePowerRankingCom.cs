using ClientCommon;

namespace GameServer;

public class ServerJobBattlePowerRankingCommandHandler : InGameCommandHandler<ServerJobBattlePowerRankingCommandBody, ServerJobBattlePowerRankingResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nJobId = m_body.jobId;
		if (nJobId <= 0)
		{
			throw new CommandHandleException(1, "직업ID가 유효하지 않습니다.");
		}
		ServerJobBattlePowerRankingManager manager = ServerJobBattlePowerRankingManager.instance;
		ServerJobBattlePowerRankingCollection rankingCollection = manager.GetRankingCollection(nJobId);
		if (rankingCollection == null)
		{
			throw new CommandHandleException(1, "직업전투력랭킹 목록에 없는 직업입니다.");
		}
		Ranking myRanking = null;
		if (nJobId == m_myHero.baseJobId)
		{
			myRanking = manager.GetRankingOfHero(m_myHero.id);
		}
		ServerJobBattlePowerRankingResponseBody resBody = new ServerJobBattlePowerRankingResponseBody();
		resBody.myRanking = myRanking?.ToPDRanking();
		resBody.rankings = rankingCollection.GetPDRankings(Resource.instance.rankingDisplayMaxCount).ToArray();
		SendResponseOK(resBody);
	}
}
