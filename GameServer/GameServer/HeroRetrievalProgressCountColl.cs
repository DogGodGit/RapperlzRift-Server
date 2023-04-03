using System;
using System.Collections.Generic;

namespace GameServer;

public class HeroRetrievalProgressCountCollection
{
	private Hero m_hero;

	private DateTime m_date = DateTime.MinValue.Date;

	private Dictionary<int, HeroRetrievalProgressCount> m_progressCounts = new Dictionary<int, HeroRetrievalProgressCount>();

	public Hero hero => m_hero;

	public DateTime date
	{
		get
		{
			return m_date;
		}
		set
		{
			m_date = value;
		}
	}

	public Dictionary<int, HeroRetrievalProgressCount> progressCounts => m_progressCounts;

	public HeroRetrievalProgressCountCollection(Hero hero, DateTime date)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_date = date;
	}

	public void AddProgressCount(HeroRetrievalProgressCount progressCount)
	{
		if (progressCount == null)
		{
			throw new ArgumentNullException("progressCount");
		}
		m_progressCounts.Add(progressCount.retrieval.id, progressCount);
	}

	public HeroRetrievalProgressCount GetProgressCount(int nId)
	{
		if (!m_progressCounts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroRetrievalProgressCount GetOrCreateProgressCount(int nId)
	{
		HeroRetrievalProgressCount progressCount = GetProgressCount(nId);
		if (progressCount == null)
		{
			Retrieval retrieval = Resource.instance.GetRetrieval(nId);
			if (retrieval == null)
			{
				return null;
			}
			progressCount = new HeroRetrievalProgressCount(this, retrieval, 0);
			AddProgressCount(progressCount);
		}
		return progressCount;
	}

	public void ProcessProgressCount(int nId)
	{
		GetOrCreateProgressCount(nId)?.IncreaseProgressCount();
	}

	public void ClearProgressCount()
	{
		m_progressCounts.Clear();
	}
}
