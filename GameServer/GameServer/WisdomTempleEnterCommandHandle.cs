using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WisdomTempleEnterCommandHandler : InGameCommandHandler<WisdomTempleEnterCommandBody, WisdomTempleEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is WisdomTempleEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		DateTime enterDate = enterTime.Date;
		Resource res = Resource.instance;
		WisdomTemple wisdomTemple = res.wisdomTemple;
		m_myHero.UseStamina(wisdomTemple.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterDate, nPaidImmediateRevivalDailyCount);
		WisdomTempleInstance wisdomTempleInst = new WisdomTempleInstance();
		lock (wisdomTempleInst.syncObject)
		{
			wisdomTempleInst.Init();
			Cache.instance.AddPlace(wisdomTempleInst);
			m_myHero.SetPositionAndRotation(wisdomTemple.startPosition, wisdomTemple.startYRotation);
			wisdomTempleInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			wisdomTempleInst.Start(enterTime);
			WisdomTempleEnterResponseBody resBody = new WisdomTempleEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.placeInstanceId = (Guid)wisdomTempleInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.dailyWisdomTemplePlayCount.value;
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessTodayTask(28, enterDate);
			m_myHero.ProcessRetrievalProgressCount(12, enterDate);
			m_myHero.ProcessMainQuestForContent(17);
			m_myHero.ProcessSubQuestForContent(17);
		}
	}

	private void SaveToDB(DateTime date, int nPaidImmediateRevivalDailyCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, date, nPaidImmediateRevivalDailyCount));
		dbWork.Schedule();
	}
}
