using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BountyHunterQuestCompleteCommandHandler : InGameCommandHandler<BountyHunterQuestCompleteCommandBody, BountyHunterQuestCompleteResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		HeroBountyHunterQuest heroBountyHunterQuest = m_myHero.bountyHunterQuest;
		if (heroBountyHunterQuest == null)
		{
			throw new CommandHandleException(1, "영웅현상금사냥꾼퀘스트가 존재하지 않습니다.");
		}
		if (!heroBountyHunterQuest.objectiveCompleted)
		{
			throw new CommandHandleException(1, "영웅현상금사냥꾼퀘스트의 목표가 완료되지 않았습니다.");
		}
		BountyHunterQuestRewardCollection rewardCollection = Resource.instance.GetBountyHunterQuestRewardCollection(heroBountyHunterQuest.itemGrade);
		BountyHunterQuestReward bountyHunterQuestrReward = rewardCollection.GetReward(m_myHero.level);
		long lnExpReward = bountyHunterQuestrReward.expRewardValue;
		if (lnExpReward > 0)
		{
			lnExpReward = (long)Math.Floor((float)lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		m_myHero.bountyHunterQuest = null;
		SaveToDB(heroBountyHunterQuest, bountyHunterQuestrReward.level, lnExpReward);
		BountyHunterQuestCompleteResponseBody resBody = new BountyHunterQuestCompleteResponseBody();
		resBody.acquiredExp = lnExpReward;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
		m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.BountyHunterQuest, 1, m_currentTime);
		if (heroBountyHunterQuest.itemGrade == 5)
		{
			m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.LegendBountyHunterQuest, 1, m_currentTime);
		}
	}

	private void SaveToDB(HeroBountyHunterQuest heroBountyHunterQuest, int nOldLevel, long lnRewardExp)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateBountyHunterQuest_Status(heroBountyHunterQuest.id, 1, m_currentTime));
		dbWork.Schedule();
		SaveToDB_BountyHunterQuestRewardLog(heroBountyHunterQuest, nOldLevel, lnRewardExp);
	}

	private void SaveToDB_BountyHunterQuestRewardLog(HeroBountyHunterQuest heroBountyHunterQuest, int nOldLevel, long lnRewardExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddBountyHunterQuestRewardLog(Guid.NewGuid(), m_myHero.id, heroBountyHunterQuest.id, nOldLevel, lnRewardExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
