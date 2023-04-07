using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class InfiniteWarMatchingCancelCommandHandler : InGameCommandHandler<InfiniteWarMatchingCancelCommandBody, InfiniteWarMatchingCancelResponseBody>
{
	public const short kResult_NotInfiniteWarMatching = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.matchingRoom is InfiniteWarMatchingRoom matchingRoom))
		{
			throw new CommandHandleException(101, "무한대전 매칭중이 아닙니다.");
		}
		matchingRoom.ExitHero(m_myHero);
		SendResponseOK(null);
	}
}
