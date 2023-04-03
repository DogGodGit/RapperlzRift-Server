using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardShopRandomProduct : IPickEntry
{
	private int m_nId;

	private int m_nPoint;

	private CreatureCard m_creatureCard;

	public int id => m_nId;

	public int point => m_nPoint;

	public CreatureCard creatureCard => m_creatureCard;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "상품ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
		int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
		m_creatureCard = Resource.instance.GetCreatureCard(nCreatureCardId);
		if (m_creatureCard == null)
		{
			SFLogUtil.Warn(GetType(), "크리쳐카드가 존재하지 않습니다. m_nId = " + m_nId + ", nCreatureCardId = " + nCreatureCardId);
		}
	}
}
