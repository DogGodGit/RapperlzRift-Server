using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Open7DayEventMission
{
	public const int kType_Level = 1;

	public const int kType_StoryDungeon = 2;

	public const int kType_SubGearLevel = 3;

	public const int kType_ExpDungeon = 4;

	public const int kType_BountyHunterQuest = 5;

	public const int kType_SubGearSoulStoneLevel = 6;

	public const int kType_BattlePower = 7;

	public const int kType_ProofOfValor = 8;

	public const int kType_GuildMissionQuest = 9;

	public const int kType_MainGearEnchantLevel = 10;

	public const int kType_FishingQuest = 11;

	public const int kType_Rank = 12;

	public const int kType_SecretLetterQuest = 13;

	public const int kType_MysteryBoxQuest = 14;

	public const int kType_DimensionRaidQuest = 15;

	public const int kType_HolyWarQuest = 16;

	private int m_nId;

	private Open7DayEventDay m_day;

	private int m_nType;

	private long m_lnTargetValue;

	private List<Open7DayEventMissionReward> m_rewards = new List<Open7DayEventMissionReward>();

	public int id => m_nId;

	public Open7DayEventDay day => m_day;

	public int type => m_nType;

	public long targetValue => m_lnTargetValue;

	public List<Open7DayEventMissionReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nDay = Convert.ToInt32(dr["day"]);
		m_day = Resource.instance.GetOpen7DayEventDay(nDay);
		if (m_day == null)
		{
			SFLogUtil.Warn(GetType(), "일차가 존재하지 않습니다. m_nId = " + m_nId + ", nDay = " + nDay);
		}
		m_nType = Convert.ToInt32(dr["type"]);
		if (m_nType <= 0)
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nType= " + m_nType);
		}
		m_lnTargetValue = Convert.ToInt64(dr["targetValue"]);
		if (m_lnTargetValue <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표치가 유효하지 않습니다. m_nId = " + m_nId + ", m_lnTargetValue = " + m_lnTargetValue);
		}
	}

	public void AddReward(Open7DayEventMissionReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public static bool IsProgressCountAccumulationType(int nType)
	{
		switch (nType)
		{
		case 2:
		case 4:
		case 5:
		case 8:
		case 9:
		case 11:
		case 13:
		case 14:
		case 15:
		case 16:
			return true;
		default:
			return false;
		}
	}
}
