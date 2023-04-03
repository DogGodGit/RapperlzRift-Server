using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class BlacklistEntry
{
	private Hero m_owner;

	private Guid m_heroId = Guid.Empty;

	private string m_sName;

	private int m_nNationId;

	private int m_nJobId;

	private int m_nLevel;

	private long m_lnBattlePower;

	public Hero owner => m_owner;

	public Guid heroId => m_heroId;

	public string name => m_sName;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public int level => m_nLevel;

	public long battlePower => m_lnBattlePower;

	public BlacklistEntry(Hero owner)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		m_owner = owner;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Init((Guid)dr["targetHeroId"], Convert.ToString(dr["name"]), Convert.ToInt32(dr["nationId"]), Convert.ToInt32(dr["jobId"]), Convert.ToInt32(dr["level"]), Convert.ToInt64(dr["battlePower"]));
	}

	public void Init(Guid heroId, string sName, int nNationId, int nJobId, int nLevel, long lnBattlePower)
	{
		m_heroId = heroId;
		m_sName = sName;
		m_nNationId = nNationId;
		m_nJobId = nJobId;
		m_nLevel = nLevel;
		m_lnBattlePower = lnBattlePower;
	}

	public PDBlacklistEntry ToPDBlacklistEntry()
	{
		PDBlacklistEntry inst = new PDBlacklistEntry();
		inst.heroId = (Guid)m_heroId;
		inst.name = m_sName;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		return inst;
	}

	public static List<PDBlacklistEntry> ToPDBlacklistEntries(IEnumerable<BlacklistEntry> entries)
	{
		List<PDBlacklistEntry> results = new List<PDBlacklistEntry>();
		foreach (BlacklistEntry entry in entries)
		{
			results.Add(entry.ToPDBlacklistEntry());
		}
		return results;
	}
}
