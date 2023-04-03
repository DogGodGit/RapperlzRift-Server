using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class CashProductPurchaseCount
{
	private Account m_account;

	private int m_nProductId;

	private int m_nCount;

	public Account account => m_account;

	public int productId => m_nProductId;

	public int count
	{
		get
		{
			return m_nCount;
		}
		set
		{
			m_nCount = value;
		}
	}

	public CashProductPurchaseCount(Account account)
		: this(account, 0, 0)
	{
	}

	public CashProductPurchaseCount(Account account, int nProductId, int nCount)
	{
		if (account == null)
		{
			throw new ArgumentNullException("account");
		}
		m_account = account;
		m_nProductId = nProductId;
		m_nCount = nCount;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nProductId = Convert.ToInt32(dr["productId"]);
		m_nCount = Convert.ToInt32(dr["purchaseCount"]);
	}

	public PDCashProductPurchaseCount ToPDCashProductPurchaseCount()
	{
		PDCashProductPurchaseCount inst = new PDCashProductPurchaseCount();
		inst.productId = m_nProductId;
		inst.count = m_nCount;
		return inst;
	}

	public static List<PDCashProductPurchaseCount> ToPDCashProductPurchaseCounts(IEnumerable<CashProductPurchaseCount> counts)
	{
		List<PDCashProductPurchaseCount> results = new List<PDCashProductPurchaseCount>();
		foreach (CashProductPurchaseCount count in counts)
		{
			results.Add(count.ToPDCashProductPurchaseCount());
		}
		return results;
	}
}
