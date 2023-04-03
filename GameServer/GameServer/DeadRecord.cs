using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class DeadRecord
{
	private Guid m_id = Guid.Empty;

	private Hero m_owner;

	private Guid m_killerId = Guid.Empty;

	private string m_sName;

	private int m_nNationId;

	private int m_nJobId;

	private int m_nLevel;

	private long m_lnBattlePower;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Guid id => m_id;

	public Hero owner => m_owner;

	public Guid killerId => m_killerId;

	public string name => m_sName;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public int level => m_nLevel;

	public long battlePower => m_lnBattlePower;

	public DateTimeOffset regTime => m_regTime;

	public DeadRecord(Hero owner)
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
		Init((Guid)dr["recordId"], (Guid)dr["killerId"], Convert.ToString(dr["name"]), Convert.ToInt32(dr["nationId"]), Convert.ToInt32(dr["jobId"]), Convert.ToInt32(dr["level"]), Convert.ToInt64(dr["battlePower"]), (DateTimeOffset)dr["regTime"]);
	}

	public void Init(Hero killer, DateTimeOffset regTime)
	{
		if (killer == null)
		{
			throw new ArgumentNullException("killer");
		}
		Init(Guid.NewGuid(), killer.id, killer.name, killer.nationId, killer.jobId, killer.level, killer.battlePower, regTime);
	}

	private void Init(Guid id, Guid killerId, string sName, int nNationId, int nJobId, int nLevel, long lnBattlePower, DateTimeOffset regTime)
	{
		m_id = id;
		m_killerId = killerId;
		m_sName = sName;
		m_nNationId = nNationId;
		m_nJobId = nJobId;
		m_nLevel = nLevel;
		m_lnBattlePower = lnBattlePower;
		m_regTime = regTime;
	}

	public float GetRegElapsedTime(DateTimeOffset time)
	{
		return (float)(time - m_regTime).TotalSeconds;
	}

	public PDDeadRecord ToPDDeadRecord(DateTimeOffset time)
	{
		PDDeadRecord inst = new PDDeadRecord();
		inst.id = (Guid)m_id;
		inst.killerId = (Guid)m_killerId;
		inst.name = m_sName;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		inst.regElapsedTime = GetRegElapsedTime(time);
		return inst;
	}

	public static List<PDDeadRecord> ToPDDeadRecords(IEnumerable<DeadRecord> records, DateTimeOffset time)
	{
		List<PDDeadRecord> results = new List<PDDeadRecord>();
		foreach (DeadRecord record in records)
		{
			results.Add(record.ToPDDeadRecord(time));
		}
		return results;
	}
}
