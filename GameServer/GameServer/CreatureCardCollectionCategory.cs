using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardCollectionCategory
{
	private int m_nId;

	private Dictionary<int, CreatureCardCollection> m_collections = new Dictionary<int, CreatureCardCollection>();

	public int id => m_nId;

	public Dictionary<int, CreatureCardCollection> collections => m_collections;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["categoryId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
	}

	public void AddCollection(CreatureCardCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_collections.Add(collection.id, collection);
	}

	public CreatureCardCollection GetCollection(int nId)
	{
		if (!m_collections.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
