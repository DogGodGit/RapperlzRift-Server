using System;
using System.Collections.Generic;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class PartyMember
{
	private Party m_party;

	private Hero m_hero;

	private Guid m_id = Guid.Empty;

	private string m_sName;

	private int m_nNationId;

	private int m_nJobId;

	private int m_nLevel;

	private int m_nVipLevel;

	private long m_lnBattlePower;

	private int m_nMaxHP;

	private int m_nHP;

	private Timer m_logOutDurationTimer;

	private bool m_bReleased;

	public Party party => m_party;

	public Hero hero => m_hero;

	public bool isLoggedIn => m_hero != null;

	public Guid id => m_id;

	public string name => m_sName;

	public int nationId => m_nNationId;

	public int jobId => m_nJobId;

	public int level => m_nLevel;

	public int vipLevel => m_nVipLevel;

	public long battlePower => m_lnBattlePower;

	public int maxHP => m_nMaxHP;

	public int hp => m_nHP;

	public bool isMaster => m_id == m_party.master.id;

	public PartyMember(Party party, Hero hero)
	{
		m_party = party;
		m_hero = hero;
		m_id = hero.id;
		m_sName = hero.name;
		m_nNationId = hero.nationId;
		Refresh();
		m_logOutDurationTimer = new Timer(OnLogOutDurationTimerTick);
	}

	public void Refresh()
	{
		if (m_hero == null)
		{
			return;
		}
		lock (m_hero.syncObject)
		{
			m_nJobId = m_hero.jobId;
			m_nLevel = m_hero.level;
			m_nVipLevel = m_hero.vipLevel.level;
			m_lnBattlePower = m_hero.battlePower;
			m_nMaxHP = m_hero.realMaxHP;
			m_nHP = m_hero.hp;
		}
	}

	public void LogIn(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (isLoggedIn)
		{
			throw new InvalidOperationException("이미 로그인중입니다.");
		}
		m_hero = hero;
		Refresh();
		StopLogOutDurationTimer();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (PartyMember member in m_party.members)
		{
			if (!(member.id == m_hero.id))
			{
				m_hero.AddTempFriend(member.id, member.name, member.nationId, member.jobId, member.level, member.battlePower, currentTime);
			}
		}
	}

	public void LogOut()
	{
		if (isLoggedIn)
		{
			if (isMaster)
			{
				m_party.CancelAllInvitations();
				m_party.RefuseAllApplications();
			}
			Refresh();
			m_hero = null;
			StartLogOutDurationTimer();
		}
	}

	private void StartLogOutDurationTimer()
	{
		m_logOutDurationTimer.Change(Resource.instance.partyMemberLogOutDuration * 1000, -1);
	}

	private void StopLogOutDurationTimer()
	{
		m_logOutDurationTimer.Change(-1, -1);
	}

	private void DisposeLogOutDurationTimer()
	{
		m_logOutDurationTimer.Dispose();
	}

	private void OnLogOutDurationTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessLogOutDurationTimerTick));
	}

	private void ProcessLogOutDurationTimerTick()
	{
		if (!m_bReleased && m_hero == null)
		{
			m_party.Exit(this);
		}
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeLogOutDurationTimer();
			m_bReleased = true;
		}
	}

	public PDPartyMember ToPDPartyMember()
	{
		Refresh();
		PDPartyMember inst = new PDPartyMember();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.jobId = m_nJobId;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		inst.maxHP = m_nMaxHP;
		inst.hp = m_nHP;
		inst.isLoggedIn = isLoggedIn;
		return inst;
	}

	public PDSimpleHero ToPDSimpleHero()
	{
		Refresh();
		PDSimpleHero inst = new PDSimpleHero();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = m_nNationId;
		inst.jobId = m_nJobId;
		inst.level = m_nLevel;
		inst.vipLevel = m_nVipLevel;
		inst.battlePower = m_lnBattlePower;
		return inst;
	}

	public static List<PDPartyMember> ToPDPartyMembers(IEnumerable<PartyMember> members)
	{
		List<PDPartyMember> results = new List<PDPartyMember>();
		foreach (PartyMember member in members)
		{
			results.Add(member.ToPDPartyMember());
		}
		return results;
	}
}
