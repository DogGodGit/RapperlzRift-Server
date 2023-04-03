using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildSupplySupportQuest
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private int m_nLimitTime;

	private Npc m_startNpc;

	private Npc m_completionNpc;

	private Cart m_cart;

	private GuildBuildingPointReward m_guildBuildingPointReward;

	private GuildFundReward m_guildFundReward;

	private float m_fCompletionRewardableRadius;

	private GuildContributionPointReward m_completionGuildContributionPointReward;

	private List<GuildSupplySupportQuestReward> m_rewards = new List<GuildSupplySupportQuestReward>();

	public int limitTime => m_nLimitTime;

	public Npc startNpc => m_startNpc;

	public Npc completionNpc => m_completionNpc;

	public Cart cart => m_cart;

	public GuildBuildingPointReward guildBuildingPointReward => m_guildBuildingPointReward;

	public GuildFundReward guildFundReward => m_guildFundReward;

	public float completionRewardableRadius => m_fCompletionRewardableRadius;

	public GuildContributionPointReward completionGuildContributionPointReward => m_completionGuildContributionPointReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
		Resource res = Resource.instance;
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		if (nStartNpcId > 0)
		{
			m_startNpc = res.GetNpc(nStartNpcId);
			if (m_startNpc == null)
			{
				SFLogUtil.Warn(GetType(), "시작NPC가 존재하지 않습니다. nStartNpcId = " + nStartNpcId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "시작NPCID가 유효하지 않습니다. nStartNpcId = " + nStartNpcId);
		}
		int nCompletionNpcId = Convert.ToInt32(dr["completionNpcId"]);
		if (nCompletionNpcId > 0)
		{
			m_completionNpc = res.GetNpc(nCompletionNpcId);
			if (m_completionNpc == null)
			{
				SFLogUtil.Warn(GetType(), "완료NPC가 존재하지 않습니다. nCompletionNpcId = " + nCompletionNpcId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "완료NPCID가 유효하지 않습니다. nCompletionNpcId = " + nCompletionNpcId);
		}
		int nCartId = Convert.ToInt32(dr["cartId"]);
		if (nCartId > 0)
		{
			m_cart = res.GetCart(nCartId);
			if (m_cart == null)
			{
				SFLogUtil.Warn(GetType(), "카트가 존재하지 않습니다. m_cart = " + m_cart);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "카트ID가 유효하지 않습니다. nCartId = " + nCartId);
		}
		long lnGuildBuildingPointRewardId = Convert.ToInt64(dr["guildBuildingPointRewardId"]);
		if (lnGuildBuildingPointRewardId > 0)
		{
			m_guildBuildingPointReward = res.GetGuildBuildingPointReward(lnGuildBuildingPointRewardId);
			if (m_guildBuildingPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "길드건설점수보상이 존재하지 않습니다. lnGuildBuildingPointRewardId = " + lnGuildBuildingPointRewardId);
			}
		}
		else if (lnGuildBuildingPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "길드건설점수보상ID가 유효하지 않습니다. lnGuildBuildingPointRewardId = " + lnGuildBuildingPointRewardId);
		}
		long lnGuildFundRewardId = Convert.ToInt64(dr["guildFundRewardId"]);
		if (lnGuildFundRewardId > 0)
		{
			m_guildFundReward = res.GetGuildFundReward(lnGuildFundRewardId);
			if (m_guildFundReward == null)
			{
				SFLogUtil.Warn(GetType(), "길드자금보상이 존재하지 않습니다. lnGuildFundRewardId = " + lnGuildFundRewardId);
			}
		}
		else if (lnGuildFundRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "길드자금보상ID가 유효하지 않습니다. lnGuildFundRewardId = " + lnGuildFundRewardId);
		}
		m_fCompletionRewardableRadius = Convert.ToSingle(dr["completionRewardableRadius"]);
		if (m_fCompletionRewardableRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "완료보상가능반지름이 유효하지 않습니다. m_fCompletionRewardableRadius = " + m_fCompletionRewardableRadius);
		}
		long lnCompletionGuildContributionPointRewardId = Convert.ToInt64(dr["completionGuildContributionPointRewardId"]);
		if (lnCompletionGuildContributionPointRewardId > 0)
		{
			m_completionGuildContributionPointReward = res.GetGuildContributionPointReward(lnCompletionGuildContributionPointRewardId);
			if (m_completionGuildContributionPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "완료길드공헌점수보상이 존재하지 않습니다. m_completionGuildContributionPointReward = " + m_completionGuildContributionPointReward);
			}
		}
		else if (lnCompletionGuildContributionPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "완료길드공헌점수보상ID가 유효하지 않습니다. lnCompletionGuildContributionPointRewardId = " + lnCompletionGuildContributionPointRewardId);
		}
	}

	public void AddReward(GuildSupplySupportQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public GuildSupplySupportQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
