using System;
using ClientCommon;

namespace GameServer;

public class NationWarResultCommandHandler : InGameCommandHandler<NationWarResultCommandBody, NationWarResultResponseBody>
{
	public const short kResult_ResultNotExist = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		Cache cache = Cache.instance;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		NationWarManager nationWarManager = cache.nationWarManager;
		NationWarResult nationWarResult = nationWarManager.GetNationWarResult(m_myHero.nationId, currentTime.Date);
		if (nationWarResult == null)
		{
			throw new CommandHandleException(101, "국가전 결과가 존재하지 않습니다.");
		}
		NationWarMember nationWarMember = nationWarResult.GetNationWarMember(m_myHero.id);
		NationWarResultResponseBody resBody = new NationWarResultResponseBody();
		resBody.winNationId = nationWarResult.winNationId;
		if (nationWarMember != null)
		{
			resBody.myRanking = nationWarMember.ranking;
			resBody.myKillCount = nationWarMember.killCount;
			resBody.myAssistCount = nationWarMember.assistCount;
			resBody.myDeadCount = nationWarMember.deadCount;
			resBody.myImmediateRevivalCount = nationWarMember.immediateRevivalCount;
			resBody.rewardedExp = nationWarMember.rewardedExp;
		}
		resBody.offenseNationId = nationWarResult.offenseNationId;
		resBody.offenseNationRankings = nationWarResult.GetPDOffenseNationWarRankings().ToArray();
		resBody.defenseNationId = nationWarResult.defenseNationId;
		resBody.defenseNationRankings = nationWarResult.GetPDDefenseNationWarRankings().ToArray();
		SendResponseOK(resBody);
	}
}
