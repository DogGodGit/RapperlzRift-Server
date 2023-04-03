using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WeeklyQuestTenRoundImmediatlyCompleteCommandHandler : InGameCommandHandler<WeeklyQuestTenRoundImmediatlyCompleteCommandBody, WeeklyQuestTenRoundImmediatlyCompleteResponseBody>
{
	public const short kResult_NotExistWeeklyQuest = 101;

	public const short kResult_AlreadyWeeklyQuestCompleted = 102;

	public const short kResult_MismatchRoundId = 103;

	public const short kResult_NotEnoughVipLevel = 104;

	public const short kResult_NotEnoughItem = 105;

	private Guid m_oldRoundId = Guid.Empty;

	private HeroWeeklyQuest m_heroWeeklyQuest;

	private int m_nRequiredItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private long m_lnTotalExpReward;

	private long m_lnTotalGoldReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private ResultItemCollection m_round10RewardItemCollection;

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid roundId = (Guid)m_body.roundId;
		if (roundId == Guid.Empty)
		{
			throw new CommandHandleException(1, "라운드ID가 유효하지 않습니다. roundId = " + roundId);
		}
		m_heroWeeklyQuest = m_myHero.weeklyQuest;
		if (m_heroWeeklyQuest == null)
		{
			throw new CommandHandleException(101, "영웅주간퀘스트가 존재하지 않습니다.");
		}
		if (m_heroWeeklyQuest.isCompleted)
		{
			throw new CommandHandleException(102, "영웅주간퀘스트를 완료했습니다.");
		}
		if (m_heroWeeklyQuest.roundMission == null)
		{
			throw new CommandHandleException(1, "영웅주간퀘스트미션이 존재하지 않습니다.");
		}
		if (m_heroWeeklyQuest.roundId != roundId)
		{
			throw new CommandHandleException(103, "현재진행중인 라운드ID가 아닙니다. roundId = " + roundId);
		}
		Resource res = Resource.instance;
		WeeklyQuest weeklyQuest = res.weeklyQuest;
		if (m_myHero.vipLevel.level < weeklyQuest.tenRoundCompletionRequiredVipLevel)
		{
			throw new CommandHandleException(104, "VIP레벨이 부족합니다.");
		}
		int nCompletionCount = m_heroWeeklyQuest.roundNo - 1;
		int nRequiredCompletionCount = 10 - nCompletionCount % 10;
		int nTargetCompletionCount = Math.Min(weeklyQuest.roundCount, m_heroWeeklyQuest.roundNo + nRequiredCompletionCount - 1);
		nRequiredCompletionCount = nTargetCompletionCount - m_heroWeeklyQuest.roundNo + 1;
		m_nRequiredItemId = weeklyQuest.roundImmediateCompletionRequiredItemId;
		if (m_myHero.GetItemCount(m_nRequiredItemId) < nRequiredCompletionCount)
		{
			throw new CommandHandleException(105, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(m_nRequiredItemId, bFisetUseOwn: true, nRequiredCompletionCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		for (int i = nCompletionCount + 1; i <= nTargetCompletionCount; i++)
		{
			WeeklyQuestRound weeklyQuestRound = weeklyQuest.GetRound(i);
			WeeklyQuestRoundReward weeklyQuestRoundReward = weeklyQuestRound.GetReward(m_myHero.level);
			if (weeklyQuestRoundReward != null)
			{
				long lnExpReward = weeklyQuestRoundReward.expRewardValue;
				long lnGoldReward = weeklyQuestRoundReward.goldRewardValue;
				m_lnTotalExpReward += lnExpReward;
				m_lnTotalGoldReward += lnGoldReward;
			}
		}
		float fRewardFactor = weeklyQuest.tenRoundCompletionRewardFactor;
		m_lnTotalExpReward = (long)((float)m_lnTotalExpReward * fRewardFactor);
		m_lnTotalGoldReward = (long)((float)m_lnTotalGoldReward * fRewardFactor);
		m_lnTotalExpReward = (long)Math.Floor((float)m_lnTotalExpReward * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
		m_myHero.AddExp(m_lnTotalExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_myHero.AddGold(m_lnTotalGoldReward);
		if (nTargetCompletionCount % 10 == 0)
		{
			m_round10RewardItemCollection = new ResultItemCollection();
			foreach (WeeklyQuestTenRoundReward reward in Resource.instance.weeklyQuest.tenRoundRewards)
			{
				ItemReward itemReward = reward.itemReward;
				int nRemainingItemCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
				if (nRemainingItemCount > 0)
				{
					if (m_mail == null)
					{
						m_mail = Mail.Create("MAIL_REWARD_N_18", "MAIL_REWARD_D_18", m_currentTime);
					}
					m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingItemCount, itemReward.owned));
				}
				m_round10RewardItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_oldRoundId = m_heroWeeklyQuest.roundId;
		m_heroWeeklyQuest.roundNo = nTargetCompletionCount;
		if (nTargetCompletionCount < weeklyQuest.roundCount)
		{
			m_heroWeeklyQuest.SetNextRound();
		}
		else
		{
			m_heroWeeklyQuest.Complete();
		}
		SaveToDB();
		SaveToLogDB();
		WeeklyQuestTenRoundImmediatlyCompleteResponseBody resBody = new WeeklyQuestTenRoundImmediatlyCompleteResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.acquiredExp = m_lnTotalExpReward;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.nextRoundNo = m_heroWeeklyQuest.roundNo;
		resBody.nextRoundId = (Guid)m_heroWeeklyQuest.roundId;
		resBody.nextRoundMissionId = m_heroWeeklyQuest.roundMissionId;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroWeeklyQuest(m_heroWeeklyQuest));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			if (m_heroWeeklyQuest.roundMission != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestRoundCreationLog(m_heroWeeklyQuest.roundId, m_heroWeeklyQuest.hero.id, m_heroWeeklyQuest.weekStartDate, m_heroWeeklyQuest.roundNo, m_heroWeeklyQuest.roundMissionId, m_currentTime));
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestImmediateComletionLog1(Guid.NewGuid(), m_myHero.id, m_oldRoundId, m_heroWeeklyQuest.roundNo - 1, m_nRequiredItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_lnTotalExpReward, m_lnTotalGoldReward, m_currentTime));
			if (m_round10RewardItemCollection != null)
			{
				Guid round10RewardLogId = Guid.NewGuid();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestTenRoundRewardLog(round10RewardLogId, m_heroWeeklyQuest.hero.id, m_heroWeeklyQuest.weekStartDate, m_heroWeeklyQuest.roundNo - 1, m_currentTime));
				foreach (ResultItem resultItem in m_round10RewardItemCollection.resultItems)
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestTenRoundRewardDetailLog(Guid.NewGuid(), round10RewardLogId, resultItem.item.id, resultItem.owned, resultItem.count));
				}
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
