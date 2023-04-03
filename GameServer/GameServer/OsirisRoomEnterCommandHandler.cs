using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class OsirisRoomEnterCommandHandler : InGameCommandHandler<OsirisRoomEnterCommandBody, OsirisRoomEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is OsirisRoomEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		OsirisRoomDifficulty difficulty = param.difficulty;
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도가 존재하지 않습니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		m_myHero.UseStamina(difficulty.osirisRoom.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterTime.Date);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterTime.Date, nPaidImmediateRevivalDailyCount);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		OsirisRoomInstance osirisRoomInst = new OsirisRoomInstance();
		lock (osirisRoomInst.syncObject)
		{
			osirisRoomInst.Init(difficulty);
			Cache.instance.AddPlace(osirisRoomInst);
			OsirisRoom osirisRoom = difficulty.osirisRoom;
			m_myHero.SetPositionAndRotation(osirisRoom.startPosition, osirisRoom.startYRotation);
			osirisRoomInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			osirisRoomInst.Start(enterTime);
			OsirisRoomEnterResponseBody resBody = new OsirisRoomEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.placeInstanceId = (Guid)osirisRoomInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.dailyOsirisRoomPlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessSeriesMission(3);
			m_myHero.ProcessTodayMission(3, enterTime);
			m_myHero.ProcessTodayTask(16, enterTime.Date);
			m_myHero.ProcessMainQuestForContent(2);
			m_myHero.ProcessSubQuestForContent(2);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}
}
