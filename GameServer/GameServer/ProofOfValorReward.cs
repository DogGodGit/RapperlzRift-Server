using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorReward
{
	private ProofOfValor m_proofOfValor;

	private int m_nHeroLevel;

	private ExpReward m_successExpReward;

	private ExpReward m_failureExpReward;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int heroLevel => m_nHeroLevel;

	public ExpReward successExpReward => m_successExpReward;

	public long successExpRewardValue
	{
		get
		{
			if (m_successExpReward == null)
			{
				return 0L;
			}
			return m_successExpReward.value;
		}
	}

	public ExpReward failureExpReward => m_failureExpReward;

	public long failureExpRewardValue
	{
		get
		{
			if (m_failureExpReward == null)
			{
				return 0L;
			}
			return m_failureExpReward.value;
		}
	}

	public ProofOfValorReward(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nHeroLevel = Convert.ToInt32(dr["heroLevel"]);
		if (m_nHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "영웅레벨이 유효하지 않습니다. m_nHeroLevel = " + m_nHeroLevel);
		}
		long lnSuccessExpRewardId = Convert.ToInt64(dr["successExpRewardId"]);
		if (lnSuccessExpRewardId > 0)
		{
			m_successExpReward = res.GetExpReward(lnSuccessExpRewardId);
			if (m_successExpReward == null)
			{
				SFLogUtil.Warn(GetType(), "성공경험치보상이 존재하지 않습니다. m_nHeroLevel = " + m_nHeroLevel + ", lnSuccessExpRewardId = " + lnSuccessExpRewardId);
			}
		}
		else if (lnSuccessExpRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "성공경험치보상ID가 유효하지 않습니다. m_nHeroLevel = " + m_nHeroLevel + ", lnSuccessExpRewardId = " + lnSuccessExpRewardId);
		}
		long lnFailureExpRewardId = Convert.ToInt64(dr["failureExpRewardId"]);
		if (lnFailureExpRewardId > 0)
		{
			m_failureExpReward = res.GetExpReward(lnFailureExpRewardId);
			if (m_failureExpReward == null)
			{
				SFLogUtil.Warn(GetType(), "실패경험치보상이 존재하지 않습니다. m_nHeroLevel = " + m_nHeroLevel + ", lnSuccessExpRewardId = " + lnSuccessExpRewardId);
			}
		}
		else if (lnFailureExpRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "실패경험치보상ID가 유효하지 않습니다. m_nHeroLevel = " + m_nHeroLevel + ", lnSuccessExpRewardId = " + lnSuccessExpRewardId);
		}
	}
}
