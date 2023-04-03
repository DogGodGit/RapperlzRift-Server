using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroMainGearRefinement
{
	private HeroMainGear m_heroMainGear;

	private int m_nTurn;

	private List<HeroMainGearRefinementAttr> m_refinementAttrs = new List<HeroMainGearRefinementAttr>();

	public HeroMainGear heroMainGear => m_heroMainGear;

	public int turn => m_nTurn;

	public List<HeroMainGearRefinementAttr> attrs => m_refinementAttrs;

	public HeroMainGearRefinement(HeroMainGear heroMainGear, int nTurn)
	{
		m_heroMainGear = heroMainGear;
		m_nTurn = nTurn;
	}

	public void AddAttr(HeroMainGearRefinementAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		if (attr.refinement != null)
		{
			throw new Exception("이미 영웅메인장비세련에 추가된 영웅메인장비세련속성입니다.");
		}
		m_refinementAttrs.Add(attr);
		attr.refinement = this;
	}

	public HeroMainGearRefinementAttr GetAttr(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_refinementAttrs.Count)
		{
			return null;
		}
		return m_refinementAttrs[nIndex];
	}

	public PDHeroMainGearRefinement ToPDHeroMainGearRefinement()
	{
		PDHeroMainGearRefinement inst = new PDHeroMainGearRefinement();
		inst.turn = m_nTurn;
		inst.attrs = HeroMainGearRefinementAttr.ToHeroMainGearRefinementAttrs(m_refinementAttrs).ToArray();
		return inst;
	}

	public static List<PDHeroMainGearRefinement> ToPDHeroMainGearRefinements(IEnumerable<HeroMainGearRefinement> refinements)
	{
		List<PDHeroMainGearRefinement> results = new List<PDHeroMainGearRefinement>();
		foreach (HeroMainGearRefinement refinement in refinements)
		{
			results.Add(refinement.ToPDHeroMainGearRefinement());
		}
		return results;
	}
}
