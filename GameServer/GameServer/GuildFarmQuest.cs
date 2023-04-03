using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildFarmQuest
{
	private int m_nStartTime;

	private int m_nEndTime;

	private int m_nLimitCount;

	private GuildTerritoryNpc m_questNpc;

	private GuildTerritoryNpc m_targetNpc;

	private int m_nInteractionDuration;

	private ItemReward m_completionItemReward;

	private GuildContributionPointReward m_completionGuildContributionPointReward;

	private GuildBuildingPointReward m_completionGuildBuildingPointReward;

	private List<GuildFarmQuestReward> m_rewards = new List<GuildFarmQuestReward>();

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

	public int limitCount => m_nLimitCount;

	public GuildTerritoryNpc questNpc => m_questNpc;

	public GuildTerritoryNpc targetNpc => m_targetNpc;

	public int interactionDuration => m_nInteractionDuration;

	public ItemReward completionItemReward => m_completionItemReward;

	public GuildContributionPointReward completionGuildContributionPointReward => m_completionGuildContributionPointReward;

	public int completionGuildContributionPointRewardValue
	{
		get
		{
			if (m_completionGuildContributionPointReward == null)
			{
				return 0;
			}
			return m_completionGuildContributionPointReward.value;
		}
	}

	public GuildBuildingPointReward completionGuildBuildingPointReward => m_completionGuildBuildingPointReward;

	public int completionGuildBuildingPointRewardValue
	{
		get
		{
			if (m_completionGuildBuildingPointReward == null)
			{
				return 0;
			}
			return m_completionGuildBuildingPointReward.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStartTime = Convert.ToInt32(dr["startTime"]);
		if (m_nStartTime < 0 || m_nStartTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 유효하지 않습니다. m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0 || m_nEndTime > 86400)
		{
			SFLogUtil.Warn(GetType(), "종료시간이 유효하지 않습니다. m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime >= m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 종료시간보다 크거나 같습니다. m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		int nQuestGuildTerritoryNpcId = Convert.ToInt32(dr["questGuildTerritoryNpcId"]);
		m_questNpc = Resource.instance.GetGuildTerritoryNpc(nQuestGuildTerritoryNpcId);
		if (m_questNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다. nQuestGuildTerritoryNpcId = " + nQuestGuildTerritoryNpcId);
		}
		int nTargetGuildTerritoryNpcId = Convert.ToInt32(dr["targetGuildTerritoryNpcId"]);
		m_targetNpc = Resource.instance.GetGuildTerritoryNpc(nTargetGuildTerritoryNpcId);
		if (m_targetNpc == null)
		{
			SFLogUtil.Warn(GetType(), "대상NPC가 존재하지 않습니다. nTargetGuildTerritoryNpcId = " + nTargetGuildTerritoryNpcId);
		}
		m_nInteractionDuration = Convert.ToInt32(dr["interactionDuration"]);
		if (m_nInteractionDuration < 0)
		{
			SFLogUtil.Warn(GetType(), "상호작용기간이 유효하지 않습니다. m_nInteractionDuration = " + m_nInteractionDuration);
		}
		long lnCompletionItemRewardId = Convert.ToInt64(dr["completionItemRewardId"]);
		m_completionItemReward = Resource.instance.GetItemReward(lnCompletionItemRewardId);
		if (m_completionItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료아이템보상이 존재하지 않습니다. lnCompletionItemRewardId = " + lnCompletionItemRewardId);
		}
		long lnCompletionGuildContributionPointRewardId = Convert.ToInt64(dr["completionGuildContributionPointRewardId"]);
		m_completionGuildContributionPointReward = Resource.instance.GetGuildContributionPointReward(lnCompletionGuildContributionPointRewardId);
		if (m_completionGuildContributionPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료길드공헌도보상이 존재하지 않습니다. lnCompletionGuildContributionPointRewardId = " + lnCompletionGuildContributionPointRewardId);
		}
		long lnCompletionGuildBuildingPointRewardId = Convert.ToInt64(dr["completionGuildBuildingPointRewardId"]);
		m_completionGuildBuildingPointReward = Resource.instance.GetGuildBuildingPointReward(lnCompletionGuildBuildingPointRewardId);
		if (m_completionGuildContributionPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료길드건설도보상이 존재하지 않습니다. lnCompletionGuildBuildingPointRewardId = " + lnCompletionGuildBuildingPointRewardId);
		}
	}

	public bool IsQuestTime(float fTimeOfDay)
	{
		if (fTimeOfDay >= (float)m_nStartTime)
		{
			return fTimeOfDay < (float)m_nEndTime;
		}
		return false;
	}

	public void AddReward(GuildFarmQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public GuildFarmQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
