using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeEnterCommandHandler : InGameCommandHandler<UndergroundMazeEnterCommandBody, UndergroundMazeEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is UndergroundMazeEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		UndergroundMazeFloor floor = param.floor;
		DateTimeOffset enterTime = param.enterTime;
		if (floor == null)
		{
			throw new CommandHandleException(1, "지하미궁층이 존재하지 않습니다.");
		}
		UndergroundMazeInstance undergroundMazeInst = m_myHero.nationInst.GetUndergroundMazeInstance(floor.floor);
		if (undergroundMazeInst == null)
		{
			throw new CommandHandleException(1, "지하미궁인스턴스가 존재하지 않습니다.");
		}
		lock (undergroundMazeInst.syncObject)
		{
			UndergroundMaze undergroundMaze = floor.undergroundMaze;
			m_myHero.SetPositionAndRotation(undergroundMaze.SelectPosition(), undergroundMaze.SelectRotationY());
			undergroundMazeInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			m_myHero.StartUndergroundMazePlay(enterTime);
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddUndergroundMazePlayLog(m_myHero.undergroundMazeLogId, undergroundMazeInst.instanceId, m_myHero.id, 0, enterTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				LogError(null, ex, bStackTrace: true);
			}
			UndergroundMazeEnterResponseBody resBody = new UndergroundMazeEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.playtime = m_myHero.undergroundMazePlayTime;
			resBody.placeInstanceId = (Guid)undergroundMazeInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			List<Sector> interestSectors = undergroundMazeInst.GetInterestSectors(m_myHero.sector);
			resBody.heroes = Sector.GetPDHeroes(interestSectors, m_myHero.id, enterTime).ToArray();
			resBody.monsterInsts = Sector.GetPDMonsterInstances<PDUndergroundMazeMonsterInstance>(interestSectors, enterTime).ToArray();
			SendResponseOK(resBody);
			m_myHero.ProcessMainQuestForContent(5);
			m_myHero.ProcessSubQuestForContent(5);
		}
	}
}
