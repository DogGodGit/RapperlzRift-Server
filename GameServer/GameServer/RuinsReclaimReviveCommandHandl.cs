using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimReviveCommandHandler : InGameCommandHandler<RuinsReclaimReviveCommandBody, RuinsReclaimReviveResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_NotDead = 102;

	public const short kResult_DelayTimeNotElapsed = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is RuinsReclaimInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 2)
		{
			throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태가 아닙니다.");
		}
		DateTimeOffset m_currentTime = DateTimeUtil.currentTime;
		float fElapsedTime = (float)(m_currentTime - m_myHero.lastDamageTime).TotalSeconds;
		if (!currentPlace.ruinsReclaim.IsSafeRevivalWaitingTimeElapsed(fElapsedTime))
		{
			throw new CommandHandleException(103, "부활지연시간이 경과되지 않았습니다. fElapsedTime = " + fElapsedTime);
		}
		RuinsReclaimStep step = currentPlace.ruinsReclaim.GetStep(currentPlace.stepNo);
		RuinsReclaimRevivalPoint revivalPoint = step.revivalPoint;
		currentPlace.ChangeHeroPositionAndRotation(m_myHero, revivalPoint.SelectPosition(), revivalPoint.SelectRotationY(), bSendInterestTargetChangeEvent: true, m_currentTime);
		m_myHero.Revive(bSendEvent: true);
		m_currentDate = m_currentTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(m_currentDate);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(nPaidImmediateRevivalDailyCount);
		SaveToDB_Log();
		RuinsReclaimReviveResponseBody resBody = new RuinsReclaimReviveResponseBody();
		resBody.hp = m_myHero.hp;
		resBody.position = m_myHero.position;
		resBody.rotationY = m_myHero.rotationY;
		resBody.date = (DateTime)m_currentDate;
		resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nPaidImmediateRevivalDailyCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_currentDate, nPaidImmediateRevivalDailyCount));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRevivalLog(Guid.NewGuid(), m_myHero.id, 1, 0, 0, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
