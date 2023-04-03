using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GoldDungeonEnterCommandHandler : InGameCommandHandler<GoldDungeonEnterCommandBody, GoldDungeonEnterResponseBody>
{
	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is GoldDungeonEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		GoldDungeonDifficulty difficulty = param.difficulty;
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도가 존재하지 않습니다.");
		}
		DateTimeOffset enterTime = param.enterTime;
		m_myHero.UseStamina(difficulty.goldDungeon.requiredStamina, enterTime);
		m_myHero.ClearPaidImmediateRevivalDailyCount(enterTime.Date);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
		SaveToDB(enterTime.Date, nPaidImmediateRevivalDailyCount);
		GoldDungeonInstance goldDungeonInst = new GoldDungeonInstance();
		lock (goldDungeonInst.syncObject)
		{
			goldDungeonInst.Init(difficulty);
			Cache.instance.AddPlace(goldDungeonInst);
			GoldDungeon goldDungeon = difficulty.goldDungeon;
			m_myHero.SetPositionAndRotation(goldDungeon.startPosition, goldDungeon.startYRotation);
			goldDungeonInst.Enter(m_myHero, enterTime, bIsRevivalEnter: false);
			goldDungeonInst.Start(enterTime);
			GoldDungeonEnterResponseBody resBody = new GoldDungeonEnterResponseBody();
			resBody.date = (DateTime)enterTime.Date;
			resBody.placeInstanceId = (Guid)goldDungeonInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.hp = m_myHero.hp;
			resBody.stamina = m_myHero.stamina;
			resBody.playCount = m_myHero.dailyGoldDungeonPlayCount.value;
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
