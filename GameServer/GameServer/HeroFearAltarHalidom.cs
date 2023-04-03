using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HeroFearAltarHalidom
{
	private Hero m_hero;

	private DateTime m_weekStartDate = DateTime.MinValue;

	private FearAltarHalidom m_halidom;

	public Hero hero => m_hero;

	public DateTime weekStartDate => m_weekStartDate;

	public FearAltarHalidom halidom => m_halidom;

	public HeroFearAltarHalidom(Hero hero)
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
		int nHalidomId = Convert.ToInt32(dr["halidomId"]);
		if (nHalidomId > 0)
		{
			m_halidom = Resource.instance.fearAltar.GetHalidom(nHalidomId);
			if (m_halidom == null)
			{
				throw new Exception("성물이 존재하지 않습니다. nHalidomId = " + nHalidomId);
			}
			return;
		}
		throw new Exception("성물ID가 유효하지 않습니다. nHalidomId = " + nHalidomId);
	}

	public void Init(DateTime weekStartDate, FearAltarHalidom halidom)
	{
		if (halidom == null)
		{
			throw new ArgumentNullException("halidom");
		}
		m_weekStartDate = weekStartDate;
		m_halidom = halidom;
	}
}
