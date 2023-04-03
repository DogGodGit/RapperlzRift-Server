using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ExpDungeonEnterCommandHandler : InGameCommandHandler<ExpDungeonEnterCommandBody, ExpDungeonEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is ExpDungeonEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		ExpDungeonDifficulty difficulty = param.difficulty;
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도가 존재하지 않습니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		DateTime enterDate = enterTime.Date;
		m_myHero.UseStamina(difficulty.expDungeon.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterTime.Date);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterTime.Date, nPaidImmediateRevivalDailyCount);
		ExpDungeonInstance expDungeonInst = new ExpDungeonInstance();
		lock (expDungeonInst.syncObject)
		{
			expDungeonInst.Init(difficulty);
			Cache.instance.AddPlace(expDungeonInst);
			ExpDungeon expDungeon = difficulty.expDungeon;
			m_myHero.SetPositionAndRotation(expDungeon.startPosition, expDungeon.startYRotation);
			expDungeonInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			expDungeonInst.Start(enterTime);
			ExpDungeonEnterResponseBody resBody = new ExpDungeonEnterResponseBody();
			resBody.date = (DateTime)enterDate;
			resBody.placeInstanceId = (Guid)expDungeonInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.dailyExpDungeonPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessSeriesMission(4);
			m_myHero.ProcessTodayMission(4, enterTime);
			m_myHero.ProcessTodayTask(1, enterDate);
			m_myHero.IncreaseOpen7DayEventProgressCount(4);
			m_myHero.ProcessRetrievalProgressCount(1, enterDate);
			m_myHero.ProcessMainQuestForContent(3);
			m_myHero.ProcessSubQuestForContent(3);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
