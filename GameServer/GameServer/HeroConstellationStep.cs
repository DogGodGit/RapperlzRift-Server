using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroConstellationStep
{
	private HeroConstellation m_constellation;

	private int m_nStep;

	private ConstellationEntry m_entry;

	private int m_nFailPoint;

	private bool m_bActivated;

	public HeroConstellation constellation => m_constellation;

	public int step => m_nStep;

	public ConstellationEntry entry
	{
		get
		{
			return m_entry;
		}
		set
		{
			m_entry = value;
		}
	}

	public int failPoint
	{
		get
		{
			return m_nFailPoint;
		}
		set
		{
			m_nFailPoint = value;
		}
	}

	public bool activated
	{
		get
		{
			return m_bActivated;
		}
		set
		{
			m_bActivated = value;
		}
	}

	public HeroConstellationStep(HeroConstellation constellation, int nStep)
	{
		if (constellation == null)
		{
			throw new ArgumentNullException("constellation");
		}
		m_constellation = constellation;
		m_nStep = nStep;
	}

	public void InIt(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nCycle = Convert.ToInt32(dr["cycle"]);
		int nEntryNo = Convert.ToInt32(dr["entryNo"]);
		ConstellationStep constellationStep = Resource.instance.GetConstellation(m_constellation.id).GetStep(m_nStep);
		m_entry = constellationStep.GetCycle(nCycle).GetEntry(nEntryNo);
		m_nFailPoint = Convert.ToInt32(dr["failPoint"]);
		m_bActivated = Convert.ToBoolean(dr["activated"]);
	}

	public bool isOpenableStep()
	{
		if (m_entry.cycle.cycle <= 1)
		{
			return false;
		}
		return true;
	}

	public PDHeroConstellationStep ToPDHeroConstellationStep()
	{
		PDHeroConstellationStep inst = new PDHeroConstellationStep();
		inst.step = m_nStep;
		inst.cycle = m_entry.cycle.cycle;
		inst.entryNo = m_entry.no;
		inst.failPoint = m_nFailPoint;
		inst.activated = m_bActivated;
		return inst;
	}

	public static List<PDHeroConstellationStep> ToPDHeroConstellationSteps(IEnumerable<HeroConstellationStep> steps)
	{
		List<PDHeroConstellationStep> insts = new List<PDHeroConstellationStep>();
		foreach (HeroConstellationStep step in steps)
		{
			insts.Add(step.ToPDHeroConstellationStep());
		}
		return insts;
	}
}
