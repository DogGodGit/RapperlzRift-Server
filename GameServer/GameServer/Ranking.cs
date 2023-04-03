using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class Ranking
{
	private int m_nRanking;

	private Guid m_heroId = Guid.Empty;

	private int m_nNationId;

	private int m_nJobId;

	private string m_sName;

	private int m_nLevel;

	private long m_lnBattlePower;

	private long m_lnExp;

	private int m_nExploitPoint;

	private int m_nCollectionFamePoint;

	private int m_nExplorationPoint;

	public int ranking => m_nRanking;

	public Guid heroId => m_heroId;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public string name => m_sName;

	public int level => m_nLevel;

	public long battlePower => m_lnBattlePower;

	public long exp => m_lnExp;

	public int exploitPoint => m_nExploitPoint;

	public int collectionFamePoint => m_nCollectionFamePoint;

	public int explorationPoint => m_nExplorationPoint;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRanking = Convert.ToInt32(dr["ranking"]);
		m_heroId = (Guid)dr["heroId"];
		m_nNationId = Convert.ToInt32(dr["nationId"]);
		m_nJobId = Convert.ToInt32(dr["jobId"]);
		m_sName = Convert.ToString(dr["name"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (dr.Table.Columns.Contains("battlePower"))
		{
			m_lnBattlePower = Convert.ToInt64(dr["battlePower"]);
		}
		if (dr.Table.Columns.Contains("exp"))
		{
			m_lnExp = Convert.ToInt64(dr["exp"]);
		}
		if (dr.Table.Columns.Contains("exploitPoint"))
		{
			m_nExploitPoint = Convert.ToInt32(dr["exploitPoint"]);
		}
		if (dr.Table.Columns.Contains("collectionFamePoint"))
		{
			m_nCollectionFamePoint = Convert.ToInt32(dr["collectionFamePoint"]);
		}
		if (dr.Table.Columns.Contains("explorationPoint"))
		{
			m_nExplorationPoint = Convert.ToInt32(dr["explorationPoint"]);
		}
	}

	public PDRanking ToPDRanking()
	{
		PDRanking inst = new PDRanking();
		inst.ranking = m_nRanking;
		inst.heroId = (Guid)m_heroId;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.name = m_sName;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		inst.exp = m_lnExp;
		inst.exploitPoint = m_nExploitPoint;
		inst.collectionFamePoint = m_nCollectionFamePoint;
		inst.explorationPoint = m_nExplorationPoint;
		return inst;
	}
}
