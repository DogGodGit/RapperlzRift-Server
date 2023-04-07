using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class StoryDungeonEnterCommandHandler : InGameCommandHandler<StoryDungeonEnterCommandBody, StoryDungeonEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is StoryDungeonEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령엽니다.");
		}
		StoryDungeonDifficulty difficulty = param.difficulty;
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도가 존재하지 않습니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		m_myHero.UseStamina(difficulty.storyDungeon.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterTime.Date);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterTime.Date, nPaidImmediateRevivalDailyCount);
		StoryDungeonInstance storyDungeonInst = new StoryDungeonInstance();
		lock (storyDungeonInst.syncObject)
		{
			storyDungeonInst.Init(difficulty);
			Cache.instance.AddPlace(storyDungeonInst);
			StoryDungeon storyDungeon = difficulty.storyDungeon;
			m_myHero.SetPositionAndRotation(storyDungeon.SelectPosition(), storyDungeon.startYRotation);
			storyDungeonInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			storyDungeonInst.Start(enterTime);
			StoryDungeonEnterResponseBody resBody = new StoryDungeonEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.placeInstanceId = (Guid)storyDungeonInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.GetOrCreateStoryDungeonPlay(storyDungeon.no, enterTime.Date).enterCount;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessTodayTask(10, enterTime.Date);
			m_myHero.IncreaseOpen7DayEventProgressCount(2);
			m_myHero.ProcessMainQuestForContent(1);
			m_myHero.ProcessSubQuestForContent(1);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
