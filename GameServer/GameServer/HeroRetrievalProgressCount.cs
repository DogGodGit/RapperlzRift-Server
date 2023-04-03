using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroRetrievalProgressCount
{
	private HeroRetrievalProgressCountCollection m_collection;

	private Retrieval m_retrieval;

	private int m_nProgressCount;

	public HeroRetrievalProgressCountCollection collection => m_collection;

	public Retrieval retrieval => m_retrieval;

	public int progressCount => m_nProgressCount;

	public HeroRetrievalProgressCount(HeroRetrievalProgressCountCollection collection)
		: this(collection, null, 0)
	{
	}

	public HeroRetrievalProgressCount(HeroRetrievalProgressCountCollection collection, Retrieval retrieval, int nProgressCount)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_collection = collection;
		m_retrieval = retrieval;
		m_nProgressCount = nProgressCount;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nRetrievalId = Convert.ToInt32(dr["retrievalId"]);
		m_retrieval = Resource.instance.GetRetrieval(nRetrievalId);
		if (m_retrieval == null)
		{
			throw new Exception("회수가 존재하지 않습니다. nRetrievalId = " + nRetrievalId);
		}
		m_nProgressCount = Convert.ToInt32(dr["ProgressCount"]);
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		ServerEvent.SendRetrievalProgressCountUpdated(m_collection.hero.account.peer, ToPDHeroRetreivalProgressCount());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_collection.hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroRetrievalProgressCount(m_collection.hero.id, m_retrieval.id, m_collection.date, m_nProgressCount));
		dbWork.Schedule();
	}

	public PDHeroRetrievalProgressCount ToPDHeroRetreivalProgressCount()
	{
		PDHeroRetrievalProgressCount inst = new PDHeroRetrievalProgressCount();
		inst.date = (DateTime)m_collection.date;
		inst.retrievalId = m_retrieval.id;
		inst.prorgressCount = m_nProgressCount;
		return inst;
	}

	public static List<PDHeroRetrievalProgressCount> ToPDHeroRetrievalProrgressCounts(IEnumerable<HeroRetrievalProgressCount> progressCounts)
	{
		List<PDHeroRetrievalProgressCount> insts = new List<PDHeroRetrievalProgressCount>();
		foreach (HeroRetrievalProgressCount progressCount in progressCounts)
		{
			insts.Add(progressCount.ToPDHeroRetreivalProgressCount());
		}
		return insts;
	}
}
