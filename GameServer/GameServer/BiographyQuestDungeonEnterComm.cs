using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BiographyQuestDungeonEnterCommandHandler : InGameCommandHandler<BiographyQuestDungeonEnterCommandBody, BiographyQuestDungeonEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is BiographyQuestDungeonEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		BiographyQuestDungeon dungeon = param.dungeon;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		BiographyQuestDungeonInstance biographyQuestDungeonInst = new BiographyQuestDungeonInstance();
		lock (biographyQuestDungeonInst.syncObject)
		{
			biographyQuestDungeonInst.Init(dungeon);
			Cache.instance.AddPlace(biographyQuestDungeonInst);
			m_myHero.SetPositionAndRotation(dungeon.SelectStartPosition(), dungeon.startRotationY);
			biographyQuestDungeonInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			biographyQuestDungeonInst.Start();
			DateTime currentDate = currentTime.Date;
			m_myHero.ClearPaidImmediateRevivalDailyCount(currentDate);
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(currentDate, nPaidImmediateRevivalDailyCount);
			BiographyQuestDungeonEnterResponseBody resBody = new BiographyQuestDungeonEnterResponseBody();
			resBody.placeInstanceId = (Guid)biographyQuestDungeonInst.instanceId;
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
