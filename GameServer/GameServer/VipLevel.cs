using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class VipLevel
{
	private int m_nLevel;

	private long m_lnRequiredAccVipPoint;

	private int m_nMainGearEnchantMaxCount;

	private int m_nMainGearRefinementMaxCount;

	private int m_nMountGearRefinementMaxCount;

	private int m_nExpPotionUseMaxCount;

	private int m_nStaminaBuyMaxCount;

	private int m_nExpDungeonEnterCount;

	private int m_nGoldDungeonEnterCount;

	private int m_nOsirisRoomEnterCount;

	private int m_nExpScrollUseMaxCount;

	private int m_nDailyMaxExploitPoint;

	private int m_nMysteryBoxBoostGradePoolId;

	private MysteryBoxGradePool m_mysteryBoxBoostGradePool;

	private int m_nSecretLetterBoostGradePoolId;

	private SecretLetterGradePool m_secretLetterBoostGradePool;

	private int m_nArtifactRoomInitMaxCount;

	private int m_nAncientRelicEnterCount;

	private int m_nFieldOfHonorEnterCount;

	private int m_nDistortionScrollUseMaxCount;

	private int m_nGuildDonationMaxCount;

	private int m_nNationDonationMaxCount;

	private int m_nSoulCoveterWeeklyEnterCount;

	private bool m_bCreatureCardCompositionEnabled;

	private int m_nCreatureCardShopPaidRefreshMaxCount;

	private int m_nProofOfValorEnterCount;

	private int m_nTrueHeroQuestStepNo;

	private int m_nFearAltarEnterCount;

	private float m_fExpDungeonAdditionalExpRewardFactor;

	private int m_nLuckyShopPickMaxCount;

	private int m_nCreatureVariationMaxCount;

	private int m_nTradeShipEnterCount;

	private List<VipLevelReward> m_rewards = new List<VipLevelReward>();

	public int level => m_nLevel;

	public long requiredAccVipPoint => m_lnRequiredAccVipPoint;

	public VipLevel nextVipLevel => Resource.instance.GetVipLevel(m_nLevel + 1);

	public int mainGearEnchantMaxCount => m_nMainGearEnchantMaxCount;

	public int mainGearRefinementMaxCount => m_nMainGearRefinementMaxCount;

	public int mountGearRefinementMaxCount => m_nMountGearRefinementMaxCount;

	public int expPotionUseMaxCount => m_nExpPotionUseMaxCount;

	public int staminaBuyMaxCount => m_nStaminaBuyMaxCount;

	public int expDungeonEnterCount => m_nExpDungeonEnterCount;

	public int goldDungeonEnterCount => m_nGoldDungeonEnterCount;

	public int osirisRoomEnterCount => m_nOsirisRoomEnterCount;

	public int expScrollUseMaxCount => m_nExpScrollUseMaxCount;

	public int dailyMaxExploitPoint => m_nDailyMaxExploitPoint;

	public int mysteryBoxBoostGradePoolId => m_nMysteryBoxBoostGradePoolId;

	public MysteryBoxGradePool mysteryBoxBoostGradePool => m_mysteryBoxBoostGradePool;

	public int secretLetterBoostGradePoolId => m_nSecretLetterBoostGradePoolId;

	public SecretLetterGradePool secretLetterBoostGradePool => m_secretLetterBoostGradePool;

	public int artifactRoomInitMaxCount => m_nArtifactRoomInitMaxCount;

	public int ancientRelicEnterCount => m_nAncientRelicEnterCount;

	public int fieldOfHonorEnterCount => m_nFieldOfHonorEnterCount;

	public int distortionScrollUseMaxCount => m_nDistortionScrollUseMaxCount;

	public int guildDonationMaxCount => m_nGuildDonationMaxCount;

	public int nationDonationMaxCount => m_nNationDonationMaxCount;

	public int soulCoveterWeeklyEnterCount => m_nSoulCoveterWeeklyEnterCount;

	public bool creatureCardCompositionEnabled => m_bCreatureCardCompositionEnabled;

	public int creatureCardShopPaidRefreshMaxCount => m_nCreatureCardShopPaidRefreshMaxCount;

	public int proofOfValorEnterCount => m_nProofOfValorEnterCount;

	public int trueHeroQuestStepNo => m_nTrueHeroQuestStepNo;

	public int fearAltarEnterCount => m_nFearAltarEnterCount;

	public float expDungeonAdditionalExpRewardFactor => m_fExpDungeonAdditionalExpRewardFactor;

	public int luckyShopPickMaxCount => m_nLuckyShopPickMaxCount;

	public int creatureVariationMaxCount => m_nCreatureVariationMaxCount;

	public int tradeShipEnterCount => m_nTradeShipEnterCount;

	public List<VipLevelReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["vipLevel"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_lnRequiredAccVipPoint = Convert.ToInt64(dr["requiredAccVipPoint"]);
		if (m_lnRequiredAccVipPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "필요누적VIP포인트가 유효하지않습니다. m_nLevel = " + m_nLevel + ", m_lnRequiredAccVipPoint = " + m_lnRequiredAccVipPoint);
		}
		m_nMainGearEnchantMaxCount = Convert.ToInt32(dr["mainGearEnchantMaxCount"]);
		if (m_nMainGearEnchantMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "메인장비 강화최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nMainGearEnchantMaxCount = " + m_nMainGearEnchantMaxCount);
		}
		m_nMainGearRefinementMaxCount = Convert.ToInt32(dr["mainGearRefinementMaxCount"]);
		if (m_nMainGearRefinementMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "메인장비 세련최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nMainGearRefinementMaxCount = " + m_nMainGearRefinementMaxCount);
		}
		m_nMountGearRefinementMaxCount = Convert.ToInt32(dr["mountGearRefinementMaxCount"]);
		if (m_nMountGearRefinementMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "탈것장비 세련최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nMountGearRefinementMaxCount = " + m_nMountGearRefinementMaxCount);
		}
		m_nExpPotionUseMaxCount = Convert.ToInt32(dr["expPotionUseMaxCount"]);
		if (m_nExpPotionUseMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "경험치물약 사용최대횟수가 유효하지 않습니다.  m_nLevel = " + m_nLevel + ", m_nExpPotionUseMaxCount = " + m_nExpPotionUseMaxCount);
		}
		m_nStaminaBuyMaxCount = Convert.ToInt32(dr["staminaBuyMaxCount"]);
		if (m_nStaminaBuyMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "스태미너구매최대횟수가 유효하지 않습니다.  m_nLevel = " + m_nLevel + ", m_nStaminaBuyMaxCount = " + m_nStaminaBuyMaxCount);
		}
		m_nExpDungeonEnterCount = Convert.ToInt32(dr["expDungeonEnterCount"]);
		if (m_nExpDungeonEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "경험치던전입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nExpDungeonEnterCount = " + m_nExpDungeonEnterCount);
		}
		m_nOsirisRoomEnterCount = Convert.ToInt32(dr["osirisRoomEnterCount"]);
		if (m_nOsirisRoomEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "오시리스의방입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nOsirisRoomEnterCount = " + m_nOsirisRoomEnterCount);
		}
		m_nGoldDungeonEnterCount = Convert.ToInt32(dr["goldDungeonEnterCount"]);
		if (m_nGoldDungeonEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "골드던전입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nGoldDungeonEnterCount = " + m_nGoldDungeonEnterCount);
		}
		m_nExpScrollUseMaxCount = Convert.ToInt32(dr["ExpScrollUseMaxCount"]);
		if (m_nExpScrollUseMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "경험치주문서사용최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nExpScrollUseMaxCount = " + m_nExpScrollUseMaxCount);
		}
		m_nDailyMaxExploitPoint = Convert.ToInt32(dr["dailyMaxExploitPoint"]);
		if (m_nDailyMaxExploitPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "일일최대공적포인트가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nDailyMaxExploitPoint = " + m_nDailyMaxExploitPoint);
		}
		m_nMysteryBoxBoostGradePoolId = Convert.ToInt32(dr["mysteryBoxBoostGradePoolId"]);
		if (m_nMysteryBoxBoostGradePoolId <= 0)
		{
			SFLogUtil.Warn(GetType(), "의문의상자부스트등급풀ID가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nMysteryBoxBoostGradePoolId = " + m_nMysteryBoxBoostGradePoolId);
		}
		m_nSecretLetterBoostGradePoolId = Convert.ToInt32(dr["secretLetterBoostGradePoolId"]);
		if (m_nSecretLetterBoostGradePoolId <= 0)
		{
			SFLogUtil.Warn(GetType(), "밀서부스트등급풀ID가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nSecretLetterBoostGradePoolId = " + m_nSecretLetterBoostGradePoolId);
		}
		m_nArtifactRoomInitMaxCount = Convert.ToInt32(dr["artifactRoomInitMaxCount"]);
		m_nAncientRelicEnterCount = Convert.ToInt32(dr["ancientRelicEnterCount"]);
		m_nFieldOfHonorEnterCount = Convert.ToInt32(dr["fieldOfHonorEnterCount"]);
		m_nDistortionScrollUseMaxCount = Convert.ToInt32(dr["distortionScrollUseMaxCount"]);
		if (m_nDistortionScrollUseMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "왜곡주문서최대사용횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nDistortionScrollUseMaxCount = " + m_nDistortionScrollUseMaxCount);
		}
		m_nGuildDonationMaxCount = Convert.ToInt32(dr["guildDonationMaxCount"]);
		if (m_nGuildDonationMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "길드기부최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nGuildDonationMaxCount = " + m_nGuildDonationMaxCount);
		}
		m_nNationDonationMaxCount = Convert.ToInt32(dr["nationDonationMaxCount"]);
		if (m_nNationDonationMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "국가기부최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNationDonationMaxCount = " + m_nNationDonationMaxCount);
		}
		m_nSoulCoveterWeeklyEnterCount = Convert.ToInt32(dr["soulCoveterWeeklyEnterCount"]);
		if (m_nSoulCoveterWeeklyEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "영혼을탐하는자주간입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nSoulCoveterWeeklyEnterCount = " + m_nSoulCoveterWeeklyEnterCount);
		}
		m_bCreatureCardCompositionEnabled = Convert.ToBoolean(dr["creatureCardCompositionEnabled"]);
		m_nCreatureCardShopPaidRefreshMaxCount = Convert.ToInt32(dr["CreatureCardShopPaidRefreshMaxCount"]);
		if (m_nCreatureCardShopPaidRefreshMaxCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "크리처카드상점유료갱신최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nCreatureCardShopPaidRefreshMaxCount = " + m_nCreatureCardShopPaidRefreshMaxCount);
		}
		m_nProofOfValorEnterCount = Convert.ToInt32(dr["proofOfValorEnterCount"]);
		if (m_nProofOfValorEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "용맹의증명입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nProofOfValorEnterCount = " + m_nProofOfValorEnterCount);
		}
		m_nTrueHeroQuestStepNo = Convert.ToInt32(dr["trueHeroQuestStepNo"]);
		if (m_nTrueHeroQuestStepNo < 0)
		{
			SFLogUtil.Warn(GetType(), "진정한영웅퀘스트단계번호가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nTrueHeroQuestStepNo = " + m_nTrueHeroQuestStepNo);
		}
		m_nFearAltarEnterCount = Convert.ToInt32(dr["fearAltarEnterCount"]);
		if (m_nFearAltarEnterCount < 0)
		{
			SFLogUtil.Warn(GetType(), "공포의제단입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nFearAltarEnterCount = " + m_nFearAltarEnterCount);
		}
		m_fExpDungeonAdditionalExpRewardFactor = Convert.ToSingle(dr["expDungeonAdditionalExpRewardFactor"]);
		if (m_fExpDungeonAdditionalExpRewardFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "경험치던전추가보상계수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_fExpDungeonAdditionalExpRewardFactor = " + m_fExpDungeonAdditionalExpRewardFactor);
		}
		m_nLuckyShopPickMaxCount = Convert.ToInt32(dr["luckyShopPickMaxcount"]);
		if (m_nLuckyShopPickMaxCount < 0)
		{
			SFLogUtil.Warn(GetType(), "행운상점뽑기최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nLuckyShopPickMaxCount = " + m_nLuckyShopPickMaxCount);
		}
		m_nCreatureVariationMaxCount = Convert.ToInt32(dr["creatureVariationMaxCount"]);
		if (m_nCreatureVariationMaxCount < 0)
		{
			SFLogUtil.Warn(GetType(), "크리처변이최대횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nCreatureVariationMaxCount = " + m_nCreatureVariationMaxCount);
		}
		m_nTradeShipEnterCount = Convert.ToInt32(dr["tradeShipEnterCount"]);
		if (m_nTradeShipEnterCount < 0)
		{
			SFLogUtil.Warn(GetType(), "무역선탈환입장횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nTradeShipEnterCount = " + m_nTradeShipEnterCount);
		}
	}

	public void SetLate()
	{
		m_mysteryBoxBoostGradePool = Resource.instance.mysteryBoxQuest.GetBoxGradePool(m_nMysteryBoxBoostGradePoolId);
		m_secretLetterBoostGradePool = Resource.instance.secretLetterQuest.GetLetterGradePool(m_nSecretLetterBoostGradePoolId);
	}

	public void AddReward(VipLevelReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}
}
