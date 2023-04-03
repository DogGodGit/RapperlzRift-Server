using System;

namespace GameServer;

public class CartReceivedDamage
{
	private Guid m_attackerId = Guid.Empty;

	private string m_sAttackerName;

	private long m_lnDamage;

	private DateTimeOffset m_time = DateTimeOffset.MinValue;

	public Guid attackerId => m_attackerId;

	public string attackerName => m_sAttackerName;

	public long damage => m_lnDamage;

	public DateTimeOffset time => m_time;

	public CartReceivedDamage(Guid attackerId, string sAttackerName)
	{
		m_attackerId = attackerId;
		m_sAttackerName = sAttackerName;
	}

	public void AddDamage(long lnDamage, DateTimeOffset time)
	{
		m_lnDamage += lnDamage;
		m_time = time;
	}
}
