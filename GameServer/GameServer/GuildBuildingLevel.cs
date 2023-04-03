using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildBuildingLevel
{
	private GuildBuilding m_building;

	private int m_nLevel;

	private int m_nNextLevelUpGuildBuildingPoint;

	private int m_nNextLevelUpGuildFund;

	public GuildBuilding building => m_building;

	public int level => m_nLevel;

	public int nextLevelUpGuildBuildingPoint => m_nNextLevelUpGuildBuildingPoint;

	public int nextLevelUpGuildFund => m_nNextLevelUpGuildFund;

	public bool isMaxLevel => this == m_building.lastLevel;

	public GuildBuildingLevel(GuildBuilding building)
	{
		m_building = building;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nNextLevelUpGuildBuildingPoint = Convert.ToInt32(dr["nextLevelUpGuildBuildingPoint"]);
		if (m_nNextLevelUpGuildBuildingPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업길드건설도가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpGuildBuildingPoint = " + m_nNextLevelUpGuildBuildingPoint);
		}
		m_nNextLevelUpGuildFund = Convert.ToInt32(dr["nextLevelUpGuildFund"]);
		if (m_nNextLevelUpGuildFund < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업길드자금이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpGuildFund = " + m_nNextLevelUpGuildFund);
		}
	}
}
