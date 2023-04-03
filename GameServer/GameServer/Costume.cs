using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Costume
{
	private int m_nId;

	private int m_nRequiredHeroLevel;

	private int m_nPeriodLimitDay;

	private Dictionary<int, CostumeAttr> m_attrs = new Dictionary<int, CostumeAttr>();

	public int id => m_nId;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int periodLimitDay => m_nPeriodLimitDay;

	public bool isUnlimit => m_nPeriodLimitDay == 0;

	public Dictionary<int, CostumeAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["costumeId"]);
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nPeriodLimitDay = Convert.ToInt32(dr["periodLimitDay"]);
		if (m_nPeriodLimitDay < 0)
		{
			SFLogUtil.Warn(GetType(), "기간제한일이 유효하지 않습니다. m_nId = " + m_nId + ", m_nPeriodLimitDay = " + m_nPeriodLimitDay);
		}
	}

	public void AddAttr(CostumeAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr.attrId, attr);
	}

	public CostumeAttr GetAttr(int nId)
	{
		if (!m_attrs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
