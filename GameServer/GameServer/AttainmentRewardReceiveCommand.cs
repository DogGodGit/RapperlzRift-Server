using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class AttainmentRewardReceiveCommandHandler : InGameCommandHandler<AttainmentRewardReceiveCommandBody, AttainmentRewardReceiveResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_NotCompletedRequiredMainQuest = 102;

	public const short kResult_NotEnoughInventory = 103;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<HeroMainGear> m_addedMainGears = new List<HeroMainGear>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nEntryNo = m_body.entryNo;
		if (nEntryNo <= 0)
		{
			throw new CommandHandleException(1, "항목번호가 유효하지 않습니다. nEntryNo = " + nEntryNo);
		}
		if (nEntryNo != m_myHero.rewardedAttainmentEntryNo + 1)
		{
			throw new CommandHandleException(1, "보상받을 도달항목번호가 아닙니다. nEntryNo = " + nEntryNo);
		}
		AttainmentEntry attainmentEntry = Resource.instance.GetAttainmentEntry(nEntryNo);
		if (attainmentEntry == null)
		{
			throw new CommandHandleException(1, "도달항목이 존재하지 않습니다. nEntryNo = " + nEntryNo);
		}
		if (attainmentEntry.type == AttainmentEntryType.HeroLevel)
		{
			if (attainmentEntry.requiredHeroLevel > m_myHero.level)
			{
				throw new CommandHandleException(101, "영웅레벨이 부족합니다.");
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(attainmentEntry.requiredMainQuestNo))
		{
			throw new CommandHandleException(102, "필요한 메인퀘스트를 완료하지 않았습니다.");
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		int nEmptySlotCount = m_myHero.emptyInventorySlotCount;
		foreach (AttainmentEntryReward reward2 in attainmentEntry.rewards)
		{
			switch (reward2.type)
			{
			case AttainmentEntryRewardType.MainGear:
				if (nEmptySlotCount < 1)
				{
					throw new CommandHandleException(103, "인벤토리 슬롯이 부족합니다.");
				}
				nEmptySlotCount--;
				break;
			case AttainmentEntryRewardType.Item:
			{
				ItemReward itemReward2 = reward2.itemReward;
				resultItemCollection.AddResultItemCount(itemReward2.item, itemReward2.owned, itemReward2.count);
				break;
			}
			}
		}
		if (!m_myHero.IsAvailableInventory(resultItemCollection, nEmptySlotCount))
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		foreach (AttainmentEntryReward reward in attainmentEntry.rewards)
		{
			switch (reward.type)
			{
			case AttainmentEntryRewardType.MainGear:
			{
				MainGear rewardMainGear = reward.mainGear;
				int nEnchantLevel = 0;
				bool bOwned = reward.mainGearOwned;
				HeroMainGear heroMainGear = new HeroMainGear(m_myHero);
				heroMainGear.Init(rewardMainGear, nEnchantLevel, bOwned, m_currentTime);
				m_myHero.AddMainGear(heroMainGear, bInit: false, m_currentTime);
				InventorySlot emptySlot = m_myHero.GetEmptyInventorySlot();
				emptySlot.Place(heroMainGear);
				m_changedInventorySlots.Add(emptySlot);
				m_addedMainGears.Add(heroMainGear);
				break;
			}
			case AttainmentEntryRewardType.Item:
			{
				ItemReward itemReward = reward.itemReward;
				m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
				break;
			}
			}
		}
		m_myHero.rewardedAttainmentEntryNo = nEntryNo;
		SaveToDB(resultItemCollection);
		AttainmentRewardReceiveResponseBody resBody = new AttainmentRewardReceiveResponseBody();
		resBody.addedMainGears = HeroMainGear.ToPDFullHeroMainGears(m_addedMainGears).ToArray();
		resBody.maxAcquisitionMainGearGrade = m_myHero.maxAcquisitionMainGearGrade;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(ResultItemCollection resultItemCollection)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (HeroMainGear mainGear in m_addedMainGears)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroMainGear(mainGear));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_AttainmentEntryReawrd(m_myHero.id, m_myHero.rewardedAttainmentEntryNo));
		dbWork.Schedule();
		SaveToDB_AddAttainmentEntryRewardLog(resultItemCollection);
	}

	private void SaveToDB_AddAttainmentEntryRewardLog(ResultItemCollection resultItemCollection)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddAttainmentEntryRewardLog(logId, m_myHero.id, m_myHero.rewardedAttainmentEntryNo, m_currentTime));
			foreach (HeroMainGear mainGear in m_addedMainGears)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddAttainmentEntryRewardDetailLog(Guid.NewGuid(), logId, 1, mainGear.id, 0, 0, bItemOwned: false, 0));
			}
			foreach (ResultItem item in resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddAttainmentEntryRewardDetailLog(Guid.NewGuid(), logId, 2, Guid.Empty, item.item.id, item.count, item.owned, 0));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
