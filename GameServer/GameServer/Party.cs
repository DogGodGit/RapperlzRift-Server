using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Party
{
	public const int kMemberUpdateInterval = 1000;

	public const float kCallCoolTimeFactor = 0.9f;

	private Guid m_id = Guid.Empty;

	private NationInstance m_nationInst;

	private List<PartyMember> m_members = new List<PartyMember>();

	private PartyMember m_master;

	private bool m_bReleased;

	private Timer m_memberUpdateTimer;

	private Dictionary<long, PartyInvitation> m_invitations = new Dictionary<long, PartyInvitation>();

	private Dictionary<long, PartyApplication> m_applications = new Dictionary<long, PartyApplication>();

	private DateTimeOffset m_lastCallTime = DateTimeOffset.MinValue;

	public Guid id => m_id;

	public NationInstance nationInst => m_nationInst;

	public List<PartyMember> members => m_members;

	public bool isMemberFull => m_members.Count >= Resource.instance.partyMemberMaxCount;

	public PartyMember master => m_master;

	public Party()
	{
		m_id = Guid.NewGuid();
	}

	public void Init(Hero creator)
	{
		if (creator == null)
		{
			throw new ArgumentNullException("creator");
		}
		m_nationInst = creator.nationInst;
		PartyMember member = CreateMember(creator);
		AddMember(member);
		m_master = member;
		m_memberUpdateTimer = new Timer(OnMemberUpdateTimerTick);
		m_memberUpdateTimer.Change(1000, 1000);
		Cache.instance.AddParty(this);
	}

	private PartyMember CreateMember(Hero hero)
	{
		return hero.partyMember = new PartyMember(this, hero);
	}

	private void AddMember(PartyMember member)
	{
		m_members.Add(member);
		Cache.instance.AddPartyMember(member);
	}

	private void RemoveMember(PartyMember member)
	{
		m_members.Remove(member);
		if (member.hero != null)
		{
			member.hero.partyMember = null;
		}
		member.Release();
		Cache.instance.RemovePartyMember(member.id);
	}

	public PartyMember GetMember(Guid memberId)
	{
		foreach (PartyMember member in m_members)
		{
			if (member.id == memberId)
			{
				return member;
			}
		}
		return null;
	}

	public List<ClientPeer> GetClientPeers(Guid memberIdToExclude)
	{
		List<ClientPeer> results = new List<ClientPeer>();
		foreach (PartyMember member in m_members)
		{
			if (member.id != memberIdToExclude && member.hero != null)
			{
				results.Add(member.hero.account.peer);
			}
		}
		return results;
	}

	public List<PDPartyMember> GetPDPartyMembers()
	{
		return PartyMember.ToPDPartyMembers(m_members);
	}

	public void Enter(Hero hero, Guid memberIdToExcludeFromEventTarget)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("member");
		}
		List<ClientPeer> eventTargets = GetClientPeers(memberIdToExcludeFromEventTarget);
		PartyMember newMember = CreateMember(hero);
		AddMember(newMember);
		ServerEvent.SendPartyMemberEnter(eventTargets, newMember.ToPDPartyMember());
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (PartyMember member in m_members)
		{
			if (member.id == hero.id)
			{
				continue;
			}
			hero.AddTempFriend(member.id, member.name, member.nationId, member.jobId, member.level, member.battlePower, currentTime);
			Hero memberHero = member.hero;
			if (memberHero != null)
			{
				lock (memberHero.syncObject)
				{
					memberHero.AddTempFriend(hero, currentTime);
				}
			}
		}
	}

	public void Exit(PartyMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		RemoveMember(member);
		if (m_members.Count == 0)
		{
			Close();
			return;
		}
		if (member.isMaster)
		{
			ChangeMaster(SelectMaster(), Guid.Empty);
		}
		ServerEvent.SendPartyMemberExit(GetClientPeers(Guid.Empty), member.id, bBanished: false);
	}

	private PartyMember SelectMaster()
	{
		PartyMember selectedMember = null;
		foreach (PartyMember member in m_members)
		{
			if (member.isLoggedIn)
			{
				selectedMember = member;
				break;
			}
		}
		if (selectedMember == null)
		{
			selectedMember = m_members.First();
		}
		return selectedMember;
	}

	public void Banish(PartyMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		RemoveMember(member);
		ServerEvent.SendPartyMemberExit(GetClientPeers(m_master.id), member.id, bBanished: true);
		if (member.hero != null)
		{
			ServerEvent.SendPartyBanished(member.hero.account.peer);
		}
	}

	public void ChangeMaster(PartyMember targetMember, Guid memberIdToExcludeFromEventTarget)
	{
		if (targetMember == null)
		{
			throw new ArgumentNullException("targetMember");
		}
		m_members.Remove(targetMember);
		m_members.Insert(0, targetMember);
		m_master = targetMember;
		CancelAllInvitations();
		RefuseAllApplications();
		ServerEvent.SendPartyMasterChanged(GetClientPeers(memberIdToExcludeFromEventTarget), m_master.id, GetCallRemainingCoolTime(DateTimeUtil.currentTime));
	}

	public void Disband()
	{
		List<ClientPeer> eventTargets = new List<ClientPeer>();
		PartyMember[] array = m_members.ToArray();
		foreach (PartyMember member in array)
		{
			if (member.hero != null)
			{
				lock (member.hero.syncObject)
				{
					RemoveMember(member);
				}
				if (!member.isMaster)
				{
					eventTargets.Add(member.hero.account.peer);
				}
			}
			else
			{
				RemoveMember(member);
			}
		}
		Close();
		ServerEvent.SendPartyDisbanded(eventTargets);
	}

	private void Close()
	{
		CancelAllInvitations();
		RefuseAllApplications();
		Release();
		Cache.instance.RemoveParty(m_id);
	}

	private void DisposeMemberUpdateTimer()
	{
		if (m_memberUpdateTimer != null)
		{
			m_memberUpdateTimer.Dispose();
			m_memberUpdateTimer = null;
		}
	}

	private void OnMemberUpdateTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessMemberUpdateTimerTick));
	}

	private void ProcessMemberUpdateTimerTick()
	{
		if (!m_bReleased)
		{
			ServerEvent.SendPartyMembersUpdated(GetClientPeers(Guid.Empty), GetPDPartyMembers().ToArray());
		}
	}

	public void Release()
	{
		if (m_bReleased)
		{
			return;
		}
		DisposeMemberUpdateTimer();
		foreach (PartyInvitation invitation in m_invitations.Values)
		{
			invitation.Release();
		}
		m_bReleased = true;
	}

	public PartyInvitation GetInvitation(long lnNo)
	{
		if (!m_invitations.TryGetValue(lnNo, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsInvitationForHero(Guid heroId)
	{
		foreach (PartyInvitation invitation in m_invitations.Values)
		{
			if (invitation.target.id == heroId)
			{
				return true;
			}
		}
		return false;
	}

	public PartyInvitation Invite(Hero target, DateTimeOffset time)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		PartyInvitation invitation = new PartyInvitation(this, target, m_master, time);
		AddInvitation(invitation);
		target.OnPartyInvited(invitation);
		return invitation;
	}

	private void AddInvitation(PartyInvitation invitation)
	{
		m_invitations.Add(invitation.no, invitation);
	}

	private void RemoveInvitation(PartyInvitation invitation)
	{
		m_invitations.Remove(invitation.no);
		invitation.Release();
	}

	public void OnInvitationLifetimeEnded(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveInvitation(invitation);
		if (m_master.hero != null)
		{
			ServerEvent.SendPartyInvitationLifetimeEnded(m_master.hero.account.peer, invitation.no);
		}
	}

	public void OnInvitationAccepted(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveInvitation(invitation);
		ServerEvent.SendPartyInvitationAccepted(m_master.hero.account.peer, invitation.no);
		Enter(invitation.target, Guid.Empty);
	}

	public void OnInvitationRefused(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveInvitation(invitation);
		ServerEvent.SendPartyInvitationRefused(m_master.hero.account.peer, invitation.no);
	}

	public void CancelInvitation(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveInvitation(invitation);
		invitation.target.OnPartyInvitationCanceled(invitation);
	}

	public void CancelAllInvitations()
	{
		PartyInvitation[] array = m_invitations.Values.ToArray();
		foreach (PartyInvitation invitation in array)
		{
			lock (invitation.target.syncObject)
			{
				CancelInvitation(invitation);
			}
		}
	}

	public PartyApplication GetApplication(long lnNo)
	{
		if (!m_applications.TryGetValue(lnNo, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddApplication(PartyApplication app)
	{
		m_applications.Add(app.no, app);
	}

	private void RemoveApplication(long lnNo)
	{
		m_applications.Remove(lnNo);
	}

	public void OnApplied(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		AddApplication(app);
		ServerEvent.SendPartyApplicationArrived(m_master.hero.account.peer, app.ToPDPartyApplication());
	}

	public void OnApplicationLifetimeEnded(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveApplication(app.no);
		if (m_master.hero != null)
		{
			ServerEvent.SendPartyApplicationLifetimeEnded(m_master.hero.account.peer, app.no);
		}
	}

	public void AcceptApplication(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveApplication(app.no);
		Enter(app.applicant, m_master.id);
		app.applicant.OnPartyApplicationAccepted(app);
	}

	public void RefuseApplication(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveApplication(app.no);
		app.applicant.OnPartyApplicationRefused(app);
	}

	public void RefuseAllApplications()
	{
		PartyApplication[] array = m_applications.Values.ToArray();
		foreach (PartyApplication app in array)
		{
			lock (app.applicant.syncObject)
			{
				RefuseApplication(app);
			}
		}
	}

	public void OnApplicationCanceled(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveApplication(app.no);
		ServerEvent.SendPartyApplicationCanceled(m_master.hero.account.peer, app.no);
	}

	public float GetCallRemainingCoolTime(DateTimeOffset time)
	{
		return (float)Math.Max((double)Resource.instance.partyCallCoolTime - (time - m_lastCallTime).TotalSeconds, 0.0);
	}

	public bool IsCallCoolTimeElapsed(DateTimeOffset time)
	{
		return (time - m_lastCallTime).TotalSeconds >= (double)((float)Resource.instance.partyCallCoolTime * 0.9f);
	}

	public void Call(DateTimeOffset time)
	{
		m_lastCallTime = time;
		ContinentInstance currentPlace = (ContinentInstance)m_master.hero.currentPlace;
		ServerEvent.SendPartyCall(GetClientPeers(m_master.id), currentPlace.continent.id, currentPlace.nationId, m_master.hero.position);
	}

	public PDParty ToPDParty(DateTimeOffset time)
	{
		PDParty inst = new PDParty();
		inst.id = (Guid)m_id;
		inst.members = GetPDPartyMembers().ToArray();
		inst.masterId = (Guid)m_master.id;
		inst.callRemainingCoolTime = GetCallRemainingCoolTime(time);
		return inst;
	}

	public PDSimpleParty ToPDSimpleParty()
	{
		PDSimpleParty inst = new PDSimpleParty();
		inst.id = (Guid)m_id;
		inst.master = m_master.ToPDSimpleHero();
		inst.memberCount = m_members.Count;
		return inst;
	}
}
