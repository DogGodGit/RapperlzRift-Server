using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MoneyBuff
{
	public const int kMoneyType_Gold = 1;

	public const int kMoneyType_Dia = 2;

	private int m_nId;

	private int m_nLifetime;

	private int m_nMoneyType;

	private int m_nMoneyAmount;

	private List<MoneyBuffAttr> m_attrs = new List<MoneyBuffAttr>();

	public int id => m_nId;

	public int lifetime => m_nLifetime;

	public int moneyType => m_nMoneyType;

	public int moneyAmount => m_nMoneyAmount;

	public List<MoneyBuffAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["buffId"]);
		m_nLifetime = Convert.ToInt32(dr["lifetime"]);
		if (m_nLifetime <= 0)
		{
			SFLogUtil.Warn(GetType(), "유지기간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nLifetime = " + m_nLifetime);
		}
		m_nMoneyType = Convert.ToInt32(dr["moneyType"]);
		if (!IsDefinedMoneyType(m_nMoneyType))
		{
			SFLogUtil.Warn(GetType(), "재화타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMoneyType = " + m_nMoneyType);
		}
		m_nMoneyAmount = Convert.ToInt32(dr["moneyAmount"]);
		if (m_nMoneyAmount < 0)
		{
			SFLogUtil.Warn(GetType(), "재화량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMoneyAmount = " + m_nMoneyAmount);
		}
	}

	public void AddAttr(MoneyBuffAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
	}

	public static bool IsDefinedMoneyType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
