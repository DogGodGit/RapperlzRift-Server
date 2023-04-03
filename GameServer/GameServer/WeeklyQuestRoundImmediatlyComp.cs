using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WeeklyQuestRoundImmediatlyCompleteCommandHandler : InGameCommandHandler<WeeklyQuestRoundImmediatlyCompleteCommandBody, WeeklyQuestRoundImmediatlyCompleteResponseBody>
{
	public const short kResult_NotExistWeeklyQuest = 101;

	public const short kResult_AlreadyWeeklyQuestCompleted = 102;

	public const short kResult_MismatchRoundId = 103;

	public const short kResult_InvalidateStatus = 104;

	public const short kResult_NotEnoughItem = 105;

	private HeroWeeklyQuest m_heroWeeklyQuest;

	private Guid m_oldRoundId = Guid.Empty;

	private int m_nRequiredItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private long m_lnExpReward;

	private long m_lnGoldReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

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
		if (!m_heroWeeklyQuest.isRoundAccepted)
		{
			throw new CommandHandleException(104, "영웅주간퀘스트가 진행상태가 아닙니다.");
		}
		m_nRequiredItemId = Resource.instance.weeklyQuest.roundImmediateCompletionRequiredItemId;
		if (m_myHero.GetItemCount(m_nRequiredItemId) < 1)
		{
			throw new CommandHandleException(105, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(m_nRequiredItemId, bFisetUseOwn: true, 1, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_oldRoundId = m_heroWeeklyQuest.roundId;
		m_heroWeeklyQuest.CompleteRound(m_currentTime, bSendEvent: false, m_changedInventorySlots, out m_lnExpReward, out m_lnGoldReward);
		SaveToLogDB();
		WeeklyQuestRoundImmediatlyCompleteResponseBody resBody = new WeeklyQuestRoundImmediatlyCompleteResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.acquiredExp = m_lnExpReward;
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

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestImmediateComletionLog1(Guid.NewGuid(), m_myHero.id, m_oldRoundId, m_heroWeeklyQuest.roundNo - 1, m_nRequiredItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_lnGoldReward, m_lnExpReward, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
