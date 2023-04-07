using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class DailyQuestCompleteCommandHandler : InGameCommandHandler<DailyQuestCompleteCommandBody, DailyQuestCompleteResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	public const short kResult_NotMissionCompleted = 102;

	private HeroDailyQuest m_oldHeroDailyQuest;

	private HeroDailyQuest m_newHeroDailyQuest;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private int m_nOldLevel;

	private long m_lnExpReward;

	private int m_nVipPointReawrd;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid questId = (Guid)m_body.questId;
		if (questId == Guid.Empty)
		{
			throw new CommandHandleException(1, "퀘스트ID가 유효하지 않습니다. questId = " + questId);
		}
		m_oldHeroDailyQuest = m_myHero.GetDailyQuest(questId);
		if (m_oldHeroDailyQuest == null)
		{
			throw new CommandHandleException(1, "영웅일일퀘스트가 존재하지 않습니다. questId = " + questId);
		}
		if (!m_oldHeroDailyQuest.isAccepted)
		{
			throw new CommandHandleException(1, "현재 영웅일일퀘스트는 진행상태가 아닙니다.");
		}
		if (!m_oldHeroDailyQuest.IsMissionCompleted(m_currentTime))
		{
			throw new CommandHandleException(102, "미션을 완료하지 못했습니다.");
		}
		DailyQuestMission dailyQuestMission = m_oldHeroDailyQuest.mission;
		DailyQuestGrade dailyQuestGrade = dailyQuestMission.grade;
		DailyQuestReward dailyQuestReward = Resource.instance.dailyQuest.GetReward(m_myHero.level);
		m_nOldLevel = m_myHero.level;
		m_lnExpReward = dailyQuestReward.expRewardValue;
		m_nVipPointReawrd = dailyQuestGrade.rewardVipPoint;
		ItemReward itemReawrd = dailyQuestGrade.itemReward;
		if (itemReawrd != null && m_myHero.GetInventoryAvailableSpace(itemReawrd.item, itemReawrd.owned) < itemReawrd.count)
		{
			throw new CommandHandleException(101, "인벤토리가 부족합니다.");
		}
		m_lnExpReward = (long)Math.Floor((float)m_lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
		m_myHero.AddExp(m_lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_myHero.AddVipPoint(0, m_nVipPointReawrd);
		if (itemReawrd != null)
		{
			m_myHero.AddItem(itemReawrd.item, itemReawrd.owned, itemReawrd.count, m_changedInventorySlots);
		}
		m_newHeroDailyQuest = m_myHero.CreateHeroDailyQuest(m_oldHeroDailyQuest.slotIndex);
		m_myHero.SetDailyQuest(m_newHeroDailyQuest);
		SaveToDB();
		SaveToLogDB(itemReawrd);
		DailyQuestCompleteResponseBody resBody = new DailyQuestCompleteResponseBody();
		resBody.vipPoint = m_myHero.totalVipPoint;
		resBody.acquiredExp = m_lnExpReward;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlot = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.addedDailyQuest = m_newHeroDailyQuest.ToPDHeroDailyQuest(m_currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_VipPoint(m_myHero.id, m_myHero.vipPoint));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroDailyQuest_Complete(m_oldHeroDailyQuest.id, m_currentTime));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroDailyQuest(m_newHeroDailyQuest.id, m_newHeroDailyQuest.hero.id, m_newHeroDailyQuest.slotIndex, m_newHeroDailyQuest.mission.id, m_currentTime));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(ItemReward rewardItem)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestCreationLog(m_newHeroDailyQuest.id, m_newHeroDailyQuest.hero.id, m_newHeroDailyQuest.slotIndex, m_newHeroDailyQuest.mission.id, m_currentTime));
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_oldHeroDailyQuest.id, m_nOldLevel, m_nVipPointReawrd, m_lnExpReward, rewardItem.item.id, rewardItem.owned, rewardItem.count, m_newHeroDailyQuest.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
