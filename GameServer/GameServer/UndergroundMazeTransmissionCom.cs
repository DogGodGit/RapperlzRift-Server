using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class UndergroundMazeTransmissionCommandHandler : InGameCommandHandler<UndergroundMazeTransmissionCommandBody, UndergroundMazeTransmissionResponseBody>
{
	public const short kResult_OutOfRange = 101;

	public const short kResult_Dead = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is UndergroundMazeInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nNpcId = m_body.npcId;
		int nFloor = m_body.floor;
		UndergroundMaze undergroundMaze = Resource.instance.undergroundMaze;
		UndergroundMazeNpc npc = currentPlace.floor.GetNpc(nNpcId);
		if (npc == null)
		{
			throw new CommandHandleException(1, "해당 NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
		}
		UndergroundMazeFloor floor = undergroundMaze.GetFloor(nFloor);
		if (floor == null)
		{
			throw new CommandHandleException(1, "해당 지하미로층이 존재하지 않습니다. nFloor = " + nFloor);
		}
		if (npc.GetTransmissionEntry(nFloor) == null)
		{
			throw new CommandHandleException(1, "해당 층은 이동이 불가능 합니다. nFloor = " + nFloor);
		}
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "입장할 수 없는 위치입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new UndergroundMazeEnterForUndergroundMazeTransmissionParam(floor));
		SendResponseOK(null);
	}
}
