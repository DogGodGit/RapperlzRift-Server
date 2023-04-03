using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class Biography
{
	private int m_nId;

	private Item m_requiredItem;

	private List<BiographyReward> m_rewards = new List<BiographyReward>();

	private List<BiographyQuest> m_quests = new List<BiographyQuest>();

	public int id => m_nId;

	public Item requiredItem => m_requiredItem;

	public List<BiographyReward> rewards => m_rewards;

	public List<BiographyQuest> quests => m_quests;

	public BiographyQuest lastQuest => m_quests.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["biographyId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "전기ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nRequiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		m_requiredItem = Resource.instance.GetItem(nRequiredItemId);
		if (m_requiredItem == null)
		{
			SFLogUtil.Warn(GetType(), "필요아이템이 존재하지 않습니다. m_nId = " + m_nId + ", nRequiredItemId = " + nRequiredItemId);
		}
	}

	public void AddReward(BiographyReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public BiographyReward GetReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}

	public void AddQuest(BiographyQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quests.Add(quest);
	}

	public BiographyQuest GetQuest(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_quests.Count)
		{
			return null;
		}
		return m_quests[nIndex];
	}
}
