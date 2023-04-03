using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeExitCommandHandler : InGameCommandHandler<UndergroundMazeExitCommandBody, UndergroundMazeExitResponseBody>
{
	public const short kResult_Dead = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is UndergroundMazeInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, null);
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nPlayTime = (int)Math.Floor((DateTimeUtil.currentTime - m_myHero.undergroundMazeStartTime).TotalSeconds);
			logWork.AddSqlCommand(GameLogDac.CSC_UpdateUndergroundMazePlayLog(m_myHero.undergroundMazeLogId, nPlayTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		UndergroundMazeExitResponseBody resBody = new UndergroundMazeExitResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		resBody.date = (DateTime)m_myHero.undergroundMazeDate;
		resBody.playTime = m_myHero.undergroundMazePlayTime;
		SendResponseOK(resBody);
	}
}
