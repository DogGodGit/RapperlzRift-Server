using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RankAttr
{
	private Rank m_rank;

	private int m_nId;

	private AttrValue m_attrValue;

	public Rank rank => m_rank;

	public int id => m_nId;

	public AttrValue attrValue => m_attrValue;

	public RankAttr(Rank rank)
	{
		if (rank == null)
		{
			throw new ArgumentNullException("rank");
		}
		m_rank = rank;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. rankNo = " + m_rank.no + ", m_nId = " + m_nId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		if (lnAttrValueId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성값ID가 유효하지 않습니다. rankNo = " + m_rank.no + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. rankNo = " + m_rank.no + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
