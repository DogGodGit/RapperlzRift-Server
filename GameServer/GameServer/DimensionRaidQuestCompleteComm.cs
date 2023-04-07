using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class DimensionRaidQuestCompleteCommandHandler : InGameCommandHandler<DimensionRaidQuestCompleteCommandBody, DimensionRaidQuestCompleteResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private HeroDimensionRaidQuest m_heroQuest;

	private long m_lnRewardExp;

	private int m_nRewardExploitPoint;

	private int m_nAcquiredExploitPoint;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		m_heroQuest = m_myHero.dimensionRaidQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		if (!m_heroQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(1, "모든 단계를 완료하지 않았습니다.");
		}
		DimensionRaidQuest quest = Resource.instance.dimensionRaidQuest;
		Npc questNpc = quest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_myHero.dimensionRaidQuest = null;
		DimensionRaidQuestReward reward = quest.GetReward(m_myHero.level);
		if (reward != null)
		{
			m_lnRewardExp = reward.expRewardValue;
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			m_nRewardExploitPoint = reward.exploitPointRewardValue;
			m_nAcquiredExploitPoint = m_myHero.AddExploitPoint(m_nRewardExploitPoint, m_currentTime, bSaveToDB: false);
			m_itemReward = reward.itemReward;
			if (m_itemReward != null)
			{
				int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
				if (nRemainingCount > 0)
				{
					m_mail = Mail.Create("MAIL_NAME_00012", "MAIL_DESC_00012", m_currentTime);
					m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
					m_myHero.AddMail(m_mail, bSendEvent: true);
				}
			}
		}
		SaveToDB();
		SaveToGameLogDB();
		DimensionRaidQuestCompleteResponseBody resBody = new DimensionRaidQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnRewardExp;
		resBody.acquiredExploitPoint = m_nAcquiredExploitPoint;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.exploitPoint = m_myHero.exploitPoint;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.date = (DateTime)m_currentDate;
		resBody.dailyExploitPoint = m_myHero.dailyExploitPoint.value;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateDimensionRaidQuest_Status(m_heroQuest.id, 1, m_currentTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nItemId = 0;
			int nItemCount = 0;
			bool bItemOwned = false;
			if (m_itemReward != null)
			{
				nItemId = m_itemReward.item.id;
				nItemCount = m_itemReward.count;
				bItemOwned = m_itemReward.owned;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddDimensionRaidQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_heroQuest.id, m_nRewardExploitPoint, m_nAcquiredExploitPoint, m_lnRewardExp, nItemId, nItemCount, bItemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
