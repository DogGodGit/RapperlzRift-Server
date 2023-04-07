using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class GuildTerritoryExitCommandHandler : InGameCommandHandler<GuildTerritoryExitCommandBody, GuildTerritoryExitResponseBody>
{
	public const short kResult_Dead = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, null);
		GuildTerritoryExitResponseBody resBody = new GuildTerritoryExitResponseBody();
		resBody.previousContinentId = m_myHero.previousContinentId;
		resBody.previousNationId = m_myHero.previousNationId;
		SendResponseOK(resBody);
	}
}
