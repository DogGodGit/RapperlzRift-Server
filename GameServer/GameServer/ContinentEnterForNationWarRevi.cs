using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ContinentEnterForNationWarReviveCommandHandler : InGameCommandHandler<ContinentEnterForNationWarReviveCommandBody, ContinentEnterForNationWarReviveResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentEnterForNationWarReviveParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		Continent targetContinent = param.continent;
		int nTargetNationId = param.nationId;
		Vector3 targetPosition = param.position;
		float fTargetRotationY = param.rotationY;
		DateTimeOffset revivalTime = param.revivalTime;
		ContinentInstance targetPlace = null;
		if (targetContinent.isNationTerritory)
		{
			NationInstance nationInst = Cache.instance.GetNationInstance(nTargetNationId);
			if (nationInst == null)
			{
				throw new CommandHandleException(1, "해당 국가가 존재하지 않습니다. nTargetNationId = " + nTargetNationId);
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
		m_myHero.SetPositionAndRotation(targetPosition, fTargetRotationY);
		m_myHero.Revive(bSendEvent: false);
		DateTime revivalDate = revivalTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(revivalDate);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(revivalTime, nPaidImmediateRevivalDailyCount);
		SaveToDB_AddHeroRevivallog(revivalTime);
		lock (targetPlace.syncObject)
		{
			targetPlace.Enter(m_myHero, revivalTime, bIsRevivalEnter: true);
			ContinentEnterForNationWarReviveResponseBody resBody = new ContinentEnterForNationWarReviveResponseBody();
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, revivalTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, revivalTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, revivalTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			resBody.hp = m_myHero.hp;
			resBody.date = (DateTime)revivalDate;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(DateTimeOffset currentTime, int nPaidImmediateRevivalDailyCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, currentTime.Date, nPaidImmediateRevivalDailyCount));
		dbWork.Schedule();
	}

	private void SaveToDB_AddHeroRevivallog(DateTimeOffset currentTime)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRevivalLog(Guid.NewGuid(), m_myHero.id, 1, 0, 0, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
public class ContinentEnterForNationWarReviveParam : PlaceEntranceParam
{
	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private DateTimeOffset m_revivalTime = DateTimeOffset.MinValue;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public DateTimeOffset revivalTime => m_revivalTime;

	public ContinentEnterForNationWarReviveParam(Continent continent, int nNationId, Vector3 position, float fRotationY, DateTimeOffset revivalTime)
	{
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_revivalTime = revivalTime;
	}
}
