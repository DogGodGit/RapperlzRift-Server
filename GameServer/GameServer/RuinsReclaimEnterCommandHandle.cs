using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimEnterCommandHandler : InGameCommandHandler<RuinsReclaimEnterCommandBody, RuinsReclaimEnterResponseBody>
{
	public const short kResult_EnterTimeout = 101;

	public const short kResult_NotEnoughItem = 102;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	private RuinsReclaimInstance m_ruinsReclaimInst;

	private bool m_bIsFreeEnter;

	private int m_nEnterRequiredItemId;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private DateValuePair<int> m_dailyRuinsReclaimFreePlayCount;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.currentPlace != null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!(m_myHero.placeEntranceParam is RuinsReclaimEnterParam param))
		{
			throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
		}
		m_dungeonCreationTime = param.dungeonCreationTime;
		DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
		Guid instanceId = param.ruinsReclaimInstanceId;
		m_ruinsReclaimInst = Cache.instance.GetPlace(instanceId) as RuinsReclaimInstance;
		if (m_ruinsReclaimInst == null)
		{
			throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
		}
		if (m_ruinsReclaimInst.isFinished)
		{
			throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
		}
		RuinsReclaim ruinsReclaim = m_ruinsReclaimInst.ruinsReclaim;
		m_nEnterRequiredItemId = ruinsReclaim.enterRequiredItemId;
		if (m_myHero.GetRuinsReclaimAvailableFreeEnterCount(dungeonCreationDate) <= 0)
		{
			if (m_myHero.GetItemCount(m_nEnterRequiredItemId) <= 0)
			{
				throw new CommandHandleException(102, "입장에 필요한 아이템이 부족합니다.");
			}
		}
		else
		{
			m_bIsFreeEnter = true;
		}
		if (!m_bIsFreeEnter)
		{
			m_myHero.UseItem(m_nEnterRequiredItemId, bFirstUseOwn: true, 1, m_changedInventorySlots);
		}
		m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
		m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		lock (m_ruinsReclaimInst.syncObject)
		{
			m_myHero.SetPositionAndRotation(ruinsReclaim.SelectStartPosition(), ruinsReclaim.SelectStartRotationY());
			m_ruinsReclaimInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
			m_myHero.RefreshDailyRuinsReclaimFreePlayCount(dungeonCreationDate);
			m_dailyRuinsReclaimFreePlayCount = m_myHero.dailyRuinsReclaimFreePlayCount;
			if (m_bIsFreeEnter)
			{
				m_dailyRuinsReclaimFreePlayCount.value++;
			}
			int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
			SaveToDB(nPaidImmediateRevivalDailyCount);
			SaveToDB_Log();
			RuinsReclaimEnterResponseBody resBody = new RuinsReclaimEnterResponseBody();
			resBody.date = (DateTime)dungeonCreationDate;
			resBody.placeInstanceId = (Guid)m_ruinsReclaimInst.instanceId;
			resBody.position = m_myHero.position;
			resBody.rotationY = m_myHero.rotationY;
			resBody.remainingStartTime = m_ruinsReclaimInst.GetRemainingStartTime(currentTime);
			resBody.remainingLimitTime = m_ruinsReclaimInst.GetRemainingLimitTime(currentTime);
			resBody.stepNo = m_ruinsReclaimInst.stepNo;
			resBody.waveNo = m_ruinsReclaimInst.waveNo;
			resBody.heroes = m_ruinsReclaimInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
			resBody.monsterInsts = m_ruinsReclaimInst.GetPDMonsterInstances(currentTime).ToArray();
			resBody.rewardObjectInsts = m_ruinsReclaimInst.GetPDRewardObjectInstnaces().ToArray();
			resBody.monsterTransformationCancelObjectInsts = m_ruinsReclaimInst.GetPDMonsterTransformationCancelObjectInstances().ToArray();
			resBody.monsterTransformationHeroes = (Guid[])(object)m_ruinsReclaimInst.GetMonsterTransformationHeroes().ToArray();
			resBody.hp = m_myHero.hp;
			resBody.freePlayCount = m_dailyRuinsReclaimFreePlayCount.value;
			resBody.changedInventorySlot = ((m_changedInventorySlots.Count > 0) ? m_changedInventorySlots[0].ToPDInventorySlot() : null);
			resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
			SendResponseOK(resBody);
			if (m_dailyRuinsReclaimFreePlayCount.value == 1)
			{
				m_myHero.ProcessTodayTask(29, dungeonCreationDate);
			}
			m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.RuinsReclaimDungeon, 1, currentTime);
			m_myHero.ProcessMainQuestForContent(18);
			m_myHero.ProcessSubQuestForContent(18);
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
		if (m_bIsFreeEnter)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RuinsReclaimFreePlay(m_myHero.id, m_dailyRuinsReclaimFreePlayCount.date, m_dailyRuinsReclaimFreePlayCount.value));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimMemberLog(m_ruinsReclaimInst.instanceId, base.hero.id, m_bIsFreeEnter ? 1 : 2, m_myHero.level));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
