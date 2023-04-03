using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuestCompleteCommandHandler : InGameCommandHandler<CreatureFarmQuestCompleteCommandBody, CreatureFarmQuestCompleteResponseBody>
{
	public const short kResult_AlreadyCompletedQuest = 101;

	public const short kResult_NotCompletedQuestObjective = 102;

	public const short kResult_UnableInteractionPositionWithCompletionNPC = 103;

	private HeroCreatureFarmQuest m_heroCreatureFarmQuest;

	private ResultItemCollection m_resultItemCollection;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
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
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		m_heroCreatureFarmQuest = m_myHero.creatureFarmQuest;
		if (m_heroCreatureFarmQuest == null)
		{
			throw new CommandHandleException(1, "영웅크리처농장퀘스트가 존재하지 않습니다.");
		}
		if (m_heroCreatureFarmQuest.instanceId != instanceId)
		{
			throw new CommandHandleException(1, "진행중인 영웅크리처농장퀘스트ID가 아닙니다. instanceId = " + instanceId);
		}
		if (m_heroCreatureFarmQuest.mission != null)
		{
			throw new CommandHandleException(102, "퀘스트 미션을 모두 완료하지 않았습니다.");
		}
		CreatureFarmQuest creatureFarmQuest = Resource.instance.creatureFarmQuest;
		Npc completionNpc = creatureFarmQuest.completionNpc;
		if (completionNpc == null)
		{
			throw new CommandHandleException(1, "완료NPC가 존재하지 않습니다.");
		}
		if (!currentPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 완료NPC 입니다.");
		}
		if (!completionNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "완료NPC와 상호작용할 수 없는 위치입니다.");
		}
		m_resultItemCollection = new ResultItemCollection();
		foreach (CreatureFarmQuestItemReward reward in creatureFarmQuest.itemRewards)
		{
			ItemReward itemReward = reward.itemReward;
			if (itemReward != null)
			{
				m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		long lnRewardExp = 0L;
		CreatureFarmQuestExpReward creatureFarmQuestExpReward = creatureFarmQuest.GetExpReward(m_myHero.level);
		if (creatureFarmQuestExpReward != null)
		{
			lnRewardExp = creatureFarmQuestExpReward.expRewardValue;
			lnRewardExp += (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
		}
		foreach (ResultItem result in m_resultItemCollection.resultItems)
		{
			int nRemainingCount = m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_33", "MAIL_REWARD_D_33", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(result.item, nRemainingCount, result.owned));
			}
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_myHero.CompleteCreatureFarmQuest();
		SaveToDB();
		SaveToLogDB(lnRewardExp);
		CreatureFarmQuestCompleteResponseBody resBody = new CreatureFarmQuestCompleteResponseBody();
		resBody.acquiredExp = lnRewardExp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreatureFarmQuest_Completion(m_heroCreatureFarmQuest.instanceId, m_currentTime));
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

	private void SaveToLogDB(long lnRewardExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureFarmQuestRewardLog(m_heroCreatureFarmQuest.instanceId, m_myHero.id, lnRewardExp, m_currentTime));
			foreach (ResultItem result in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureFarmQuestRewardDetailLog(Guid.NewGuid(), m_heroCreatureFarmQuest.instanceId, result.item.id, result.owned, result.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
