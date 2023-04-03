using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class GuildBuildingInstance
{
	private Guild m_guild;

	private GuildBuilding m_building;

	private GuildBuildingLevel m_level;

	public Guild guild => m_guild;

	public GuildBuilding building => m_building;

	public bool isLobby => m_building.id == 1;

	public bool isLaboratory => m_building.id == 2;

	public bool isShop => m_building.id == 3;

	public bool isTankFactory => m_building.id == 4;

	public GuildBuildingLevel level
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public GuildBuildingInstance()
		: this(null)
	{
	}

	public GuildBuildingInstance(Guild guild)
	{
		m_guild = guild;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nBuildingId = Convert.ToInt32(dr["buildingId"]);
		int nLevel = Convert.ToInt32(dr["level"]);
		GuildBuilding building = Resource.instance.GetGuildBuilding(nBuildingId);
		if (building == null)
		{
			throw new Exception(string.Concat("존재하지 않는 길드건물입니다. guildId = ", m_guild.id, ", nBuildingId = ", nBuildingId));
		}
		GuildBuildingLevel level = building.GetLevel(nLevel);
		m_building = building;
		m_level = level;
	}

	public void Create(Guild guild, GuildBuildingLevel level)
	{
		if (guild == null)
		{
			throw new ArgumentNullException("guild");
		}
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_guild = guild;
		m_building = level.building;
		m_level = level;
	}

	public PDGuildBuildingInstance ToPDGuildBuildingInstance()
	{
		PDGuildBuildingInstance inst = new PDGuildBuildingInstance();
		inst.buildingId = m_building.id;
		inst.level = m_level.level;
		return inst;
	}

	public static List<PDGuildBuildingInstance> ToPDGuildBuildingInstances(IEnumerable<GuildBuildingInstance> buildings)
	{
		List<PDGuildBuildingInstance> insts = new List<PDGuildBuildingInstance>();
		foreach (GuildBuildingInstance building in buildings)
		{
			insts.Add(building.ToPDGuildBuildingInstance());
		}
		return insts;
	}
}
