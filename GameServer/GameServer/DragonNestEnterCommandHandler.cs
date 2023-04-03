using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class DragonNestEnterCommandHandler : InGameCommandHandler<DragonNestEnterCommandBody, DragonNestEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private DragonNestInstance m_dragonNestInst;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is DragonNestEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.dragonNestInstanceId;
		m_dragonNestInst = Cache.instance.GetPlace(instanceId) as DragonNestInstance;
		if (m_dragonNestInst == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_dragonNestInst.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		DragonNest dragonNest = Resource.instance.dragonNest;
		m_myHero.UseItem(dragonNest.enterRequiredItemId, bFirstUseOwn: true, 1, m_changedInventorySlots);
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_dragonNestInst.syncObject)
		{
			m_myHero.SetPositionAndRotation(dragonNest.SelectStartPosition(), dragonNest.SelectStartRotationY());
			m_dragonNestInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SaveToDB_Log();
			DragonNestEnterResponseBody resBody = new DragonNestEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_dragonNestInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_dragonNestInst.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_dragonNestInst.GetRemainingLimitTime(currentTime);
			resBody.stepNo = m_dragonNestInst.stepNo;
			resBody.heroes = m_dragonNestInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_dragonNestInst.GetPDMonsterInstances(currentTime).ToArray();
			resBody.trapEffectHeroes = (Guid[])(object)m_dragonNestInst.GetTrapEffectHeroes().ToArray();
			resBody.hp = m_myHero.hp;
			resBody.changedInventorySlot = ((m_changedInventorySlots.Count > 0) ? m_changedInventorySlots[0].ToPDInventorySlot() : null);
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			m_myHero.ProcessMainQuestForContent(22);
			m_myHero.ProcessSubQuestForContent(22);
		}
	}

	private void SaveToDB(int nPaidImmediateRevivalCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestMemberLog(m_dragonNestInst.instanceId, m_myHero.id, m_myHero.level));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
