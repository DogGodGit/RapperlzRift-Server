using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorPaidRefresh
{
	private ProofOfValor m_proofOfValor;

	private int m_nRefreshCount;

	private int m_nRequiredDia;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int refreshCount => m_nRefreshCount;

	public int requiredDia => m_nRequiredDia;

	public ProofOfValorPaidRefresh(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRefreshCount = Convert.ToInt32(dr["RefreshCount"]);
		if (m_nRefreshCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "갱신횟수가 유효하지 않습니다. m_nRefreshCount = " + m_nRefreshCount);
		}
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia < 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아수가 유효하지 않습니다. m_nRefreshCount = " + m_nRefreshCount + ", m_nRefreshCount = " + m_nRefreshCount);
		}
	}
}
