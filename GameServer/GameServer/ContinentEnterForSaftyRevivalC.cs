using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ContinentEnterForSaftyRevivalCommandHandler : InGameCommandHandler<ContinentEnterForSaftyRevivalCommandBody, ContinentEnterForSaftyRevivalResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentEnterForSaftyRevivalParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		Continent targetContinent = param.continent;
		int nTargetNationId = param.nationId;
		Vector3 targetPosition = param.position;
		float fTargetRotationY = param.rotationY;
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
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(currentTime, nPaidImmediateRevivalDailyCount);
		lock (targetPlace.syncObject)
		{
			targetPlace.Enter(m_myHero, currentTime, bIsRevivalEnter: true);
			ContinentEnterForSaftyRevivalResponseBody resBody = new ContinentEnterForSaftyRevivalResponseBody();
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, currentTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			resBody.hp = m_myHero.hp;
			resBody.date = (DateTime)currentDate;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(DateTimeOffset currentTime, int nPaidImmediateRevivalDailyCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, currentTime.Date, nPaidImmediateRevivalDailyCount));
		dbWork.Schedule();
		SaveToDB_AddHeroRevivallog(currentTime);
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
