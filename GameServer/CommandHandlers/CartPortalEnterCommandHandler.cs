using ClientCommon;

namespace GameServer.CommandHandlers;

public class CartPortalEnterCommandHandler : InGameCommandHandler<CartPortalEnterCommandBody, CartPortalEnterResponseBody>
{
    public const short kResult_NotCartRiding = 101;

    public const short kResult_LevelUnderflowed = 102;

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
        Portal linkedPortal = portal.linkedPortal;
        int nHeroLevel = m_myHero.level;
        int nRequiredHeroLevel = linkedPortal.continent.requiredHeroLevel;
        if (nHeroLevel < nRequiredHeroLevel)
        {
            throw new CommandHandleException(102, "영웅의 레벨이 낮아 해당 대륙전송을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
        }
        CartInstance cartInst = m_myHero.ridingCartInst;
        if (cartInst == null)
        {
            throw new CommandHandleException(101, "카트를 타고있지 않습니다.");
        }
        lock (cartInst.syncObject)
        {
            if (!portal.IsEnterablePosition(cartInst.position, cartInst.radius))
            {
                throw new CommandHandleException(1, "입장할 수 없는 위치입니다.");
            }
            currentPlace.Exit(m_myHero, isLogOut: false, null);
            currentPlace.ExitCart(cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
            cartInst.placeEntranceParam = new PortalExitParam(linkedPortal, currentPlace.nation.id);
            SendResponseOK(null);
        }
    }
}
