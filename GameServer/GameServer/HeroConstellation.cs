using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroConstellation
{
	private Hero m_hero;

	private int m_nId;

	private List<HeroConstellationStep> m_steps = new List<HeroConstellationStep>();

	public Hero hero => m_hero;

	public int id => m_nId;

	public List<HeroConstellationStep> steps => m_steps;

	public HeroConstellation(Hero hero, int nId)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_nId = nId;
	}

	public void AddStep(HeroConstellationStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public HeroConstellationStep GetStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public HeroConstellationStep CreateStep(ConstellationStep constellationStep)
	{
		if (constellationStep == null)
		{
			throw new ArgumentNullException("constellationStep");
		}
		HeroConstellationStep heroConstellationStep = new HeroConstellationStep(this, constellationStep.step);
		heroConstellationStep.entry = constellationStep.GetCycle(1).GetEntry(1);
		AddStep(heroConstellationStep);
		return heroConstellationStep;
	}

	public PDHeroConstellation ToPDHeroConstellation()
	{
		PDHeroConstellation inst = new PDHeroConstellation();
		inst.id = m_nId;
		inst.steps = HeroConstellationStep.ToPDHeroConstellationSteps(m_steps).ToArray();
		return inst;
	}

	public static List<PDHeroConstellation> ToPDHeroConstellations(IEnumerable<HeroConstellation> constellations)
	{
		List<PDHeroConstellation> insts = new List<PDHeroConstellation>();
		foreach (HeroConstellation consellation in constellations)
		{
			insts.Add(consellation.ToPDHeroConstellation());
		}
		return insts;
	}
}
