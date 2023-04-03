using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class PartyApplication
{
	private long m_lnNo;

	private Party m_party;

	private Hero m_applicant;

	private Timer m_lifetimeTimer;

	private bool m_bReleased;

	public static readonly SFSynchronizedLongFactory noFactory = new SFSynchronizedLongFactory();

	public long no => m_lnNo;

	public Party party => m_party;

	public Hero applicant => m_applicant;

	public PartyApplication(Party party, Hero applicant, DateTimeOffset time)
	{
		m_lnNo = noFactory.NewValue();
		m_party = party;
		m_applicant = applicant;
		m_lifetimeTimer = new Timer(OnLifetimeTimerTick);
		m_lifetimeTimer.Change(Resource.instance.partyApplicationLifetime * 1000, -1);
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
		lock (m_applicant.syncObject)
		{
			m_applicant.OnPartyApplicationLifetimeEnded(this);
			m_party.OnApplicationLifetimeEnded(this);
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

	public PDPartyApplication ToPDPartyApplication()
	{
		PDPartyApplication inst = new PDPartyApplication();
		inst.no = m_lnNo;
		inst.partyId = (Guid)m_party.id;
		inst.applicantId = (Guid)m_applicant.id;
		inst.applicantName = m_applicant.name;
		return inst;
	}
}
