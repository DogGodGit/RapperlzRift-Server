using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class UndergroundMazePortalExitCommandHandler : InGameCommandHandler<UndergroundMazePortalExitCommandBody, UndergroundMazePortalExitResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is UndergroundMazePortalExitParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		UndergroundMazePortal linkedPortal = param.linkedPortal;
		if (linkedPortal == null)
		{
			throw new CommandHandleException(1, "지하미궁연결포탈이 존재하지 않습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		UndergroundMazeInstance undergroundMazeInst = m_myHero.nationInst.GetUndergroundMazeInstance(linkedPortal.floor.floor);
		if (undergroundMazeInst == null)
		{
			throw new CommandHandleException(1, "지하미궁인스턴스가 존재하지 않습니다.");
		}
		lock (undergroundMazeInst.syncObject)
		{
			_ = Resource.instance.undergroundMaze;
			m_myHero.SetPositionAndRotation(linkedPortal.SelectExitPosition(), linkedPortal.SelectExitYRotation());
			undergroundMazeInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			UndergroundMazePortalExitResponseBody resBody = new UndergroundMazePortalExitResponseBody();
			resBody.placeInstanceId = (Guid)undergroundMazeInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.heroes = undergroundMazeInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = undergroundMazeInst.GetPDMonsterInstances<PDUndergroundMazeMonsterInstance>(currentTime).ToArray();
			SendResponseOK(resBody);
		}
	}
}
