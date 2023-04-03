using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class Friend
{
	private Hero m_owner;

	private Guid m_id = Guid.Empty;

	private string m_sName;

	private int m_nNationId;

	private int m_nJobId;

	private int m_nLevel;

	private long m_lnBattlePower;

	private bool m_bIsLoggedIn;

	public Hero owner => m_owner;

	public Guid id => m_id;

	public string name => m_sName;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public int level => m_nLevel;

	public long battlePower => m_lnBattlePower;

	public bool isLoggedIn => m_bIsLoggedIn;

	public Friend(Hero owner)
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
		Init((Guid)dr["friendId"], Convert.ToString(dr["name"]), Convert.ToInt32(dr["nationId"]), Convert.ToInt32(dr["jobId"]), Convert.ToInt32(dr["level"]), Convert.ToInt64(dr["battlePower"]), bIsLoggedIn: false);
	}

	public void Init(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		Init(hero.id, hero.name, hero.nationId, hero.jobId, hero.level, hero.battlePower, bIsLoggedIn: true);
	}

	private void Init(Guid id, string sName, int nNationId, int nJobId, int nLevel, long lnBattlePower, bool bIsLoggedIn)
	{
		m_id = id;
		m_sName = sName;
		m_nNationId = nNationId;
		m_nJobId = nJobId;
		m_nLevel = nLevel;
		m_lnBattlePower = lnBattlePower;
		m_bIsLoggedIn = bIsLoggedIn;
	}

	private void Refresh()
	{
		Hero hero = Cache.instance.GetLoggedInHero(m_id);
		if (hero != null)
		{
			lock (hero.syncObject)
			{
				m_nJobId = hero.jobId;
				m_nLevel = hero.level;
				m_lnBattlePower = hero.battlePower;
			}
			m_bIsLoggedIn = true;
		}
		else
		{
			m_bIsLoggedIn = false;
		}
	}

	public PDFriend ToPDFriend()
	{
		Refresh();
		PDFriend inst = new PDFriend();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		inst.isLoggedIn = m_bIsLoggedIn;
		return inst;
	}

	public static List<PDFriend> ToPDFriends(IEnumerable<Friend> friends)
	{
		List<PDFriend> results = new List<PDFriend>();
		foreach (Friend friend in friends)
		{
			results.Add(friend.ToPDFriend());
		}
		return results;
	}
}
