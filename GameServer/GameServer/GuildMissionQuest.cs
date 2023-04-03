using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildMissionQuest
{
	private string m_sNameKey;

	private int m_nLimitCount;

	private Npc m_startNpc;

	private ItemReward m_completionItemReward;

	private Dictionary<int, GuildMission> m_missions = new Dictionary<int, GuildMission>();

	private int m_nTotalPoint;

	private List<GuildMissionQuestReward> m_rewards = new List<GuildMissionQuestReward>();

	public string nameKey => m_sNameKey;

	public int limitCount => m_nLimitCount;

	public Npc startNpc => m_startNpc;

	public ItemReward completionItemReward => m_completionItemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "일일제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		m_startNpc = Resource.instance.GetNpc(nStartNpcId);
		if (m_startNpc == null)
		{
			SFLogUtil.Warn(GetType(), "시작NPC가 존재하지 않습니다. nStartNpcId = " + nStartNpcId);
		}
		long lnCompletionItemRewardId = Convert.ToInt64(dr["completionItemRewardId"]);
		m_completionItemReward = Resource.instance.GetItemReward(lnCompletionItemRewardId);
		if (m_completionItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료아이템보상이 존재하지 않습니다. m_completionItemReward = " + m_completionItemReward);
		}
	}

	public void AddMission(GuildMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.id, mission);
		m_nTotalPoint += mission.point;
	}

	public GuildMission GetMission(int nId)
	{
		if (!m_missions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public GuildMission SelectMission()
	{
		return Util.SelectPickEntry(m_missions.Values, m_nTotalPoint);
	}

	public void AddReward(GuildMissionQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public GuildMissionQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
