using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ConstellationStep
{
	private Constellation m_constellation;

	private int m_nStep;

	private int m_nRequiredDia;

	private List<ConstellationCycle> m_cycles = new List<ConstellationCycle>();

	public Constellation constellation => m_constellation;

	public int step => m_nStep;

	public int requiredDia => m_nRequiredDia;

	public List<ConstellationCycle> cycles => m_cycles;

	public int lastCycle => m_cycles.Count;

	public ConstellationStep(Constellation constellation)
	{
		if (constellation == null)
		{
			throw new ArgumentNullException("constellation");
		}
		m_constellation = constellation;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep <= 0)
		{
			SFLogUtil.Warn(GetType(), "단계가 유효하지 않습니다. m_nStep = " + m_nStep);
		}
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia < 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다. m_nStep = " + m_nStep + ", m_nRequiredDia = " + m_nRequiredDia);
		}
	}

	public void AddCycle(ConstellationCycle cycle)
	{
		if (cycle == null)
		{
			throw new ArgumentNullException("cycle");
		}
		m_cycles.Add(cycle);
	}

	public ConstellationCycle GetCycle(int nCycle)
	{
		int nIndex = nCycle - 1;
		if (nIndex < 0 || nIndex >= m_cycles.Count)
		{
			return null;
		}
		return m_cycles[nIndex];
	}
}
