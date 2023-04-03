using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class ContinentEnterForContinentTransmissionCommandHandler : InGameCommandHandler<ContinentEnterForContinentTransmissionCommandBody, ContinentEnterForContinentTransmissionResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentEnterForContinentTransmissionParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		ContinentTransmissionExit continentTransmissionExit = param.continentTransmissionExit;
		if (continentTransmissionExit == null)
		{
			throw new CommandHandleException(1, "해당 대륙전송출구는 존재하지 않습니다.");
		}
		Continent targetContinent = continentTransmissionExit.continent;
		_ = targetContinent.id;
		ContinentInstance targetPlace = null;
		targetPlace = ((!targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(targetContinent.id)) : ((ContinentInstance)m_myHero.nationInst.GetContinentInstance(targetContinent.id)));
		if (targetPlace == null)
		{
			throw new CommandHandleException(1, "해당 장소가 존재하지 않습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(continentTransmissionExit.SelectPosition(), continentTransmissionExit.SelectRotationY());
			targetPlace.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			ContinentEnterForContinentTransmissionResponseBody resBody = new ContinentEnterForContinentTransmissionResponseBody();
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, currentTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			SendResponseOK(resBody);
		}
	}
}
public class ContinentEnterForContinentTransmissionParam : PlaceEntranceParam
{
	private ContinentTransmissionExit m_continentTransmissionExit;

	public ContinentTransmissionExit continentTransmissionExit => m_continentTransmissionExit;

	public ContinentEnterForContinentTransmissionParam(ContinentTransmissionExit continentTransmissionExit)
	{
		m_continentTransmissionExit = continentTransmissionExit;
	}
}
