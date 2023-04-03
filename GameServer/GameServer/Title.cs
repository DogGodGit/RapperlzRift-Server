using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Title
{
	private int m_nId;

	private TitleType m_type;

	private int m_nGrade;

	private int m_nLifetime;

	private List<TitleActiveAttr> m_activeAttrs = new List<TitleActiveAttr>();

	private List<TitlePassiveAttr> m_passiveAttrs = new List<TitlePassiveAttr>();

	public int id => m_nId;

	public TitleType type => m_type;

	public int grade => m_nGrade;

	public int lifetime => m_nLifetime;

	public List<TitleActiveAttr> activeAttrs => m_activeAttrs;

	public List<TitlePassiveAttr> passiveAttrs => m_passiveAttrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["titleId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "칭호ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nType = Convert.ToInt32(dr["type"]);
		m_type = Resource.instance.GetTitleType(nType);
		if (m_type == null)
		{
			SFLogUtil.Warn(GetType(), "타입이 존재하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (m_nGrade <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nId = " + m_nId + ", m_nGrade = " + m_nGrade);
		}
		m_nLifetime = Convert.ToInt32(dr["lifetime"]);
		if (m_nLifetime < 0)
		{
			SFLogUtil.Warn(GetType(), "유지기간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nLifetime = " + m_nLifetime);
		}
	}

	public void AddActiveAttr(TitleActiveAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_activeAttrs.Add(attr);
	}

	public void AddPassiveAttr(TitlePassiveAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_passiveAttrs.Add(attr);
	}
}
