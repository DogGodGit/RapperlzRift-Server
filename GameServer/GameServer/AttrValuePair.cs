using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class AttrValuePair : IAttrValuePair
{
	private int m_nId;

	private int m_nValue;

	public int id
	{
		get
		{
			return m_nId;
		}
		set
		{
			m_nId = value;
		}
	}

	public int value
	{
		get
		{
			return m_nValue;
		}
		set
		{
			m_nValue = value;
		}
	}

	public AttrValuePair(int nId, int nValue)
	{
		m_nId = nId;
		m_nValue = nValue;
	}

	public PDAttrValuePair ToPDAttrValuePair()
	{
		PDAttrValuePair inst = new PDAttrValuePair();
		inst.id = m_nId;
		inst.value = m_nValue;
		return inst;
	}

	public static List<PDAttrValuePair> ToPDAttrValuePairs(IEnumerable<AttrValuePair> attrValuePairs)
	{
		List<PDAttrValuePair> insts = new List<PDAttrValuePair>();
		foreach (AttrValuePair attrValuePair in attrValuePairs)
		{
			insts.Add(attrValuePair.ToPDAttrValuePair());
		}
		return insts;
	}
}
