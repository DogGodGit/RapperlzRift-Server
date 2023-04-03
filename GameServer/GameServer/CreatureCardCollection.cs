using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardCollection
{
	private int m_nId;

	private CreatureCardCollectionCategory m_category;

	private CreatureCardCollectionGrade m_grade;

	private List<CreatureCardCollectionEntry> m_entries = new List<CreatureCardCollectionEntry>();

	private List<CreatureCardCollectionAttr> m_attrs = new List<CreatureCardCollectionAttr>();

	public int id => m_nId;

	public CreatureCardCollectionCategory category => m_category;

	public CreatureCardCollectionGrade grade => m_grade;

	public List<CreatureCardCollectionEntry> entries => m_entries;

	public List<CreatureCardCollectionAttr> attrs => m_attrs;

	public CreatureCardCollection(CreatureCardCollectionCategory category)
	{
		m_category = category;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["collectionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "컬렉션ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetCreatureCardCollectionGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "크리처카드컬렉션등급이 존재하지 않습니다. m_nId = " + m_nId + ", nGrade = " + nGrade);
		}
	}

	public void AddEntry(CreatureCardCollectionEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		entry.card.AddRelatedCreatureCardCollection(this);
	}

	public void AddAttr(CreatureCardCollectionAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
	}
}
