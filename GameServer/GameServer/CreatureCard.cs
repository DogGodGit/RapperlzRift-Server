using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCard
{
	private int m_nId;

	private int m_nCategroyId;

	private CreatureCardGrade m_grade;

	private int m_nAttack;

	private int m_nLife;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	private Dictionary<int, CreatureCardCollection> m_relatedCollections = new Dictionary<int, CreatureCardCollection>();

	public int id => m_nId;

	public int categoryId => m_nCategroyId;

	public CreatureCardGrade grade => m_grade;

	public int attack => m_nAttack;

	public int life => m_nLife;

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	public int value
	{
		get
		{
			if (m_attrValue == null)
			{
				return 0;
			}
			return m_attrValue.value;
		}
	}

	public Dictionary<int, CreatureCardCollection> relatedCollections => m_relatedCollections;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["creatureCardId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "크리처카드ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nCategroyId = Convert.ToInt32(dr["categoryId"]);
		if (m_nCategroyId < 0)
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nCategroyId = " + m_nCategroyId);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetCreatureCardGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. m_nId = " + m_nId + ", nGrade = " + nGrade);
		}
		m_nAttack = Convert.ToInt32(dr["attack"]);
		if (m_nAttack < 0)
		{
			SFLogUtil.Warn(GetType(), "공격력이 유효하지 않습니다.  m_nId = " + m_nId + ", m_nAttack = " + m_nAttack);
		}
		m_nLife = Convert.ToInt32(dr["life"]);
		if (m_nLife < 0)
		{
			SFLogUtil.Warn(GetType(), "생명력이 유효하지 않습니다.  m_nId = " + m_nId + ", m_nLife = " + m_nLife);
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nAttrId = " + m_nAttrId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 유효하지 않습니다. m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}

	public void AddRelatedCreatureCardCollection(CreatureCardCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_relatedCollections.Add(collection.id, collection);
	}
}
