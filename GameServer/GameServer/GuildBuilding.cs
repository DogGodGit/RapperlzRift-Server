using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class GuildBuilding
{
	public const int kId_Lobby = 1;

	public const int kId_Laboratory = 2;

	public const int kId_Shop = 3;

	public const int kId_TankFactory = 4;

	private int m_nId;

	private string m_sNameKey;

	private List<GuildBuildingLevel> m_levels = new List<GuildBuildingLevel>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public List<GuildBuildingLevel> levels => m_levels;

	public GuildBuildingLevel firstLevel => m_levels.FirstOrDefault();

	public GuildBuildingLevel lastLevel => m_levels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["buildingId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "건물ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}

	public void AddLevel(GuildBuildingLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
	}

	public GuildBuildingLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - firstLevel.level;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}
}
