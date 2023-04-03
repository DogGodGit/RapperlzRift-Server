using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class EliteMonsterKillAttrValue
{
	private EliteMonster m_eliteMonster;

	private int m_nKillCount;

	private AttrValue m_attrValue;

	public EliteMonster eliteMonster => m_eliteMonster;

	public int killCount => m_nKillCount;

	public AttrValue attrValue => m_attrValue;

	public int value
	{
		get
		{
			if (m_attrValue == null)
			{
				return 0;
			}
			return m_attrValue.value;
		}
	}

	public EliteMonsterKillAttrValue(EliteMonster eliteMonster)
	{
		m_eliteMonster = eliteMonster;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nKillCount = Convert.ToInt32(dr["killCount"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		if (lnAttrValueId > 0)
		{
			m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
			if (m_attrValue == null)
			{
				SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. eliteMonsterId = " + m_eliteMonster.id + ", lnAttrValueId = " + lnAttrValueId);
			}
		}
		else if (lnAttrValueId < 0)
		{
			SFLogUtil.Warn(GetType(), "속성값ID가 유효하지 않습니다. eliteMonsterId = " + m_eliteMonster.id + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
