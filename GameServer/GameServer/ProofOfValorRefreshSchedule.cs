using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorRefreshSchedule
{
	private ProofOfValor m_proofOfValor;

	private int m_nId;

	private int m_nRefreshTime;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int id => m_nId;

	public int refreshTime => m_nRefreshTime;

	public ProofOfValorRefreshSchedule(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["scheduleId"]);
		m_nRefreshTime = Convert.ToInt32(dr["refreshTime"]);
		if (m_nRefreshTime < 0)
		{
			SFLogUtil.Warn(GetType(), "갱신시각이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRefreshTime = " + m_nRefreshTime);
		}
	}
}
