using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class ContinentEnterForNationCallTransmissionCommandHandler : InGameCommandHandler<ContinentEnterForNationCallTransmissionCommandBody, ContinentEnterForNationCallTransmissionResponseBody>
{
	public const short kResult_ProgressingNationWarByTargetNation = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace is ContinentInstance)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentEnterForNationCallTransmissionParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		Continent targetContinent = param.continent;
		int nTargetNationId = param.nationId;
		Vector3 targetPosition = param.position;
		float fTargetRotationY = param.rotationY;
		DateTimeOffset enterTime = param.enterTime;
		NationInstance nationInst = Cache.instance.GetNationInstance(nTargetNationId);
		if (nationInst == null)
		{
			throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. nTargetNationId = " + nTargetNationId);
		}
		NationWarInstance nationWarInst = nationInst.nationWarInst;
		if (nationWarInst != null && !nationWarInst.IsNationWarJoinEnabled(m_myHero))
		{
			Resource res = Resource.instance;
			m_myHero.placeEntranceParam = new ContinentSaftyAreaEnterParam(res.GetContinent(res.saftyRevivalContinentId), m_myHero.nationId, res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation(), enterTime);
			throw new CommandHandleException(101, "해당 국가가 국가전 진행중이므로 입장할 수 없습니다.");
		}
		ContinentInstance targetPlace = null;
		targetPlace = ((!targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(targetContinent.id)) : ((ContinentInstance)nationInst.GetContinentInstance(targetContinent.id)));
		if (targetPlace == null)
		{
			throw new CommandHandleException(1, "해당 장소가 존재하지 않습니다.");
		}
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(targetPosition, fTargetRotationY);
			targetPlace.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			ContinentEnterForNationCallTransmissionResponseBody resBody = new ContinentEnterForNationCallTransmissionResponseBody();
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, enterTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, enterTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, enterTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			SendResponseOK(resBody);
		}
	}
}
public class ContinentEnterForNationCallTransmissionParam : PlaceEntranceParam
{
	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public DateTimeOffset enterTime => m_enterTime;

	public ContinentEnterForNationCallTransmissionParam(Continent continent, int nNationId, Vector3 position, float fRotationY, DateTimeOffset enterTime)
	{
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_enterTime = enterTime;
	}
}
