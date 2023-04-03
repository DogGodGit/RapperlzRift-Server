using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Blessing
{
	public const int kId_Hello = 1;

	public const int kId_Prospect = 2;

	private int m_nId;

	private BlessingMoneyType m_moneyType;

	private int m_nMoneyAmount;

	private ItemReward m_senderItemReward;

	private GoldReward m_receiverGoldReward;

	public int id => m_nId;

	public BlessingMoneyType moneyType => m_moneyType;

	public int moneyAmount => m_nMoneyAmount;

	public ItemReward senderItemReward => m_senderItemReward;

	public GoldReward receiverGoldReward => m_receiverGoldReward;

	public long receiverGoldRewardValue
	{
		get
		{
			if (m_receiverGoldReward == null)
			{
				return 0L;
			}
			return m_receiverGoldReward.value;
		}
	}

	public bool isProspect => m_nId == 2;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["blessingId"]);
		m_moneyType = (BlessingMoneyType)Convert.ToInt32(dr["moneyType"]);
		m_nMoneyAmount = Convert.ToInt32(dr["moneyAmount"]);
		if (m_nMoneyAmount <= 0)
		{
			SFLogUtil.Warn(GetType(), "재화량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMoneyAmount = " + m_nMoneyAmount);
		}
		long lnSenderItemRewardId = Convert.ToInt64(dr["senderItemRewardId"]);
		if (lnSenderItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "발송자아이템보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnSenderItemRewardId = " + lnSenderItemRewardId);
		}
		else if (lnSenderItemRewardId > 0)
		{
			m_senderItemReward = Resource.instance.GetItemReward(lnSenderItemRewardId);
			if (m_senderItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "발송자아이템보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnSenderItemRewardId = " + lnSenderItemRewardId);
			}
		}
		long lnReceiverGoldRewardId = Convert.ToInt64(dr["receiverGoldRewardId"]);
		if (lnReceiverGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "수신자골드보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnReceiverGoldRewardId = " + lnReceiverGoldRewardId);
		}
		else if (lnReceiverGoldRewardId > 0)
		{
			m_receiverGoldReward = Resource.instance.GetGoldReward(lnReceiverGoldRewardId);
			if (m_receiverGoldReward == null)
			{
				SFLogUtil.Warn(GetType(), "수신자골드보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnReceiverGoldRewardId = " + lnReceiverGoldRewardId);
			}
		}
	}
}
