using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class ContinentEnterForNationTransmissionCommandHandler : InGameCommandHandler<ContinentEnterForNationTransmissionCommandBody, ContinentEnterForNationTransmissionResponseBody>
{
	public const short kResult_ProgressingNationWarByTargetNation = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentEnterForNationTransmissionParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		Nation nation = param.nation;
		Continent continent = param.continent;
		if (nation == null)
		{
			throw new CommandHandleException(1, "해당 국가는 존재하지 안습니다.");
		}
		if (continent == null)
		{
			throw new CommandHandleException(1, "해당 대륙이 존재하지 않습니다.");
		}
		if (!continent.isNationTerritory)
		{
			throw new CommandHandleException(1, "해당 대륙은 국가대륙이 아닙니다.");
		}
		NationInstance nationInst = Cache.instance.GetNationInstance(nation.id);
		if (nationInst == null)
		{
			throw new CommandHandleException(1, "해당 국가인스턴스가 존재하지 않습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst != null && !nationWarInst.IsNationWarJoinEnabled(m_myHero))
		{
			Resource res = Resource.instance;
			m_myHero.placeEntranceParam = new ContinentSaftyAreaEnterParam(res.GetContinent(res.saftyRevivalContinentId), m_myHero.nationId, res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation(), currentTime);
			throw new CommandHandleException(101, "해당 국가가 국가전 진행중이므로 입장할 수 없습니다.");
		}
		NationContinentInstance targetNationContinentInst = nationInst.GetContinentInstance(continent.id);
		if (targetNationContinentInst == null)
		{
			throw new CommandHandleException(1, "해당 국가대륙인스턴스가 존재하지 않습니다.");
		}
		lock (targetNationContinentInst.syncObject)
		{
			m_myHero.SetPositionAndRotation(Resource.instance.SelectNationTransmissionExitPosition(), Resource.instance.SelectNationTransmissionExitRotationY());
			targetNationContinentInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			ContinentEnterForNationTransmissionResponseBody resBody = new ContinentEnterForNationTransmissionResponseBody();
			List<Sector> interestSectors = targetNationContinentInst.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetNationContinentInst.instanceId, targetNationContinentInst.continent.id, targetNationContinentInst.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, currentTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			SendResponseOK(resBody);
		}
	}
}
public class ContinentEnterForNationTransmissionParam : PlaceEntranceParam
{
	private Nation m_nation;

	private Continent m_continent;

	public Nation nation => m_nation;

	public Continent continent => m_continent;

	public ContinentEnterForNationTransmissionParam(Nation nation, Continent continent)
	{
		m_nation = nation;
		m_continent = continent;
	}
}
