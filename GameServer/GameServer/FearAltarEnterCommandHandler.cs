using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FearAltarEnterCommandHandler : InGameCommandHandler<FearAltarEnterCommandBody, FearAltarEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private FearAltarInstance m_fearAltarInst;

	private DateValuePair<int> m_dailyFearAltarPlayCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is FearAltarEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.fearAltarInstanceId;
		m_fearAltarInst = Cache.instance.GetPlace(instanceId) as FearAltarInstance;
		if (m_fearAltarInst == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_fearAltarInst.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		FearAltar fearAltar = Resource.instance.fearAltar;
		m_myHero.UseStamina(fearAltar.requiredStamina, m_dungeonCreationTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		FearAltarStage stage = m_fearAltarInst.stage;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_fearAltarInst.syncObject)
		{
			m_myHero.SetPositionAndRotation(stage.SelectStartPosition(), stage.SelectStartRotationY());
			m_fearAltarInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			m_myHero.RefreshDailyFearAltarPlayCount(dungeonCreationDate);
			m_dailyFearAltarPlayCount = m_myHero.dailyFearAltarPlayCount;
			m_dailyFearAltarPlayCount.value++;
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SaveToDB_Log();
			FearAltarEnterResponseBody resBody = new FearAltarEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_fearAltarInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_fearAltarInst.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_fearAltarInst.GetRemainingLimitTime(currentTime);
			resBody.waveNo = m_fearAltarInst.waveNo;
			resBody.heroes = m_fearAltarInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_fearAltarInst.GetPDMonsterInstances(currentTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_dailyFearAltarPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessTodayTask(2, dungeonCreationDate.Date);
			m_myHero.ProcessRetrievalProgressCount(3, dungeonCreationDate);
			m_myHero.ProcessMainQuestForContent(20);
			m_myHero.ProcessSubQuestForContent(20);
		}
	}

	private void SaveToDB(int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FearAltarPlay(m_myHero.id, m_dailyFearAltarPlayCount.date, m_dailyFearAltarPlayCount.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFearAltarMemberLog(m_fearAltarInst.instanceId, m_myHero.id));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
