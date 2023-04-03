using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildAltar
{
	public const float kDefenseCoolTimeFactor = 0.9f;

	private GuildTerritoryNpc m_npc;

	private int m_nDailyHeroMaxMoralPoint;

	private int m_nDailyGuildMaxMoralPoint;

	private int m_nDonationGold;

	private int m_nDonationRewardMoralPoint;

	private int m_nSpellInjectionDuration;

	private int m_nSpellInjectionRewardMoralPoint;

	private MonsterArrange m_defenseMonsterArrange;

	private int m_nDefenseRewardMoralPoint;

	private int m_nDefenseCoolTime;

	private int m_nDefenseLimitTime;

	private GuildContributionPointReward m_guildContributionPointReward;

	private GuildFundReward m_guildFundReward;

	private GuildBuildingPointReward m_guildBuildingPointReward;

	private List<GuildAltarReward> m_rewards = new List<GuildAltarReward>();

	private List<GuildAltarDefenseMonsterAttrFactor> m_defenseMonsterAttrFactors = new List<GuildAltarDefenseMonsterAttrFactor>();

	public GuildTerritoryNpc npc => m_npc;

	public int dailyHeroMaxMoralPoint => m_nDailyHeroMaxMoralPoint;

	public int dailyGuildMaxMoralPoint => m_nDailyGuildMaxMoralPoint;

	public int donationGold => m_nDonationGold;

	public int donationRewardMoralPoint => m_nDonationRewardMoralPoint;

	public int spellInjectionDuration => m_nSpellInjectionDuration;

	public int spellInjectionRewardMoralPoint => m_nSpellInjectionRewardMoralPoint;

	public MonsterArrange defenseMonsterArrange => m_defenseMonsterArrange;

	public int defenseRewardMoralPoint => m_nDefenseRewardMoralPoint;

	public int defenseCoolTime => m_nDefenseCoolTime;

	public int defenseLimitTime => m_nDefenseLimitTime;

	public GuildContributionPointReward guildContributionPointReward => m_guildContributionPointReward;

	public int guildContributionPointRewardValue
	{
		get
		{
			if (m_guildContributionPointReward == null)
			{
				return 0;
			}
			return m_guildContributionPointReward.value;
		}
	}

	public GuildFundReward guildFundReward => m_guildFundReward;

	public int guildFundRewardValue
	{
		get
		{
			if (m_guildFundReward == null)
			{
				return 0;
			}
			return m_guildFundReward.value;
		}
	}

	public GuildBuildingPointReward guildBuildingPointReward => m_guildBuildingPointReward;

	public int guildBuildingPointRewardValue
	{
		get
		{
			if (m_guildBuildingPointReward == null)
			{
				return 0;
			}
			return m_guildBuildingPointReward.value;
		}
	}

	public List<GuildAltarReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nGuildTerritoryNpcId = Convert.ToInt32(dr["guildTerritoryNpcId"]);
		m_npc = Resource.instance.GetGuildTerritoryNpc(nGuildTerritoryNpcId);
		if (m_npc == null)
		{
			SFLogUtil.Warn(GetType(), "NPC가 존재하지 않습니다. nGuildTerritoryNpcId = " + nGuildTerritoryNpcId);
		}
		m_nDailyHeroMaxMoralPoint = Convert.ToInt32(dr["dailyHeroMaxMoralPoint"]);
		if (m_nDailyHeroMaxMoralPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "일일영웅최대모럴포인트가 유효하지 않습니다. m_nDailyHeroMaxMoralPoint = " + m_nDailyHeroMaxMoralPoint);
		}
		m_nDailyGuildMaxMoralPoint = Convert.ToInt32(dr["dailyGuildMaxMoralPoint"]);
		if (m_nDailyGuildMaxMoralPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "일일길드최대모럴포인트가 유효하지 않습니다. m_nDailyGuildMaxMoralPoint = " + m_nDailyGuildMaxMoralPoint);
		}
		m_nDonationGold = Convert.ToInt32(dr["donationGold"]);
		if (m_nDonationGold <= 0)
		{
			SFLogUtil.Warn(GetType(), "기부골드가 유효하지 않습니다. m_nDonationGold = " + m_nDonationGold);
		}
		m_nDonationRewardMoralPoint = Convert.ToInt32(dr["donationRewardMoralPoint"]);
		if (m_nDonationRewardMoralPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "기부보상모럴포인트가 유효하지 않습니다. m_nDonationRewardMoralPoint = " + m_nDonationRewardMoralPoint);
		}
		m_nSpellInjectionDuration = Convert.ToInt32(dr["spellInjectionDuration"]);
		if (m_nSpellInjectionDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "마력주입기간이 유효하지 않습니다. m_nSpellInjectionDuration = " + m_nSpellInjectionDuration);
		}
		m_nSpellInjectionRewardMoralPoint = Convert.ToInt32(dr["spellInjectionRewardMoralPoint"]);
		if (m_nSpellInjectionRewardMoralPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "마력주입보상모럴포인트가 유효하지 않습니다. m_nSpellInjectionRewardMoralPoint = " + m_nSpellInjectionRewardMoralPoint);
		}
		long lnDefenseMonsterArrangeId = Convert.ToInt64(dr["defenseMonsterArrangeId"]);
		m_defenseMonsterArrange = Resource.instance.GetMonsterArrange(lnDefenseMonsterArrangeId);
		if (m_defenseMonsterArrange == null)
		{
			SFLogUtil.Warn(GetType(), "수비몬스터배치가 존재하지 않습니다. lnDefenseMonsterArrangeId = " + lnDefenseMonsterArrangeId);
		}
		m_nDefenseRewardMoralPoint = Convert.ToInt32(dr["defenseRewardMoralPoint"]);
		if (m_nDefenseRewardMoralPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "수비보상모럴포인트가 유효하지 않습니다. m_nDefenseRewardMoralPoint = " + m_nDefenseRewardMoralPoint);
		}
		m_nDefenseCoolTime = Convert.ToInt32(dr["defenseCoolTime"]);
		if (m_nDefenseCoolTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "수비쿨타임이 유효하지 않습니다. m_nDefenseCoolTime = " + m_nDefenseCoolTime);
		}
		m_nDefenseLimitTime = Convert.ToInt32(dr["defenseLimitTime"]);
		if (m_nDefenseLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "수비제한시간이 유효하지 않습니다. m_nDefenseLimitTime = " + m_nDefenseLimitTime);
		}
		long lnGuildContributionPointRewardId = Convert.ToInt64(dr["guildContributionPointRewardId"]);
		m_guildContributionPointReward = Resource.instance.GetGuildContributionPointReward(lnGuildContributionPointRewardId);
		if (m_guildContributionPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료길드공헌도보상이 존재하지 않습니다. lnGuildContributionPointRewardId = " + lnGuildContributionPointRewardId);
		}
		long lnGuildFundRewardId = Convert.ToInt64(dr["guildFundRewardId"]);
		m_guildFundReward = Resource.instance.GetGuildFundReward(lnGuildFundRewardId);
		if (m_guildFundReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료길드자금보상이 존재하지 않습니다. lnGuildFundRewardId = " + lnGuildFundRewardId);
		}
		long lnGuildBuildingPointRewardId = Convert.ToInt64(dr["guildBuildingPointRewardId"]);
		m_guildBuildingPointReward = Resource.instance.GetGuildBuildingPointReward(lnGuildBuildingPointRewardId);
		if (m_guildBuildingPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "완료길드건설도보상이 존재하지 않습니다. lnGuildBuildingPointRewardId = " + lnGuildBuildingPointRewardId);
		}
	}

	public void AddReward(GuildAltarReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public GuildAltarReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}

	public void AddDefenseMonsterAttrFactor(GuildAltarDefenseMonsterAttrFactor factor)
	{
		if (factor == null)
		{
			throw new ArgumentNullException("factor");
		}
		m_defenseMonsterAttrFactors.Add(factor);
	}

	public GuildAltarDefenseMonsterAttrFactor GetDefenseMonsterAttrFactor(int nHeroLevel)
	{
		int nIndex = nHeroLevel - 1;
		if (nIndex < 0 || nIndex >= m_defenseMonsterAttrFactors.Count)
		{
			return null;
		}
		return m_defenseMonsterAttrFactors[nIndex];
	}
}
