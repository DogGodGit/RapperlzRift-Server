using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationDonationEntry
{
	private int m_nId;

	private NationDonationEntryMoneyType m_moneyType;

	private long m_lnMoneyAmount;

	private ExploitPointReward m_exploitPointReaward;

	private NationFundReward m_nationFundRewrad;

	public int id => m_nId;

	public NationDonationEntryMoneyType moneyType => m_moneyType;

	public long moneyAmount => m_lnMoneyAmount;

	public ExploitPointReward exploitPointReward => m_exploitPointReaward;

	public int exploitPointRewardValue
	{
		get
		{
			if (m_exploitPointReaward == null)
			{
				return 0;
			}
			return m_exploitPointReaward.value;
		}
	}

	public NationFundReward nationFundReward => m_nationFundRewrad;

	public int nationFundRewardValue
	{
		get
		{
			if (m_nationFundRewrad == null)
			{
				return 0;
			}
			return m_nationFundRewrad.value;
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
			SFLogUtil.Warn(GetType(), "국가기부항목ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nMoneyType = Convert.ToInt32(dr["moneyType"]);
		if (!Enum.IsDefined(typeof(NationDonationEntryMoneyType), nMoneyType))
		{
			SFLogUtil.Warn(GetType(), "재화타입이 유효하지 않습니다. m_nId = " + m_nId + ", nMoneyType = " + nMoneyType);
		}
		m_moneyType = (NationDonationEntryMoneyType)nMoneyType;
		m_lnMoneyAmount = Convert.ToInt64(dr["moneyAmount"]);
		if (m_lnMoneyAmount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요재화량이 유효하지 않습니다. m_nId = " + m_nId + ", m_lnMoneyAmount = " + m_lnMoneyAmount);
		}
		long lnExploitPointRewardId = Convert.ToInt64(dr["exploitPointRewardId"]);
		m_exploitPointReaward = Resource.instance.GetExploitPointReward(lnExploitPointRewardId);
		if (m_exploitPointReaward == null)
		{
			SFLogUtil.Warn(GetType(), "공적포인트보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
		}
		long lnNationFundRewardId = Convert.ToInt64(dr["nationFundRewardId"]);
		m_nationFundRewrad = Resource.instance.GetNationFundReward(lnNationFundRewardId);
		if (m_nationFundRewrad == null)
		{
			SFLogUtil.Warn(GetType(), "국가자금보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnNationFundRewardId = " + lnNationFundRewardId);
		}
	}
}
