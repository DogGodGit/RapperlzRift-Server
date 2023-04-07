using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ArtifactRoomSweepCommandHandler : InGameCommandHandler<ArtifactRoomSweepCommandBody, ArtifactRoomSweepResponseBody>
{
	public const short kResult_ClearedHeroBestFloor = 101;

	public const short kResult_Sweeping = 102;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		_ = Resource.instance.artifactRoom;
		int nHeroCurrentFloor = m_myHero.artifactRoomCurrentFloor;
		int nHeroBestFloor = m_myHero.artifactRoomBestFloor;
		if (nHeroCurrentFloor > nHeroBestFloor)
		{
			throw new CommandHandleException(101, "이미 플레이어의 최고층까지 클리어했습니다.");
		}
		if (m_myHero.artifactRoomSweepStartTime.HasValue)
		{
			throw new CommandHandleException(102, "이미 소탕중 입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_myHero.StartArtifactRoomSweep(currentTime);
		ArtifactRoomSweepResponseBody resBody = new ArtifactRoomSweepResponseBody();
		resBody.remainingTime = m_myHero.GetArtifactRoomSweepRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
