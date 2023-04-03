using System;
using ClientCommon;

namespace GameServer;

public class RuinsReclaimPortalEnterCommandHandler : InGameCommandHandler<RuinsReclaimPortalEnterCommandBody, RuinsReclaimPortalEnterResponseBody>
{
	public const short kResult_NotExistPortal = 101;

	public const short kResult_NotEnterablePosition = 102;

	public const short kResult_Dead = 103;

	public const short kResult_AlreadyRidingMount = 104;

	protected override void HandleInGameCommand()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is RuinsReclaimInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		int nPortalId = m_body.portalId;
		if (nPortalId <= 0)
		{
			throw new CommandHandleException(1, "포탈ID가 유효하지 않습니다. nPortalId = " + nPortalId);
		}
		RuinsReclaimPortal portal = currentPlace.GetActivationPortal(nPortalId);
		if (portal == null)
		{
			throw new CommandHandleException(101, "포탈이 존재하지 않습니다.");
		}
		if (!portal.IsEnterablePosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(102, "입장할 수 없는 위치입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(103, "영웅이 죽은 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(104, "영웅이 카트에 탑승중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new CommandHandleException(1, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		currentPlace.ChangeHeroPositionAndRotation(m_myHero, portal.SelectExitPosition(), portal.SelectExitYRotation(), bSendInterestTargetChangeEvent: true, currentTime);
		RuinsReclaimPortalEnterResponseBody resBody = new RuinsReclaimPortalEnterResponseBody();
		resBody.position = m_myHero.position;
		resBody.rotationY = m_myHero.rotationY;
		SendResponseOK(resBody);
		ServerEvent.SendHeroRuinsReclaimPortalEnter(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, m_myHero.position, m_myHero.rotationY);
	}
}
