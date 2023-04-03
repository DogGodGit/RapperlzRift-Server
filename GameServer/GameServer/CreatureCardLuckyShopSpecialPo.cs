using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardLuckyShopSpecialPoolEntry : IPickEntry
{
	private int m_nNo;

	private int m_nPoint;

	private CreatureCard m_creatureCard;

	public int no => m_nNo;

	public int point => m_nNo;

	public CreatureCard creatureCard => m_creatureCard;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
		m_creatureCard = Resource.instance.GetCreatureCard(nCreatureCardId);
		if (m_creatureCard == null)
		{
			SFLogUtil.Warn(GetType(), "크리처카드가 존재하지 않습니다. m_nNo = " + m_nNo + ", nCreatureCardId = " + nCreatureCardId);
		}
	}
}
