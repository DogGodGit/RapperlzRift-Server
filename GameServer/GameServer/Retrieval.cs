using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Retrieval
{
	public const int kId_ExpDungeon = 1;

	public const int kId_FieldOfHonor = 2;

	public const int kId_FearAltarDungeon = 3;

	public const int kId_BountyHunterQuest = 4;

	public const int kId_DimensionRaidQuest = 5;

	public const int kId_SupplySupportQuest = 6;

	public const int kId_FishingQuest = 7;

	public const int kId_MysteryBoxQuest = 8;

	public const int kId_SecretLetterQuest = 9;

	public const int kId_TreatOfFarmQuest = 10;

	public const int kId_AncientRelic = 11;

	public const int kId_WisdomTemple = 12;

	public const int kId_DailyQuest = 13;

	private int m_nId;

	private RetrievalRewardDisplayType m_type = RetrievalRewardDisplayType.ExpReward;

	private long m_lnGoldRetrievalRequiredGold;

	private int m_nDiaRetrievalRequiredDia;

	private Dictionary<int, RetrievalReward> m_rewards = new Dictionary<int, RetrievalReward>();

	public int id => m_nId;

	public RetrievalRewardDisplayType type => m_type;

	public long goldRetrievalReqruiedGold => m_lnGoldRetrievalRequiredGold;

	public int diaRetrievalRequiredDia => m_nDiaRetrievalRequiredDia;

	public Dictionary<int, RetrievalReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["retrievalId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "회수ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nType = Convert.ToInt32(dr["rewardDisplayType"]);
		if (!Enum.IsDefined(typeof(RetrievalRewardDisplayType), nType))
		{
			SFLogUtil.Warn(GetType(), "회수보상표시타입이 유효하지 않습니다.");
		}
		m_type = (RetrievalRewardDisplayType)nType;
		m_lnGoldRetrievalRequiredGold = Convert.ToInt32(dr["goldRetrievalRequiredGold"]);
		if (m_lnGoldRetrievalRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "골드회수요구골드가 유효하지 않습니다. m_nId = " + m_nId + ", m_lnGoldRetrievalRequiredGold = " + m_lnGoldRetrievalRequiredGold);
		}
		m_nDiaRetrievalRequiredDia = Convert.ToInt32(dr["diaRetrievalRequiredDia"]);
		if (m_nDiaRetrievalRequiredDia < 0)
		{
			SFLogUtil.Warn(GetType(), "다이아회수요구다이아가 유효하지 않습니다. m_nId = " + m_nId + ", m_nDiaRetrievalRequiredDia = " + m_nDiaRetrievalRequiredDia);
		}
	}

	public void AddReward(RetrievalReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.level, reward);
	}

	public RetrievalReward GetReward(int nLevel)
	{
		if (!m_rewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}
}
