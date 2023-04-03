using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BlessingTargetLevel
{
	private int m_nId;

	private int m_nTargetHeroLevel;

	private int m_nProspectQuestObjectiveLevel;

	private int m_nProspectQuestObjectiveLimitTime;

	private Dictionary<int, ProspectQuestOwnerReward> m_prospectQuestOwnerRewards = new Dictionary<int, ProspectQuestOwnerReward>();

	private Dictionary<int, ProspectQuestTargetReward> m_prospectQuestTargetRewards = new Dictionary<int, ProspectQuestTargetReward>();

	public int id => m_nId;

	public int targetHeroLevel => m_nTargetHeroLevel;

	public int prospectQuestObjectiveLevel => m_nProspectQuestObjectiveLevel;

	public int prospectQuestObjectiveLimitTime => m_nProspectQuestObjectiveLimitTime;

	public Dictionary<int, ProspectQuestOwnerReward> prospectQuestOwnerRewards => m_prospectQuestOwnerRewards;

	public Dictionary<int, ProspectQuestTargetReward> prospectQuestTargetRewards => m_prospectQuestTargetRewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["targetLevelId"]);
		m_nTargetHeroLevel = Convert.ToInt32(dr["targetHeroLevel"]);
		if (m_nTargetHeroLevel < 1)
		{
			SFLogUtil.Warn(GetType(), "대상자레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetHeroLevel = " + m_nTargetHeroLevel);
		}
		m_nProspectQuestObjectiveLevel = Convert.ToInt32(dr["prospectQuestObjectiveLevel"]);
		if (m_nProspectQuestObjectiveLevel <= m_nTargetHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "유망자퀘스트목표레벨이 대상자레벨보다 낮습니다. m_nId = " + m_nId + ", m_nTargetHeroLevel = " + m_nTargetHeroLevel + ", m_nProspectQuestObjectiveLevel = " + m_nProspectQuestObjectiveLevel);
		}
		m_nProspectQuestObjectiveLimitTime = Convert.ToInt32(dr["prospectQuestObjectiveLimitTime"]);
		if (m_nProspectQuestObjectiveLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "유망자퀘스트목표제한시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nProspectQuestObjectiveLimitTime = " + m_nProspectQuestObjectiveLimitTime);
		}
	}

	public void AddProspectQuestOwnerReward(ProspectQuestOwnerReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_prospectQuestOwnerRewards.Add(reward.no, reward);
	}

	public void AddProspectQuestTargetReward(ProspectQuestTargetReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_prospectQuestTargetRewards.Add(reward.no, reward);
	}
}
