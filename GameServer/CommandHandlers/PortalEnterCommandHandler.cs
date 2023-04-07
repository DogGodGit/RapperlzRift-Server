using ClientCommon;

namespace GameServer.CommandHandlers;

public class PortalEnterCommandHandler : InGameCommandHandler<PortalEnterCommandBody, PortalEnterResponseBody>
{
    public const short kResult_Dead = 101;

    public const short kResult_ProgressingNationWarByTargetNation = 102;

    public const short kResult_LevelUnderflowed = 103;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "바디가 null입니다.");
        }
        int nPortalId = m_body.portalId;
        if (nPortalId <= 0)
        {
            throw new CommandHandleException(1, "포탈ID가 유효하지 않습니다. nPortalId = " + nPortalId);
        }
        if (!(m_myHero.currentPlace is NationContinentInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        Continent continent = currentPlace.continent;
        Portal portal = continent.GetPortal(nPortalId);
        if (portal == null)
        {
            throw new CommandHandleException(1, "포탈이 존재하지 않습니다. continentId = " + continent.id + ", nPortalId = " + nPortalId);
        }
        if (!portal.IsEnterablePosition(m_myHero.position, m_myHero.radius))
        {
            throw new CommandHandleException(1, "입장할 수 없는 위치입니다.");
        }
        Portal linkedPortal = portal.linkedPortal;
        int nHeroLevel = m_myHero.level;
        int nRequiredHeroLevel = linkedPortal.continent.requiredHeroLevel;
        if (nHeroLevel < nRequiredHeroLevel)
        {
            throw new CommandHandleException(103, "영웅의 레벨이 낮아 해당 대륙전송을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
        }
        if (m_myHero.isDead)
        {
            throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
        }
        if (m_myHero.isRidingCart)
        {
            throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
        }
        NationInstance nationInst = Cache.instance.GetNationInstance(currentPlace.nationId);
        NationWarInstance nationWarInst = nationInst.nationWarInst;
        if (nationWarInst != null && !nationWarInst.IsNationWarJoinEnabled(m_myHero))
        {
            throw new CommandHandleException(102, "해당 국가가 국가전 진행중이므로 입장할 수 없습니다.");
        }
        currentPlace.Exit(m_myHero, isLogOut: false, new PortalExitParam(linkedPortal, currentPlace.nation.id));
        SendResponseOK(null);
    }
}
