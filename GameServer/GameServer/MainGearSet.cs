using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearSet
{
	private int m_nTier;

	private int m_nGrade;

	private int m_nQuailty;

	private string m_sNameKey;

	private List<MainGearSetAttr> m_attrs = new List<MainGearSetAttr>();

	public int tier => m_nTier;

	public int grade => m_nGrade;

	public int quality => m_nQuailty;

	public string nameKey => m_sNameKey;

	public List<MainGearSetAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nTier = Convert.ToInt32(dr["tier"]);
		if (m_nTier <= 0)
		{
			SFLogUtil.Warn(GetType(), "티어가 유효하지 않습니다. m_nTier = " + m_nTier);
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (!MainGearGrade.IsDefined(m_nGrade))
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nTier = " + m_nTier + ", m_nGrade = " + m_nGrade);
		}
		m_nQuailty = Convert.ToInt32(dr["quality"]);
		if (!MainGearQuality.IsDefined(m_nQuailty))
		{
			SFLogUtil.Warn(GetType(), "품질이 유효하지 않습니다.");
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}

	public void AddAttr(MainGearSetAttr attr)
	{
		if (m_attrs == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
		attr.mainGearSet = this;
	}
}
