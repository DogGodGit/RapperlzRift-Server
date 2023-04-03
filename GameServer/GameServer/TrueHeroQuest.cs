using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TrueHeroQuest
{
	public const float kAreaMaxRangeFactor = 1.1f;

	private int m_nRequiredVipLevel;

	private int m_nReqruiedHeroLevel;

	private Npc m_questNpc;

	private float m_fTargetObjectInteractionDuration;

	private float m_fTargetObjectInteractionMaxRange;

	private List<TrueHeroQuestStep> m_steps = new List<TrueHeroQuestStep>();

	private Dictionary<int, TrueHeroQuestReward> m_rewards = new Dictionary<int, TrueHeroQuestReward>();

	public int requiredVipLevel => m_nRequiredVipLevel;

	public int reqruiedHeroLevel => m_nReqruiedHeroLevel;

	public Npc questNpc => m_questNpc;

	public float targetObjectInteractionDuration => m_fTargetObjectInteractionDuration;

	public float targetObjectInteractionMaxRange => m_fTargetObjectInteractionMaxRange;

	public List<TrueHeroQuestStep> steps => m_steps;

	public Dictionary<int, TrueHeroQuestReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredVipLevel = Convert.ToInt32(dr["requiredVipLevel"]);
		if (m_nRequiredVipLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "요구 VIP레벨이 유효하지 않습니다.");
		}
		m_nReqruiedHeroLevel = 0;
		if (m_nReqruiedHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다.");
		}
		int nQuestNpcId = Convert.ToInt32(dr["questNpcId"]);
		m_questNpc = Resource.instance.GetNpc(nQuestNpcId);
		if (m_questNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다. nQuestNpcId = " + nQuestNpcId);
		}
		m_fTargetObjectInteractionDuration = Convert.ToSingle(dr["targetObjectInteractionDuration"]);
		if (m_fTargetObjectInteractionDuration <= 0f)
		{
			SFLogUtil.Warn(GetType(), "목표오브젝트상호작용시간이 유효하지 않습니다. m_fTargetObjectInteractionDuration = " + m_fTargetObjectInteractionDuration);
		}
		m_fTargetObjectInteractionMaxRange = Convert.ToSingle(dr["targetObjectInteractionMaxRange"]);
		if (m_fTargetObjectInteractionDuration <= 0f)
		{
			SFLogUtil.Warn(GetType(), "목표오브젝트상호작용최대범위가 유효하지 않습니다. m_fTargetObjectInteractionMaxRange = " + m_fTargetObjectInteractionMaxRange);
		}
	}

	public void AddStep(TrueHeroQuestStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public TrueHeroQuestStep GetStep(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public void AddReward(TrueHeroQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.level, reward);
	}

	public TrueHeroQuestReward GetReward(int nLevel)
	{
		if (!m_rewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}
}
