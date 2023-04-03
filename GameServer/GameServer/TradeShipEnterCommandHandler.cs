using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TradeShipEnterCommandHandler : InGameCommandHandler<TradeShipEnterCommandBody, TradeShipEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private TradeShipInstance m_tradeShipInstance;

	private DateValuePair<int> m_dailyTradeShipPlayCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is TradeShipEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.tradeShipInstanceId;
		m_tradeShipInstance = Cache.instance.GetPlace(instanceId) as TradeShipInstance;
		if (m_tradeShipInstance == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_tradeShipInstance.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		TradeShip tradeShip = Resource.instance.tradeShip;
		m_myHero.UseStamina(tradeShip.requiredStamina, m_dungeonCreationTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_tradeShipInstance.syncObject)
		{
			m_myHero.SetPositionAndRotation(tradeShip.SelectStartPosition(), tradeShip.SelectStartRotationY());
			m_tradeShipInstance.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			m_myHero.RefreshDailyTradeShipPlayCount(dungeonCreationDate);
			m_dailyTradeShipPlayCount = m_myHero.dailyTradeShipPlayCount;
			m_dailyTradeShipPlayCount.value++;
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SaveToDB_Log();
			TradeShipEnterResponseBody resBody = new TradeShipEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_tradeShipInstance.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_tradeShipInstance.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_tradeShipInstance.GetRemainingLimitTime(currentTime);
			resBody.stepNo = m_tradeShipInstance.stepNo;
			resBody.heroes = m_tradeShipInstance.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_tradeShipInstance.GetPDMonsterInstances(currentTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_dailyTradeShipPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_TradeShipPlay(m_myHero.id, m_dungeonCreationTime.Date, m_dailyTradeShipPlayCount.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddTradeShipMemberLog(m_tradeShipInstance.instanceId, m_myHero.id, m_myHero.level));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
