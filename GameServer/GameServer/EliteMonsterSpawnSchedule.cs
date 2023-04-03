using System;
using System.Data;

namespace GameServer;

public class EliteMonsterSpawnSchedule
{
	private EliteMonsterMaster m_master;

	private int m_nNo;

	private int m_nSpawnTime;

	public EliteMonsterMaster master => m_master;

	public int no => m_nNo;

	public int spawnTime => m_nSpawnTime;

	public EliteMonsterSpawnSchedule(EliteMonsterMaster master)
	{
		m_master = master;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["scheduleNo"]);
		m_nSpawnTime = Convert.ToInt32(dr["spawnTime"]);
	}
}
