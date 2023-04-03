using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AnkouTombEnterCommandHandler : InGameCommandHandler<AnkouTombEnterCommandBody, AnkouTombEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private AnkouTombInstance m_ankouTombInstance;

	private DateValuePair<int> m_dailyAnkouTombPlayCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is AnkouTombEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.ankouTombInstanceId;
		m_ankouTombInstance = Cache.instance.GetPlace(instanceId) as AnkouTombInstance;
		if (m_ankouTombInstance == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_ankouTombInstance.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		AnkouTomb ankouTomb = Resource.instance.ankouTomb;
		m_myHero.UseStamina(ankouTomb.requiredStamina, m_dungeonCreationTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_ankouTombInstance.syncObject)
		{
			m_myHero.SetPositionAndRotation(ankouTomb.SelectStartPosition(), ankouTomb.SelectStartRotationY());
			m_ankouTombInstance.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			m_myHero.RefreshDailyAnkouTombPlayCount(dungeonCreationDate);
			m_dailyAnkouTombPlayCount = m_myHero.dailyAnkouTombPlayCount;
			m_dailyAnkouTombPlayCount.value++;
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SaveToDB_Log();
			AnkouTombEnterResponseBody resBody = new AnkouTombEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_ankouTombInstance.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_ankouTombInstance.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_ankouTombInstance.GetRemainingLimitTime(currentTime);
			resBody.waveNo = m_ankouTombInstance.waveNo;
			resBody.heroes = m_ankouTombInstance.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_ankouTombInstance.GetPDMonsterInstances(currentTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_dailyAnkouTombPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_AnkouTombPlay(m_myHero.id, m_dungeonCreationTime.Date, m_dailyAnkouTombPlayCount.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddAnkouTombMemberLog(m_ankouTombInstance.instanceId, m_myHero.id, m_myHero.level));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
