using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuest
{
	private int m_nRequiredHeroLevel;

	private Npc m_startNpc;

	private Npc m_completionNpc;

	private int m_nLimitCount;

	private List<CreatureFarmQuestExpReward> m_expRewards = new List<CreatureFarmQuestExpReward>();

	private List<CreatureFarmQuestItemReward> m_itemRewards = new List<CreatureFarmQuestItemReward>();

	private List<CreatureFarmQuestMission> m_missions = new List<CreatureFarmQuestMission>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Npc startNpc => m_startNpc;

	public Npc completionNpc => m_completionNpc;

	public int limitCount => m_nLimitCount;

	public List<CreatureFarmQuestExpReward> expRewards => m_expRewards;

	public List<CreatureFarmQuestItemReward> itemRewards => m_itemRewards;

	public List<CreatureFarmQuestMission> missions => m_missions;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		m_startNpc = Resource.instance.GetNpc(nStartNpcId);
		if (m_startNpc == null)
		{
			SFLogUtil.Warn(GetType(), "시작NPC가 존재하지 않습니다. nStartNpcId = " + nStartNpcId);
		}
		int nCompletionNpcId = Convert.ToInt32(dr["completionNpcId"]);
		m_completionNpc = Resource.instance.GetNpc(nCompletionNpcId);
		if (m_completionNpc == null)
		{
			SFLogUtil.Warn(GetType(), "완료NPC가 존재하지 않습니다. nCompletionNpcId = " + nCompletionNpcId);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount < 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
	}

	public void AddExpReward(CreatureFarmQuestExpReward expReward)
	{
		if (expReward == null)
		{
			throw new ArgumentNullException("expReward");
		}
		m_expRewards.Add(expReward);
	}

	public CreatureFarmQuestExpReward GetExpReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_expRewards.Count)
		{
			return null;
		}
		return m_expRewards[nIndex];
	}

	public void AddItemReward(CreatureFarmQuestItemReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_itemRewards.Add(reward);
	}

	public void AddMission(CreatureFarmQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission);
	}

	public CreatureFarmQuestMission GetMission(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_missions.Count)
		{
			return null;
		}
		return m_missions[nIndex];
	}
}
