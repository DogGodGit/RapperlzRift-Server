using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeCollection : IPickEntry
{
	private int m_nId;

	private int m_nActivationItemCount;

	private Dictionary<int, CostumeCollectionEntry> m_entries = new Dictionary<int, CostumeCollectionEntry>();

	private Dictionary<int, CostumeCollectionAttr> m_attrs = new Dictionary<int, CostumeCollectionAttr>();

	public int id => m_nId;

	public int activationItemCount => m_nActivationItemCount;

	public int point => 10;

	public Dictionary<int, CostumeCollectionEntry> entries => m_entries;

	public Dictionary<int, CostumeCollectionAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["costumeCollectionId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "콜렉션ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nActivationItemCount = Convert.ToInt32(dr["activationItemCount"]);
		if (m_nActivationItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "활성화아이템수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nActivationItemCount = " + m_nActivationItemCount);
		}
	}

	public void AddEntry(CostumeCollectionEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry.costume.id, entry);
	}

	public CostumeCollectionEntry GetEntry(int nId)
	{
		if (!m_entries.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddAttr(CostumeCollectionAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr.attrId, attr);
	}

	public CostumeCollectionAttr GetAttr(int nId)
	{
		if (!m_attrs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
