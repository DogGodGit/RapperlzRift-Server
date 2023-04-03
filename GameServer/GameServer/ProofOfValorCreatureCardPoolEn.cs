using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ProofOfValorCreatureCardPoolEntry : IPickEntry
{
	private ProofOfValorCreatureCardPool m_pool;

	private int m_nId;

	private int m_nPoint;

	private CreatureCard m_creatureCard;

	public ProofOfValorCreatureCardPool pool => m_pool;

	public int id => m_nId;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public CreatureCard creatureCard => m_creatureCard;

	public ProofOfValorCreatureCardPoolEntry(ProofOfValorCreatureCardPool pool)
	{
		m_pool = pool;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["entryId"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
		if (nCreatureCardId > 0)
		{
			m_creatureCard = Resource.instance.GetCreatureCard(nCreatureCardId);
			if (m_creatureCard == null)
			{
				SFLogUtil.Warn(GetType(), "크리처가드가 존재하지 않습니다. poolId = " + m_pool.id + ", m_nId = " + m_nId + ", nCreatureCardId = " + nCreatureCardId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "크리처카드ID가 유효하지 않습니다. poolId = " + m_pool.id + ", m_nId = " + m_nId + ", nCreatureCardId = " + nCreatureCardId);
		}
	}
}
