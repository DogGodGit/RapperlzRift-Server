using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TodayMission : IPickEntry
{
	public const int kId_FishingQuest = 1;

	public const int kId_BountyHunterQuest = 2;

	public const int kId_OsirisRoomDungeon = 3;

	public const int kId_ExpDungeon = 4;

	public const int kId_FieldOfHonor = 5;

	public const int kId_DimensionRaidQuest = 7;

	public const int kId_MysteryBoxQuest = 8;

	public const int kId_SupplySupportQuest = 9;

	public const int kId_Tutorial = 10;

	private int m_nId;

	private int m_nHeroMinLevel;

	private int m_nPoint;

	private int m_nTargetCount;

	private List<TodayMissionReward> m_rewards = new List<TodayMissionReward>();

	public int id => m_nId;

	public int heroMinLevel => m_nHeroMinLevel;

	public int point => m_nPoint;

	public int targetCount => m_nTargetCount;

	public List<TodayMissionReward> rewards => m_rewards;

	public TodayMission()
	{
	}

	public TodayMission(int id, int nLevel)
	{
		m_nId = id;
		m_nHeroMinLevel = nLevel;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션 ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nHeroMinLevel = Convert.ToInt32(dr["heroMinLevel"]);
		if (m_nHeroMinLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "영웅최소레벨이 유효하지 않습니다. m_nId = " + m_nId + ", heroMinLevel = " + heroMinLevel);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표 횟수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetCount = " + m_nTargetCount);
		}
	}

	public void AddReward(TodayMissionReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}
}
