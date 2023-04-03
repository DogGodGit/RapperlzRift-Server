using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildDonationEntry
{
	private int m_nId;

	private string m_sNameKey;

	private GuildDonationEntryMoneyType m_moneyType;

	private long m_lnMoneyAmount;

	private GuildContributionPointReward m_contributionPointReward;

	private GuildFundReward m_fundReward;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public GuildDonationEntryMoneyType moneyType => m_moneyType;

	public long moneyAmount => m_lnMoneyAmount;

	public GuildContributionPointReward contributionPointReward => m_contributionPointReward;

	public int contributionPointRewardValue
	{
		get
		{
			if (m_contributionPointReward == null)
			{
				return 0;
			}
			return m_contributionPointReward.value;
		}
	}

	public GuildFundReward fundReward => m_fundReward;

	public int fundRewardValue
	{
		get
		{
			if (m_fundReward == null)
			{
				return 0;
			}
			return m_fundReward.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["entryId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		int nMoneyType = Convert.ToInt32(dr["moneyType"]);
		if (!Enum.IsDefined(typeof(GuildDonationEntryMoneyType), nMoneyType))
		{
			SFLogUtil.Warn(GetType(), "재화타입이 유효하지 않습니다. m_nId = " + m_nId + ", nMoneyType = " + nMoneyType);
		}
		m_moneyType = (GuildDonationEntryMoneyType)nMoneyType;
		m_lnMoneyAmount = Convert.ToInt64(dr["moneyAmount"]);
		if (m_lnMoneyAmount < 0)
		{
			SFLogUtil.Warn(GetType(), "재화량이 유효하지 않습니다. m_nId = " + m_nId + ", m_lnMoneyAmount = " + m_lnMoneyAmount);
		}
		long lnContributionPointRewardId = Convert.ToInt64(dr["guildContributionPointRewardId"]);
		m_contributionPointReward = Resource.instance.GetGuildContributionPointReward(lnContributionPointRewardId);
		if (m_contributionPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "길드공헌점수보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnContributionPointRewardId = " + lnContributionPointRewardId);
		}
		long lnFundRewardId = Convert.ToInt64(dr["guildFundRewardId"]);
		m_fundReward = Resource.instance.GetGuildFundReward(lnFundRewardId);
		if (m_fundReward == null)
		{
			SFLogUtil.Warn(GetType(), "길드자금보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnFundRewardId = " + lnFundRewardId);
		}
	}
}
