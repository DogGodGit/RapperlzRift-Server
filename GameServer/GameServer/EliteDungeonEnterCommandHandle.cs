using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class EliteDungeonEnterCommandHandler : InGameCommandHandler<EliteDungeonEnterCommandBody, EliteDungeonEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is EliteDungeonEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		EliteMonsterMaster master = param.master;
		if (master == null)
		{
			throw new CommandHandleException(1, "해당 정예몬스터마스터가 존재하지 않습니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		EliteDungeon eliteDungeon = Resource.instance.eliteDungeon;
		m_myHero.UseStamina(eliteDungeon.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterTime.Date);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterTime.Date, nPaidImmediateRevivalDailyCount);
		EliteDungeonInstance eliteDungeonInst = new EliteDungeonInstance();
		lock (eliteDungeonInst.syncObject)
		{
			eliteDungeonInst.Init(master, enterTime);
			Cache.instance.AddPlace(eliteDungeonInst);
			m_myHero.SetPositionAndRotation(eliteDungeon.startPosition, eliteDungeon.startYRotation);
			eliteDungeonInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			eliteDungeonInst.Start(enterTime);
			EliteDungeonEnterResponseBody resBody = new EliteDungeonEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.placeInstanceId = (Guid)eliteDungeonInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.monsters = eliteDungeonInst.GetPDMonsterInstances<PDEliteDungeonMonsterInstance>(enterTime).ToArray();
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.dailyEliteDungeonPlayCount.value;
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
