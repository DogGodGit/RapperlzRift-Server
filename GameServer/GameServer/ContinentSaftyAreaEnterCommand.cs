using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class ContinentSaftyAreaEnterCommandHandler : InGameCommandHandler<ContinentSaftyAreaEnterCommandBody, ContinentSaftyAreaEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentSaftyAreaEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		Continent targetContinent = param.continent;
		int nTargetNationid = param.nationId;
		Vector3 targetPosition = param.position;
		float fTargetRotationY = param.rotationY;
		DateTimeOffset enterTime = param.enterTime;
		ContinentInstance targetPlace = null;
		if (targetContinent.isNationTerritory)
		{
			NationInstance nationInst = Cache.instance.GetNationInstance(nTargetNationid);
			if (nationInst == null)
			{
				throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. nTargetNationId = " + nTargetNationid);
			}
			targetPlace = nationInst.GetContinentInstance(targetContinent.id);
		}
		else
		{
			targetPlace = Cache.instance.GetDisputeContinentInstance(targetContinent.id);
		}
		if (targetPlace == null)
		{
			throw new CommandHandleException(1, "해당 장소가 존재하지 않습니다.");
		}
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(targetPosition, fTargetRotationY);
			targetPlace.Enter(m_myHero, enterTime, bIsRevivalEnter: true);
			ContinentSaftyAreaEnterResponseBody resBody = new ContinentSaftyAreaEnterResponseBody();
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, enterTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, enterTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, enterTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			SendResponseOK(resBody);
		}
	}
}
