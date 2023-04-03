using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeCollectionEntry
{
	private CostumeCollection m_collection;

	private Costume m_costume;

	public CostumeCollection collection => m_collection;

	public Costume costume => m_costume;

	public CostumeCollectionEntry(CostumeCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_collection = collection;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nCollectionId = Convert.ToInt32(dr["costumeCollectionId"]);
		m_collection = Resource.instance.GetCostumeCollection(nCollectionId);
		if (m_collection == null)
		{
			SFLogUtil.Warn(GetType(), "콜렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId);
		}
		int nCostumeId = Convert.ToInt32(dr["costumeId"]);
		m_costume = Resource.instance.GetCostume(nCostumeId);
		if (m_costume == null)
		{
			SFLogUtil.Warn(GetType(), "코스튬이 존재하지 않습니다. nCollectionId = " + nCollectionId + ", nCostumeId = " + nCostumeId);
		}
	}
}
