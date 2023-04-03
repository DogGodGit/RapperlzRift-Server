using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ContinentEnterForNationWarTransmissionCommandHandler : InGameCommandHandler<ContinentEnterForNationWarTransmissionCommandBody, ContinentEnterForNationWarTransmissionResponseBody>
{
	public const short kResult_NotEnoughtDia = 101;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	private DateValuePair<int> m_dailyNationWarFreeTransmissionCount;

	private DateValuePair<int> m_dailyNationWarPaidTransmissionCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ContinentEnterForNationWarTransmissionParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		Continent targetContinent = param.continent;
		int nTargetNationId = param.nationId;
		Vector3 targetPosition = param.position;
		float fTargetRotationY = param.rotationY;
		m_enterTime = param.enterTime;
		Guid declarationId = param.declarationId;
		int nTargetMonsterArrangeId = param.monsterArrangeId;
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
		NationWar nationWar = Resource.instance.nationWar;
		m_dailyNationWarFreeTransmissionCount = m_myHero.dailyNationWarFreeTransmissionCount;
		m_dailyNationWarPaidTransmissionCount = m_myHero.dailyNationWarPaidTransmissionCount;
		bool bIsFreeTransmission = true;
		int nPaidDia = 0;
		DateTime enterDate = m_enterTime.Date;
		m_myHero.RefreshDailyNationWarFreeTransmissionCount(enterDate);
		m_myHero.RefreshDailyNationWarPaidTransmissionCount(enterDate);
		m_dailyNationWarFreeTransmissionCount = m_myHero.dailyNationWarFreeTransmissionCount;
		m_dailyNationWarPaidTransmissionCount = m_myHero.dailyNationWarPaidTransmissionCount;
		if (m_dailyNationWarFreeTransmissionCount.value >= nationWar.freeTransmissionCount)
		{
			nPaidDia = nationWar.GetPaidTransmission(m_dailyNationWarPaidTransmissionCount.value + 1).requiredDia;
			if (m_myHero.dia < nPaidDia)
			{
				throw new CommandHandleException(101, "다이아가 부족합니다. myHeroDia = " + m_myHero.dia + ", nPaidDia = " + nPaidDia);
			}
			bIsFreeTransmission = false;
		}
		int nUsedOwnDia = 0;
		int nUsedUnOwnDia = 0;
		if (bIsFreeTransmission)
		{
			m_dailyNationWarFreeTransmissionCount.value++;
		}
		else
		{
			m_myHero.UseDia(nPaidDia, DateTimeUtil.currentTime, out nUsedOwnDia, out nUsedUnOwnDia);
			m_dailyNationWarPaidTransmissionCount.value++;
		}
		SaveToDB(bIsFreeTransmission, nUsedOwnDia, nUsedUnOwnDia);
		int nLogType = (bIsFreeTransmission ? 1 : 2);
		int nTransmissionCount = (bIsFreeTransmission ? m_dailyNationWarFreeTransmissionCount.value : m_dailyNationWarPaidTransmissionCount.value);
		SaveToDB_Log(declarationId, nLogType, nTargetMonsterArrangeId, nTransmissionCount, nUsedUnOwnDia, nUsedOwnDia);
		lock (targetPlace.syncObject)
		{
			m_myHero.SetPositionAndRotation(targetPosition, fTargetRotationY);
			targetPlace.Enter(m_myHero, m_enterTime, bIsRevivalEnter: false);
			ContinentEnterForNationWarTransmissionResponseBody resBody = new ContinentEnterForNationWarTransmissionResponseBody();
			List<Sector> interestSectors = targetPlace.GetInterestSectors(m_myHero.sector);
			resBody.entranceInfo = new PDContinentEntranceInfo((Guid)targetPlace.instanceId, targetPlace.continent.id, targetPlace.nationId, Sector.GetPDHeroes(interestSectors, m_myHero.id, m_enterTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(interestSectors, m_enterTime).ToArray(), Sector.GetPDContinentObjectInstances(interestSectors).ToArray(), Sector.GetPDCartInstances(interestSectors, 0L, m_enterTime).ToArray(), m_myHero.position, m_myHero.rotationY);
			resBody.date = (DateTime)enterDate;
			resBody.nationWarFreeTransmissionCount = m_dailyNationWarFreeTransmissionCount.value;
			resBody.nationWarPaidTransmissionCount = m_dailyNationWarPaidTransmissionCount.value;
			resBody.ownDIa = m_myHero.ownDia;
			resBody.unOwnDia = m_myHero.unOwnDia;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(bool bIsFreeTransmission, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myHero.account.id));
		if (bIsFreeTransmission)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWarFreeTransmissionDateCount(m_myHero.id, m_dailyNationWarFreeTransmissionCount.date, m_dailyNationWarFreeTransmissionCount.value));
		}
		else
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWarPaidTransmissionDateCount(m_myHero.id, m_dailyNationWarPaidTransmissionCount.date, m_dailyNationWarPaidTransmissionCount.value));
			if (nUsedOwnDia > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
			}
			if (nUsedUnOwnDia > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myHero.account));
			}
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log(Guid declarationId, int nLogType, int nMonsterArrangeId, int nTransmissionCount, int nUsedUnOwnDia, int nUsedOwnDia)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarHeroTransmissionLog(Guid.NewGuid(), declarationId, m_myHero.id, nLogType, nTransmissionCount, nMonsterArrangeId, nUsedUnOwnDia, nUsedOwnDia, m_enterTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
public class ContinentEnterForNationWarTransmissionParam : PlaceEntranceParam
{
	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	private Guid m_declarationId = Guid.Empty;

	private int m_nMonsterArrangeId;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public DateTimeOffset enterTime => m_enterTime;

	public Guid declarationId => m_declarationId;

	public int monsterArrangeId => m_nMonsterArrangeId;

	public ContinentEnterForNationWarTransmissionParam(Continent continent, int nNationId, Vector3 position, float fRotationY, DateTimeOffset enterTime, Guid declarationId, int nMonsterArrangeId)
	{
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_enterTime = enterTime;
		m_declarationId = declarationId;
		m_nMonsterArrangeId = nMonsterArrangeId;
	}
}
