using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class GuildTerritoryReviveCommandHandler : InGameCommandHandler<GuildTerritoryReviveCommandBody, GuildTerritoryReviveResponseBody>
{
	public const short kResult_NotDead = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태가 아닙니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new GuildTerritoryEnterForGuildTerritoryRevivalParam());
		SendResponseOK(null);
	}
}
