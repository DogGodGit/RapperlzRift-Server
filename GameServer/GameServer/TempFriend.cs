using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class TempFriend
{
	private Hero m_owner;

	private Guid m_id = Guid.Empty;

	private string m_sName;

	private int m_nNationId;

	private int m_nJobId;

	private int m_nLevel;

	private long m_lnBattlePower;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Hero owner => m_owner;

	public Guid id => m_id;

	public string name => m_sName;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public int level => m_nLevel;

	public long battlePower => m_lnBattlePower;

	public DateTimeOffset regTime => m_regTime;

	public TempFriend(Hero owner)
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
		Init((Guid)dr["friendId"], Convert.ToString(dr["name"]), Convert.ToInt32(dr["nationId"]), Convert.ToInt32(dr["jobId"]), Convert.ToInt32(dr["level"]), Convert.ToInt64(dr["battlePower"]), (DateTimeOffset)dr["regTime"]);
	}

	public void Init(Guid id, string sName, int nNationId, int nJobId, int nLevel, long lnBattlePower, DateTimeOffset regTime)
	{
		m_id = id;
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

	public PDTempFriend ToPDTempFriend(DateTimeOffset time)
	{
		PDTempFriend inst = new PDTempFriend();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		inst.regElapsedTime = GetRegElapsedTime(time);
		return inst;
	}

	public static List<PDTempFriend> ToPDTempFriends(IEnumerable<TempFriend> friends, DateTimeOffset time)
	{
		List<PDTempFriend> results = new List<PDTempFriend>();
		foreach (TempFriend friend in friends)
		{
			results.Add(friend.ToPDTempFriend(time));
		}
		return results;
	}
}
