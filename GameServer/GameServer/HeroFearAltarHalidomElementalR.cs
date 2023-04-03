using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HeroFearAltarHalidomElementalReward
{
	private Hero m_hero;

	private DateTime m_weekStartDate = DateTime.MinValue;

	private FearAltarHalidomElemental m_halidomElemental;

	public Hero hero => m_hero;

	public DateTime weekStartDate => m_weekStartDate;

	public FearAltarHalidomElemental halidomElemental => m_halidomElemental;

	public HeroFearAltarHalidomElementalReward(Hero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_weekStartDate = SFDBUtil.ToDateTime(dr["weekStartDate"], DateTime.MinValue);
		int nHalidomElementalId = Convert.ToInt32(dr["halidomElementalId"]);
		if (nHalidomElementalId > 0)
		{
			m_halidomElemental = Resource.instance.fearAltar.GetHalidomElemental(nHalidomElementalId);
			if (m_halidomElemental == null)
			{
				throw new Exception("성물원소가 존재하지 않습니다. nHalidomElementalId = " + nHalidomElementalId);
			}
			return;
		}
		throw new Exception("성물원소ID가 유효하지 않습니다. nHalidomElementalId = " + nHalidomElementalId);
	}

	public void Init(FearAltarHalidomElemental halidomElemental, DateTime weekStartDate)
	{
		if (halidomElemental == null)
		{
			throw new ArgumentNullException("halidomElemental");
		}
		m_halidomElemental = halidomElemental;
		m_weekStartDate = weekStartDate;
	}
}
