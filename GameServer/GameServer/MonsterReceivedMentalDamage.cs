using System;

namespace GameServer;

public class MonsterReceivedMentalDamage
{
	private Guid m_attackerId = Guid.Empty;

	private string m_sAttackerName;

	private int m_nDamage;

	private DateTimeOffset m_time = DateTimeOffset.MinValue;

	private int m_nRank;

	public Guid attackerId => m_attackerId;

	public string attackerName => m_sAttackerName;

	public int damage => m_nDamage;

	public DateTimeOffset time => m_time;

	public int rank
	{
		get
		{
			return m_nRank;
		}
		set
		{
			m_nRank = value;
		}
	}

	public MonsterReceivedMentalDamage(Guid attackerId, string sAttackerName)
	{
		m_attackerId = attackerId;
		m_sAttackerName = sAttackerName;
	}

	public void AddDamage(int nDamage, DateTimeOffset time)
	{
		m_nDamage += nDamage;
		m_time = time;
	}

	public int CompareTo(MonsterReceivedMentalDamage other)
	{
		if (other == null)
		{
			return 1;
		}
		int nResult = m_nDamage.CompareTo(other.damage);
		if (nResult == 0)
		{
			return -m_time.CompareTo(other.time);
		}
		return nResult;
	}

	public static int Compare(MonsterReceivedMentalDamage x, MonsterReceivedMentalDamage y)
	{
		if (x == null)
		{
			if (y != null)
			{
				return -1;
			}
			return 0;
		}
		return x.CompareTo(y);
	}
}
