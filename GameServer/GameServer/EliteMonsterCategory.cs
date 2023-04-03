using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class EliteMonsterCategory
{
	private int m_nId;

	private int m_nRecommendMinHeroLevel;

	private int m_nRecommendMaxHeroLevel;

	private Continent m_continent;

	private Dictionary<int, EliteMonsterMaster> m_masters = new Dictionary<int, EliteMonsterMaster>();

	public int id => m_nId;

	public int recommendMinHeroLevel => m_nRecommendMinHeroLevel;

	public int recommendMaxHeroLevel => m_nRecommendMaxHeroLevel;

	public Continent continent => m_continent;

	public int continentId => m_continent.id;

	public Dictionary<int, EliteMonsterMaster> masters => m_masters;

	public int masterCount => m_masters.Count;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["categoryId"]);
		m_nRecommendMinHeroLevel = Convert.ToInt32(dr["recommendMinHeroLevel"]);
		if (m_nRecommendMinHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "권장최소레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRecommendMinHeroLevel = " + m_nRecommendMinHeroLevel);
		}
		m_nRecommendMaxHeroLevel = Convert.ToInt32(dr["recommendMaxHeroLevel"]);
		if (m_nRecommendMaxHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "권장최대레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRecommendMaxHeroLevel = " + m_nRecommendMaxHeroLevel);
		}
		if (m_nRecommendMinHeroLevel > m_nRecommendMaxHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "권장최소레벨이 권장최대레벨보다 높습니다. m_nId = " + m_nId + ", m_nRecommendMinHeroLevel = " + m_nRecommendMinHeroLevel + ", m_nRecommendMaxHeroLevel = " + m_nRecommendMaxHeroLevel);
		}
		int nContinentId = Convert.ToInt32(dr["continentId"]);
		if (nContinentId > 0)
		{
			m_continent = Resource.instance.GetContinent(nContinentId);
			if (m_continent == null)
			{
				SFLogUtil.Warn(GetType(), "대륙이 존재하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
			}
			else if (!m_continent.isNationTerritory)
			{
				SFLogUtil.Warn(GetType(), "국가 소속의 대륙이 아닙니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대륙ID가 유효하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
		}
	}

	public void AddMaster(EliteMonsterMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_masters.Add(master.id, master);
	}
}
