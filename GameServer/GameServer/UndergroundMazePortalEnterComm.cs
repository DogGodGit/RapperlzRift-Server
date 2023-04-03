using ClientCommon;

namespace GameServer;

public class UndergroundMazePortalEnterCommandHandler : InGameCommandHandler<UndergroundMazePortalEnterCommandBody, UndergroundMazePortalEnterResponseBody>
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
		int nPortalId = m_body.portalId;
		UndergroundMazePortal portal = currentPlace.floor.GetPortal(nPortalId);
		if (portal == null)
		{
			throw new CommandHandleException(1, "해당 포탈이 존재하지 않습니다. nPortalId = " + nPortalId);
		}
		if (!portal.IsEnterablePosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "입장할 수 없는 위치입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new UndergroundMazePortalExitParam(portal.linkedPortal));
		SendResponseOK(null);
	}
}
