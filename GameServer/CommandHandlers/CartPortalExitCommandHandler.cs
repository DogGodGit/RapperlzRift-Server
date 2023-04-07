using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class CartPortalExitCommandHandler : InGameCommandHandler<CartPortalExitCommandBody, CartPortalExitResponseBody>
{
    private CartInstance m_cartInst;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_cartInst = m_myHero.ridingCartInst;
        if (m_cartInst == null)
        {
            throw new CommandHandleException(1, "카트를 타고있지 않습니다.");
        }
        lock (m_cartInst.syncObject)
        {
            Process();
        }
    }

    private void Process()
    {
        if (m_cartInst.currentPlace != null)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!(m_cartInst.placeEntranceParam is PortalExitParam param))
        {
            throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
        }
        Portal portal = param.portal;
        int nNationId = param.nationId;
        DateTimeOffset currentTime = DateTimeUtil.currentTime;
        Continent targetContinent = portal.continent;
        Vector3 targetPosition = portal.SelectExitPosition();
        float fTargetYRotation = portal.SelectExitYRotation();
        NationInstance nationInst = Cache.instance.GetNationInstance(nNationId);
        if (nationInst == null)
        {
            throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. nNationId = " + nNationId);
        }
        NationContinentInstance targetPlace = nationInst.GetContinentInstance(targetContinent.id);
        if (targetPlace == null)
        {
            throw new CommandHandleException(1, "해당 대륙이 존재하지 않습니다. targetContinentId = " + targetContinent.id);
        }
        lock (targetPlace.syncObject)
        {
            m_myHero.SetPositionAndRotation(targetPosition, fTargetYRotation);
            targetPlace.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
            m_cartInst.SetPositionAndRotation(targetPosition, fTargetYRotation);
            targetPlace.EnterCart(m_cartInst, currentTime, bSendEvent: true);
            CartPortalExitResponseBody resBody = new CartPortalExitResponseBody();
            List<Sector> interestSectors = targetPlace.GetInterestSectors(m_cartInst.sector);
            resBody.entranceInfo = new PDContinentEntranceInfo(targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, m_cartInst.instanceId, currentTime).ToArray(), m_cartInst.position, m_cartInst.rotationY);
            SendResponseOK(resBody);
        }
    }
}
