using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureBaseAttrValue
{
	private Creature m_creature;

	private CreatureBaseAttr m_attr;

	private int m_nMinAttrValue;

	private int m_nMaxAttrValue;

	private int m_nIncAttrValue;

	public Creature creature => m_creature;

	public CreatureBaseAttr attr => m_attr;

	public int minAttrValue => m_nMinAttrValue;

	public int maxAttrValue => m_nMaxAttrValue;

	public int incAttrValue => m_nIncAttrValue;

	public CreatureBaseAttrValue(Creature creature)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_creature = creature;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nAttrId = Convert.ToInt32(dr["attrId"]);
		m_attr = Resource.instance.GetCreatureBaseAttr(nAttrId);
		if (m_attr == null)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. nAttrId = " + nAttrId);
		}
		m_nMinAttrValue = Convert.ToInt32(dr["minAttrValue"]);
		if (m_nMinAttrValue < 0)
		{
			SFLogUtil.Warn(GetType(), "최소속성값이 유효하지 않습니다. m_nMinAttrValue = " + m_nMinAttrValue);
		}
		m_nMaxAttrValue = Convert.ToInt32(dr["maxAttrValue"]);
		if (m_nMaxAttrValue < m_nMinAttrValue)
		{
			SFLogUtil.Warn(GetType(), "최대속성값이 유효하지 않습니다. m_nMaxAttrValue = " + m_nMaxAttrValue);
		}
		m_nIncAttrValue = Convert.ToInt32(dr["incAttrValue"]);
		if (m_nIncAttrValue < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨당증가속성값이 유효하지 않습니다. m_nIncAttrValue = " + m_nIncAttrValue);
		}
	}
}
