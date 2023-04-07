using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class TradeShipMatchingCancelCommandHandler : InGameCommandHandler<TradeShipMatchingCancelCommandBody, TradeShipMatchingCancelResponseBody>
{
	public const short kResult_NotTradeShipMatching = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.matchingRoom is TradeShipMatchingRoom matchingRoom))
		{
			throw new CommandHandleException(101, "무역선탈환 매칭중이 아닙니다.");
		}
		matchingRoom.ExitHero(m_myHero);
		SendResponseOK(null);
	}
}
