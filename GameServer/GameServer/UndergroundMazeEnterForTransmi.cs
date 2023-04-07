using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class UndergroundMazeEnterForTransmissionCommandHandler : InGameCommandHandler<UndergroundMazeEnterForTransmissionCommandBody, UndergroundMazeEnterForTransmissionResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is UndergroundMazeEnterForUndergroundMazeTransmissionParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		UndergroundMazeFloor floor = param.floor;
		if (floor == null)
		{
			throw new CommandHandleException(1, "지하미궁층이 존재하지 않습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		UndergroundMazeInstance undergroundMazeInst = m_myHero.nationInst.GetUndergroundMazeInstance(floor.floor);
		if (undergroundMazeInst == null)
		{
			throw new CommandHandleException(1, "지하미궁인스턴스가 존재하지 않습니다.");
		}
		lock (undergroundMazeInst.syncObject)
		{
			UndergroundMaze undergroundMaze = Resource.instance.undergroundMaze;
			m_myHero.SetPositionAndRotation(undergroundMaze.SelectPosition(), undergroundMaze.SelectRotationY());
			undergroundMazeInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			UndergroundMazeEnterForTransmissionResponseBody resBody = new UndergroundMazeEnterForTransmissionResponseBody();
			resBody.placeInstanceId = (Guid)undergroundMazeInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.heroes = undergroundMazeInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = undergroundMazeInst.GetPDMonsterInstances<PDUndergroundMazeMonsterInstance>(currentTime).ToArray();
			SendResponseOK(resBody);
		}
	}
}
