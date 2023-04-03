using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MoneyBuffAttr
{
	private MoneyBuff m_moneyBuff;

	private int m_nId;

	private float m_fAttrFactor;

	public MoneyBuff moneyBuff => m_moneyBuff;

	public int id => m_nId;

	public float attrFactor => m_fAttrFactor;

	public MoneyBuffAttr(MoneyBuff moneyBuff)
	{
		m_moneyBuff = moneyBuff;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		m_fAttrFactor = Convert.ToSingle(dr["attrFactor"]);
		if (m_fAttrFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "속성계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fAttrFactor = " + m_fAttrFactor);
		}
	}
}
