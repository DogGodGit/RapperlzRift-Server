using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class JobChangeQuestCompleteCommandHandler : InGameCommandHandler<JobChangeQuestCompleteCommandBody, JobChangeQuestCompleteResponseBody>
{
	public const short kResult_NotProgressedQuest = 101;

	public const short kResult_NotCompletedQuestObjective = 102;

	public const short kResult_UnableInteractionPositionWithQuestNPC = 103;

	public const short kResult_NotEnoughInventory = 104;

	private HeroJobChangeQuest m_heroJobChangeQuest;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_curretTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_curretTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid instanceId = (Guid)m_body.instanceId;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다. instanceId = " + instanceId);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		m_heroJobChangeQuest = m_myHero.jobChangeQuest;
		if (m_heroJobChangeQuest == null)
		{
			throw new CommandHandleException(1, "영웅전직퀘스트가 존재하지 않습니다. instanceId = " + instanceId);
		}
		if (m_heroJobChangeQuest.instanceId != instanceId)
		{
			throw new CommandHandleException(1, "진행중인 인스턴스ID가 아닙니다. instanceId = " + instanceId);
		}
		if (!m_heroJobChangeQuest.isAccepted)
		{
			throw new CommandHandleException(101, "영웅전직퀘스트가 진행중인 상태가 아닙니다.");
		}
		if (!m_heroJobChangeQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(102, "목표가 완료되지 않았습니다.");
		}
		JobChangeQuest jobChangeQuest = m_heroJobChangeQuest.quest;
		Npc questNpc = jobChangeQuest.questNpc;
		if (questNpc == null)
		{
			throw new CommandHandleException(1, "퀘스트NPC가 존재하지 않습니다.");
		}
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 NPC입니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "해당 NPC와 상호작용할 수 없는 거리입니다.");
		}
		m_itemReward = jobChangeQuest.completionItemReward;
		if (m_itemReward != null)
		{
			if (m_myHero.GetInventoryAvailableSpace(m_itemReward.item, m_itemReward.owned) < m_itemReward.count)
			{
				throw new CommandHandleException(104, "인벤토리가 부족합니다.");
			}
			m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
		}
		m_heroJobChangeQuest.Complete();
		SaveToDB();
		SaveToLogDB();
		JobChangeQuestCompleteResponseBody resBody = new JobChangeQuestCompleteResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroJobChangeQuest_Status(m_heroJobChangeQuest.instanceId, 1, m_curretTime));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nItemId = 0;
			bool bItemOwned = false;
			int nItemCount = 0;
			if (m_itemReward != null)
			{
				nItemId = m_itemReward.item.id;
				bItemOwned = m_itemReward.owned;
				nItemCount = m_itemReward.count;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroJobChangeQuestRewardLog(m_heroJobChangeQuest.instanceId, m_myHero.id, nItemId, bItemOwned, nItemCount, m_curretTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
