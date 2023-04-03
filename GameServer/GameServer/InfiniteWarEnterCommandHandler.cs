using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class InfiniteWarEnterCommandHandler : InGameCommandHandler<InfiniteWarEnterCommandBody, InfiniteWarEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private InfiniteWarInstance m_infiniteWarInst;

	private DateValuePair<int> m_dailyInfiniteWarPlayCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is InfiniteWarEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.infiniteWarInstanceId;
		m_infiniteWarInst = Cache.instance.GetPlace(instanceId) as InfiniteWarInstance;
		if (m_infiniteWarInst == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_infiniteWarInst.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		InfiniteWar infiniteWar = Resource.instance.infiniteWar;
		m_myHero.UseStamina(infiniteWar.requiredStamina, m_dungeonCreationTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_infiniteWarInst.syncObject)
		{
			InfiniteWarStartPosition startPosition = infiniteWar.GetStartPosition(m_infiniteWarInst.heroEnterNo);
			m_myHero.SetPositionAndRotation(startPosition.SelectPosition(), startPosition.SelectRotationY());
			m_infiniteWarInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			m_myHero.RefreshDailyInfiniteWarPlayCount(dungeonCreationDate);
			m_dailyInfiniteWarPlayCount = m_myHero.dailyInfiniteWarPlayCount;
			m_dailyInfiniteWarPlayCount.value++;
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SaveToDB_Log();
			InfiniteWarEnterResponseBody resBody = new InfiniteWarEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_infiniteWarInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_infiniteWarInst.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_infiniteWarInst.GetRemainingLimitTime(currentTime);
			resBody.heroes = m_infiniteWarInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_infiniteWarInst.GetPDMonsterInstances<PDMonsterInstance>(currentTime).ToArray();
			resBody.buffBoxInsts = m_infiniteWarInst.GetPDBuffBoxInstances().ToArray();
			resBody.points = m_infiniteWarInst.GetPDInfiniteWarPoints().ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_dailyInfiniteWarPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_InfiniteWarPlay(m_myHero.id, m_dailyInfiniteWarPlayCount.date, m_dailyInfiniteWarPlayCount.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarMemberLog(m_infiniteWarInst.instanceId, m_myHero.id, base.hero.level));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
