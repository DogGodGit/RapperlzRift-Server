using ClientCommon;

namespace GameServer;

public class ContinentExitForGuildTerritoryEnterCommandHandler : InGameCommandHandler<ContinentExitForGuildTerritoryEnterCommandBody, ContinentExitForGuildTerritoryEnterResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_Dead = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되지 않았습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new GuildTerritoryEnterParam());
		SendResponseOK(null);
	}
}
