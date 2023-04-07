using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class MainQuestDungeonEnterCommandHandler : InGameCommandHandler<MainQuestDungeonEnterCommandBody, MainQuestDungeonEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is MainQuestDungeonEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		MainQuestDungeon dungeon = param.dungeon;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		MainQuestDungeonInstance mainQuestDungeonInst = new MainQuestDungeonInstance();
		lock (mainQuestDungeonInst.syncObject)
		{
			mainQuestDungeonInst.Init(dungeon);
			Cache.instance.AddPlace(mainQuestDungeonInst);
			m_myHero.SetPositionAndRotation(dungeon.SelectStartPosition(), dungeon.startYRotation);
			mainQuestDungeonInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			mainQuestDungeonInst.Start();
			DateTime currentDate = currentTime.Date;
			m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(currentDate, nPaidImmediateRevivalDailyCount);
			MainQuestDungeonEnterResponseBody resBody = new MainQuestDungeonEnterResponseBody();
			resBody.placeInstanceId = (Guid)mainQuestDungeonInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.date = (DateTime)currentDate;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
