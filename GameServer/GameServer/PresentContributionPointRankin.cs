using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class PresentContributionPointRanking
{
	private int m_nRanking;

	private Guid m_heroId = Guid.Empty;

	private int m_nNationId;

	private int m_nJobId;

	private string m_sName;

	private int m_nLevel;

	private int m_nContributionPoint;

	public int ranking => m_nRanking;

	public Guid heroId => m_heroId;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public string name => m_sName;

	public int level => m_nLevel;

	public int contributionPoint => m_nContributionPoint;

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
		m_nContributionPoint = Convert.ToInt32(dr["contributionPoint"]);
	}

	public PDPresentContributionPointRanking ToPDRanking()
	{
		PDPresentContributionPointRanking inst = new PDPresentContributionPointRanking();
		inst.ranking = m_nRanking;
		inst.heroId = (Guid)m_heroId;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.name = m_sName;
		inst.level = m_nLevel;
		inst.contributionPoint = m_nContributionPoint;
		return inst;
	}
}
