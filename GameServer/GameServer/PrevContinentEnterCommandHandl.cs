using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class PrevContinentEnterCommandHandler : InGameCommandHandler<PrevContinentEnterCommandBody, PrevContinentEnterResponseBody>
{
	public const short kResult_ProgressingNationWarByTargetNation = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		Continent prevContinent = m_myHero.previousContinent;
		if (prevContinent == null)
		{
			throw new CommandHandleException(1, "이전 대륙정보가 유효하지 않습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		ContinentInstance prevContinentInst = null;
		int nPrevNationId = 0;
		if (prevContinent.isNationTerritory)
		{
			nPrevNationId = m_myHero.previousNationId;
			if (nPrevNationId == 0)
			{
				throw new CommandHandleException(1, "이전 국가정보가 유효하지 않습니다.");
			}
			NationInstance nationInst = Cache.instance.GetNationInstance(nPrevNationId);
			if (nationInst == null)
			{
				throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. nPrevNationId = " + nPrevNationId);
			}
			NationWarInstance nationWarInst = nationInst.nationWarInst;
			if (nationWarInst != null && !nationWarInst.IsNationWarJoinEnabled(m_myHero))
			{
				Resource res = Resource.instance;
				m_myHero.placeEntranceParam = new ContinentSaftyAreaEnterParam(res.GetContinent(res.saftyRevivalContinentId), m_myHero.nationId, res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation(), currentTime);
				throw new CommandHandleException(101, "해당 국가가 국가전 진행중이므로 입장할 수 없습니다.");
			}
			prevContinentInst = Cache.instance.GetNationInstance(nPrevNationId).GetContinentInstance(prevContinent.id);
		}
		else
		{
			prevContinentInst = Cache.instance.GetDisputeContinentInstance(prevContinent.id);
		}
		if (prevContinentInst == null)
		{
			throw new CommandHandleException(1, "이전 대륙인스턴스가 존재하지 않습니다.");
		}
		lock (prevContinentInst.syncObject)
		{
			m_myHero.RestoreLocationInfo();
			prevContinentInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			PrevContinentEnterResponseBody resBody = new PrevContinentEnterResponseBody();
			List<Sector> interestSectors = prevContinentInst.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)prevContinentInst.instanceId, prevContinentInst.continent.id, prevContinentInst.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, currentTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			SendResponseOK(resBody);
		}
	}
}
