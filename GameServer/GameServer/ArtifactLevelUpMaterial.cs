using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ArtifactLevelUpMaterial
{
	private int m_nTier;

	private int m_nGrade;

	private int m_nExp;

	public int tier => m_nTier;

	public int grade => m_nGrade;

	public int exp => m_nExp;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nTier = Convert.ToInt32(dr["tier"]);
		if (m_nTier <= 0)
		{
			SFLogUtil.Warn(GetType(), "메인장비티어가 유효하지 않습니다. m_nTier = " + m_nTier);
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (m_nTier <= 0)
		{
			SFLogUtil.Warn(GetType(), "메인장비등급이 유효하지 않습니다. m_nTier = " + m_nTier + ", m_nGrade = " + m_nGrade);
		}
		m_nExp = Convert.ToInt32(dr["exp"]);
		if (m_nExp <= 0)
		{
			SFLogUtil.Warn(GetType(), "경험치가 유효하지 않습니다. m_nTier = " + m_nTier + ", m_nGrade = " + m_nGrade + ", m_nExp = " + m_nExp);
		}
	}
}
