using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AnkouTombReviveCommandHandler : InGameCommandHandler<AnkouTombReviveCommandBody, AnkouTombReviveResponseBody>
{
	public const short kResult_NotStatusPlaying = 101;

	public const short kResult_NotDead = 102;

	public const short kResult_DelayTimeNotElapsed = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is AnkouTombInstance currentPlace))
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
		m_currentTime = DateTimeUtil.currentTime;
		float fElapsedTime = (float)(m_currentTime - m_myHero.lastDamageTime).TotalSeconds;
		if (!currentPlace.ankouTomb.IsSafeRevivalWaitingTimeElapsed(fElapsedTime))
		{
			throw new CommandHandleException(103, "부활지연시간이 경과되지 않았습니다. fElapsedTime = " + fElapsedTime);
		}
		m_myHero.Revive(bSendEvent: false);
		DateTime currentDate = m_currentTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
		SaveToDB();
		SaveToDB_Log();
		AnkouTombReviveResponseBody resBody = new AnkouTombReviveResponseBody();
		resBody.hp = m_myHero.hp;
		resBody.date = (DateTime)currentDate;
		resBody.paidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_currentTime.Date, m_myHero.paidImmediateRevivalDailyCount.value));
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
			LogError(null, ex);
		}
	}
}
