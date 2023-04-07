using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class PortalExitCommandHandler : InGameCommandHandler<PortalExitCommandBody, PortalExitResponseBody>
{
    public const short kResult_ProgressingNationWarByTargetNation = 101;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_myHero.currentPlace != null)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!(m_myHero.placeEntranceParam is PortalExitParam param))
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
        NationWarInstance nationWarInst = nationInst.nationWarInst;
        if (nationWarInst != null && !nationWarInst.IsNationWarJoinEnabled(m_myHero))
        {
            Resource res = Resource.instance;
            m_myHero.placeEntranceParam = new ContinentSaftyAreaEnterParam(res.GetContinent(res.saftyRevivalContinentId), m_myHero.nationId, res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation(), currentTime);
            throw new CommandHandleException(101, "해당 국가가 국가전 진행중이므로 입장할 수 없습니다.");
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
            PortalExitResponseBody resBody = new PortalExitResponseBody();
            List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
            resBody.entranceInfo = new PDContinentEntranceInfo(targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, currentTime).ToArray(), m_myHero.position, m_myHero.rotationY);
            SendResponseOK(resBody);
        }
    }
}
