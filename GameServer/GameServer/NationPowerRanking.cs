using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class NationPowerRanking
{
	private int m_nRanking;

	private int m_nNationId;

	private int m_nNationPower;

	private int m_nNationWarPoint;

	private long m_lnBattlePower;

	public int ranking => m_nRanking;

	public int nationId => m_nNationId;

	public int nationPower => m_nNationPower;

	public int nationWarPoint => m_nNationWarPoint;

	public long battlePower => m_lnBattlePower;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRanking = Convert.ToInt32(dr["ranking"]);
		m_nNationId = Convert.ToInt32(dr["nationId"]);
		m_nNationPower = Convert.ToInt32(dr["nationPower"]) + Resource.instance.nationBasePower;
		m_nNationWarPoint = Convert.ToInt32(dr["nationWarPoint"]);
		m_lnBattlePower = Convert.ToInt32(dr["battlePower"]);
	}

	public PDNationPowerRanking ToPDNationPowerRanking()
	{
		PDNationPowerRanking inst = new PDNationPowerRanking();
		inst.ranking = m_nRanking;
		inst.nationId = m_nNationId;
		inst.nationPower = m_nNationPower;
		return inst;
	}
}
