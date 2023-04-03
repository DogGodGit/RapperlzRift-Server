using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorClearGrade
{
	public const int kClearGrade_S = 1;

	public const int kClearGrade_A = 2;

	public const int kClearGrade_B = 3;

	public const int kClearGrade_C = 4;

	private ProofOfValor m_proofOfValor;

	private int m_nClearGrade;

	private int m_nMinRemainTime;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public int clearGrade => m_nClearGrade;

	public int minRemainingTime => m_nMinRemainTime;

	public ProofOfValorClearGrade(ProofOfValor proofOfValor)
	{
		m_proofOfValor = proofOfValor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nClearGrade = Convert.ToInt32(dr["clearGrade"]);
		if (!IsDefinedClearGrade(m_nClearGrade))
		{
			SFLogUtil.Warn(GetType(), "클리어등급이 유효하지 않습니다. m_nClearGrade = " + m_nClearGrade);
		}
		m_nMinRemainTime = Convert.ToInt32(dr["minRemainTime"]);
		if (m_nMinRemainTime < 0)
		{
			SFLogUtil.Warn(GetType(), "최소남은시간이 유효하지 않습니다. m_nClearGrade = " + m_nClearGrade + ", m_nMinRemainTime = " + m_nMinRemainTime);
		}
	}

	public bool IsDefinedClearGrade(int nClearGrade)
	{
		if (nClearGrade != 1 && nClearGrade != 2 && nClearGrade != 3)
		{
			return nClearGrade == 4;
		}
		return true;
	}
}
