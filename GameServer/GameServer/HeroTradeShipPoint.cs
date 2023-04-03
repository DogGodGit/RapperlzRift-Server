using System;

namespace GameServer;

public class HeroTradeShipPoint
{
	private Hero m_hero;

	private int m_nDifficulty;

	private int m_nPoint;

	private DateTimeOffset m_updateTime = DateTimeOffset.MinValue;

	public Hero hero => m_hero;

	public int difficulty => m_nDifficulty;

	public int point => m_nPoint;

	public DateTimeOffset updateTime => m_updateTime;

	public void Init(Hero hero, TradeShipDifficulty difficulty)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_hero = hero;
		m_nDifficulty = difficulty.difficulty;
	}

	public void AddPoint(int nAmount, DateTimeOffset time)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nPoint += nAmount;
			m_updateTime = time;
		}
	}

	public int CompareTo(HeroTradeShipPoint other)
	{
		if (other == null)
		{
			return 1;
		}
		int nResult = m_nPoint.CompareTo(other.point);
		if (nResult == 0)
		{
			return -m_updateTime.CompareTo(other.updateTime);
		}
		return nResult;
	}
}
