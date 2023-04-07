using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class StoryDungeonReviveCommandHandler : InGameCommandHandler<StoryDungeonReviveCommandBody, StoryDungeonReviveResponseBody>
{
	public const short kResult_NotDead = 101;

	public const short kResult_DelayTimeNotElapsed = 102;

	public const short kResult_NotStatusPlaying = 103;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is StoryDungeonInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (currentPlace.status != 2)
		{
			throw new CommandHandleException(103, "현재 상태에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태가 아닙니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		float fElapsedTime = (float)(currentTime - m_myHero.lastDamageTime).TotalSeconds;
		if (!currentPlace.storyDungeon.IsSafeRevivalWaitingTimeElapsed(fElapsedTime))
		{
			throw new CommandHandleException(102, "부활지연시간이 경과되지 않았습니다. fElapsedTime = " + fElapsedTime);
		}
		m_myHero.Revive(bSendEvent: false);
		DateTime currentDate = currentTime.Date;
		m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, currentDate, nPaidImmediateRevivalDailyCount));
		dbWork.Schedule();
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
		StoryDungeonReviveResponseBody resBody = new StoryDungeonReviveResponseBody();
		resBody.hp = m_myHero.hp;
		resBody.date = (DateTime)currentDate;
		resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
		SendResponseOK(resBody);
	}
}
