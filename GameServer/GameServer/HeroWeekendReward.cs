using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroWeekendReward
{
	private Hero m_hero;

	private DateTime m_weekStartDate = DateTime.MinValue.Date;

	private int m_nSelection1 = -1;

	private int m_nSelection2 = -1;

	private int m_nSelection3 = -1;

	private bool m_bRewarded;

	public Hero hero => m_hero;

	public DateTime weekStartDate => m_weekStartDate;

	public int selection1 => m_nSelection1;

	public int selection2 => m_nSelection2;

	public int selection3 => m_nSelection3;

	public bool rewarded
	{
		get
		{
			return m_bRewarded;
		}
		set
		{
			m_bRewarded = value;
		}
	}

	public bool isAnySelected
	{
		get
		{
			if (m_nSelection1 < 0 && m_nSelection2 < 0)
			{
				return m_nSelection3 >= 0;
			}
			return true;
		}
	}

	public HeroWeekendReward(Hero hero)
		: this(hero, DateTime.MinValue.Date)
	{
	}

	public HeroWeekendReward(Hero hero, DateTime weekStartDate)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_weekStartDate = weekStartDate;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_weekStartDate = Convert.ToDateTime(dr["weekStartDate"]);
		m_nSelection1 = Convert.ToInt32(dr["selection1"]);
		m_nSelection2 = Convert.ToInt32(dr["selection2"]);
		m_nSelection3 = Convert.ToInt32(dr["selection3"]);
		m_bRewarded = Convert.ToBoolean(dr["rewarded"]);
	}

	public void SetSelection(int nSelectionNo, int nSelectedNumber)
	{
		switch (nSelectionNo)
		{
		case 1:
			m_nSelection1 = nSelectedNumber;
			break;
		case 2:
			m_nSelection2 = nSelectedNumber;
			break;
		case 3:
			m_nSelection3 = nSelectedNumber;
			break;
		}
	}

	public int GetSelection(int nSelectionNo)
	{
		return nSelectionNo switch
		{
			1 => m_nSelection1, 
			2 => m_nSelection2, 
			3 => m_nSelection3, 
			_ => -1, 
		};
	}

	public bool IsSelected(int nSelectionNo)
	{
		return GetSelection(nSelectionNo) >= 0;
	}

	public int CalculateResultValue()
	{
		int nResultValue = 0;
		if (m_nSelection3 >= 0)
		{
			Cal(m_nSelection3, ref nResultValue);
		}
		if (m_nSelection2 >= 0)
		{
			Cal(m_nSelection2, ref nResultValue);
		}
		if (m_nSelection1 >= 0)
		{
			Cal(m_nSelection1, ref nResultValue);
		}
		return nResultValue;
	}

	public PDHeroWeekendReward ToPDHeroWeekendReward()
	{
		PDHeroWeekendReward inst = new PDHeroWeekendReward();
		inst.weekStartDate = (DateTime)m_weekStartDate;
		inst.selection1 = m_nSelection1;
		inst.selection2 = m_nSelection2;
		inst.selection3 = m_nSelection3;
		inst.rewarded = m_bRewarded;
		return inst;
	}

	private static void Cal(int nSelection, ref int nResultValue)
	{
		nResultValue = nResultValue * 10 + nSelection;
	}
}
