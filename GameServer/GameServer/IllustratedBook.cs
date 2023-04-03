using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class IllustratedBook
{
	public const float kRadiusFactor = 1.1f;

	private int m_nId;

	private IllustratedBookType m_type;

	private int m_nExplorationPoint;

	private List<IllustratedBookAttr> m_attrs = new List<IllustratedBookAttr>();

	public int id => m_nId;

	public IllustratedBookType type => m_type;

	public int explorationPoint => m_nExplorationPoint;

	public List<IllustratedBookAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["illustratedBookId"]);
		int nType = Convert.ToInt32(dr["type"]);
		if (nType > 0)
		{
			m_type = Resource.instance.GetIllustratedBookType(nType);
			if (m_type == null)
			{
				SFLogUtil.Warn(GetType(), "타입이 존재하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		m_nExplorationPoint = Convert.ToInt32(dr["explorationPoint"]);
	}

	public void AddAttrs(IllustratedBookAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
	}
}
