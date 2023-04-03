using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroTaskConsignmentStartCount
{
	private int m_nConsignmentId;

	private int m_nCount;

	public int consignmentId => m_nConsignmentId;

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

	public HeroTaskConsignmentStartCount()
		: this(0, 0)
	{
	}

	public HeroTaskConsignmentStartCount(int nConsignmentId, int nStartCount)
	{
		m_nConsignmentId = nConsignmentId;
		m_nCount = nStartCount;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nConsignmentId = Convert.ToInt32(dr["consignmentId"]);
		m_nCount = Convert.ToInt32(dr["cnt"]);
	}

	public PDHeroTaskConsignmentStartCount ToPDHeroTaskConsignmentStartCount()
	{
		PDHeroTaskConsignmentStartCount inst = new PDHeroTaskConsignmentStartCount();
		inst.consignmentId = m_nConsignmentId;
		inst.startCount = m_nCount;
		return inst;
	}

	public static List<PDHeroTaskConsignmentStartCount> ToPDHeroTaskConsignmentStartCounts(IEnumerable<HeroTaskConsignmentStartCount> startCounts)
	{
		List<PDHeroTaskConsignmentStartCount> insts = new List<PDHeroTaskConsignmentStartCount>();
		foreach (HeroTaskConsignmentStartCount startCount in startCounts)
		{
			insts.Add(startCount.ToPDHeroTaskConsignmentStartCount());
		}
		return insts;
	}
}
