using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureInjectionLevelUpEntry : IPickEntry
{
	private int m_nNo;

	private int m_nPoint;

	private bool m_bIsCritical;

	private int m_nMinInjectionExp;

	private int m_nMaxInjectionExp;

	public int no => m_nNo;

	public int point => m_nPoint;

	public bool isCritical => m_bIsCritical;

	public int minInjectionExp => m_nMinInjectionExp;

	public int maxInjectionExp => m_nMaxInjectionExp;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		m_bIsCritical = Convert.ToBoolean(dr["isCritical"]);
		m_nMinInjectionExp = Convert.ToInt32(dr["minInjectionExp"]);
		if (m_nMinInjectionExp < 0)
		{
			SFLogUtil.Warn(GetType(), "최소주입경험치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nMinInjectionExp = " + m_nMinInjectionExp);
		}
		m_nMaxInjectionExp = Convert.ToInt32(dr["maxInjectionExp"]);
		if (m_nMaxInjectionExp < m_nMinInjectionExp)
		{
			SFLogUtil.Warn(GetType(), "최소주입경험치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nMaxInjectionExp = " + m_nMaxInjectionExp);
		}
	}
}
