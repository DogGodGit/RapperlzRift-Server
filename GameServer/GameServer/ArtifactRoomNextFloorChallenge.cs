using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ArtifactRoomNextFloorChallengeCommandHandler : InGameCommandHandler<ArtifactRoomNextFloorChallengeCommandBody, ArtifactRoomNextFloorChallengeResponseBody>
{
	public const short kResult_NotStatusClear = 101;

	public const short kResult_LevelUnderflowed = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ArtifactRoomInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 3)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		int nCurrentFloor = currentPlace.floor.floor;
		if (nCurrentFloor >= currentPlace.artifactRoom.topFloor)
		{
			throw new CommandHandleException(1, "현재 층이 마지막 층입니다.");
		}
		ArtifactRoomFloor nextFloor = currentPlace.artifactRoom.GetFloor(nCurrentFloor + 1);
		if (m_myHero.level < nextFloor.requiredHeroLevel)
		{
			throw new CommandHandleException(102, "영웅의 레벨이 낮아 다음층에 도전할 수 없습니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new ArtifactRoomEnterParam(DateTimeUtil.currentTime));
		SendResponseOK(null);
	}
}
