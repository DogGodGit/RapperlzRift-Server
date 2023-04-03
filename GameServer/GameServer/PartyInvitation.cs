using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class PartyInvitation
{
	private long m_lnNo;

	private Party m_party;

	private Hero m_target;

	private PartyMember m_inviter;

	private Timer m_lifetimeTimer;

	private bool m_bReleased;

	public static readonly SFSynchronizedLongFactory noFactory = new SFSynchronizedLongFactory();

	public long no => m_lnNo;

	public Party party => m_party;

	public Hero target => m_target;

	public PartyMember inviter => m_inviter;

	public PartyInvitation(Party party, Hero target, PartyMember inviter, DateTimeOffset time)
	{
		m_lnNo = noFactory.NewValue();
		m_party = party;
		m_target = target;
		m_inviter = inviter;
		m_lifetimeTimer = new Timer(OnLifetimeTimerTick);
		m_lifetimeTimer.Change(Resource.instance.partyInvitationLifetime * 1000, -1);
	}

	private void OnLifetimeTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessLifetimeTimerTick));
	}

	private void ProcessLifetimeTimerTick()
	{
		if (m_bReleased)
		{
			return;
		}
		lock (m_target.syncObject)
		{
			m_target.OnPartyInvitationLifetimeEnded(this);
			m_party.OnInvitationLifetimeEnded(this);
		}
	}

	private void DisposeLifetimeTimer()
	{
		m_lifetimeTimer.Dispose();
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeLifetimeTimer();
			m_bReleased = true;
		}
	}

	public PDPartyInvitation ToPDPartyInvitation()
	{
		PDPartyInvitation inst = new PDPartyInvitation();
		inst.no = m_lnNo;
		inst.partyId = (Guid)m_party.id;
		inst.targetId = (Guid)m_target.id;
		inst.targetName = m_target.name;
		inst.inviterId = (Guid)m_inviter.id;
		inst.inviterName = m_inviter.name;
		return inst;
	}
}
