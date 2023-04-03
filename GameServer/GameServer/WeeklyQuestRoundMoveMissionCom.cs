using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WeeklyQuestRoundMoveMissionCompleteCommandHandler : InGameCommandHandler<WeeklyQuestRoundMoveMissionCompleteCommandBody, WeeklyQuestRoundMoveMissionCompleteResponseBody>
{
	public const short kResult_NotExistWeeklyQuest = 101;

	public const short kResult_AlreadyWeeklyQuestCompleted = 102;

	public const short kResult_MismatchRoundId = 103;

	public const short kResult_InvalidateStatus = 104;

	private HeroWeeklyQuest m_heroWeeklyQuest;

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
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에서 사용할 수 없는 명령입니다.");
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
		WeeklyQuestMission weeklyQuestMission = m_heroWeeklyQuest.roundMission;
		if (weeklyQuestMission.type != WeeklyQuestMissionType.Move)
		{
			throw new CommandHandleException(1, "해당 미션은 이동타입이 아닙니다.");
		}
		if (!currentPlace.IsSame(weeklyQuestMission.targetContinent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "해당 장소에 없는 목표입니다.");
		}
		if (!weeklyQuestMission.TargetAreaContains(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "영웅이 목표 좌표에 위치해있지 않습니다.");
		}
		m_heroWeeklyQuest.roundProgressCount++;
		_ = m_heroWeeklyQuest.roundProgressCount;
		SaveToDB();
		m_heroWeeklyQuest.CompleteRound(m_currentTime, bSendEvent: false, m_changedInventorySlots, out m_lnExpReward, out m_lnGoldReward);
		WeeklyQuestRoundMoveMissionCompleteResponseBody resBody = new WeeklyQuestRoundMoveMissionCompleteResponseBody();
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

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroWeeklyQuest_ProgressCount(m_heroWeeklyQuest.hero.id, m_heroWeeklyQuest.roundProgressCount));
		dbWork.Schedule();
	}
}
