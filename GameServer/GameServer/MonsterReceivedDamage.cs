using System;

namespace GameServer;

public class MonsterReceivedDamage
{
	private Guid m_attackerId = Guid.Empty;

	private string m_sAttackerName;

	private long m_lnDamage;

	private DateTimeOffset m_time = DateTimeOffset.MinValue;

	private int m_nRank;

	public Guid attackerId => m_attackerId;

	public string attackerName => m_sAttackerName;

	public long damage => m_lnDamage;

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

	public MonsterReceivedDamage(Guid attackerId, string sAttackerName)
	{
		m_attackerId = attackerId;
		m_sAttackerName = sAttackerName;
	}

	public void AddDamage(long lnDamage, DateTimeOffset time)
	{
		m_lnDamage += lnDamage;
		m_time = time;
	}

	public int CompareTo(MonsterReceivedDamage other)
	{
		if (other == null)
		{
			return 1;
		}
		int nResult = m_lnDamage.CompareTo(other.damage);
		if (nResult == 0)
		{
			return -m_time.CompareTo(other.time);
		}
		return nResult;
	}

	public static int Compare(MonsterReceivedDamage x, MonsterReceivedDamage y)
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
