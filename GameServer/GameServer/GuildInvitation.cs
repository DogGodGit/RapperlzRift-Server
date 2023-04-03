using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildInvitation
{
	private Guid m_id = Guid.Empty;

	private Guild m_guild;

	private Hero m_target;

	private Guid m_inviterId = Guid.Empty;

	private string m_sInviterName;

	private Timer m_lifetimeTimer;

	private bool m_bReleased;

	public Guid id => m_id;

	public Guild guild => m_guild;

	public Hero target => m_target;

	public Guid inviterId => m_inviterId;

	public string inviterName => m_sInviterName;

	public Timer lifeTimeTimer => m_lifetimeTimer;

	public GuildInvitation(Guild guild, Hero target, Guid inviterId, string sInviterName, DateTimeOffset time)
	{
		m_id = Guid.NewGuid();
		m_guild = guild;
		m_target = target;
		m_inviterId = inviterId;
		m_sInviterName = sInviterName;
		m_lifetimeTimer = new Timer(OnLifetimeTimerTick);
		m_lifetimeTimer.Change(Resource.instance.guildInvitationLifetime * 1000, -1);
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
			m_target.OnGuildInvitationLifetimeEnded(this);
			m_guild.OnInvitationLifetimeEnded(this);
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

	public PDHeroGuildInvitation ToPDHeroGuildInvitation()
	{
		PDHeroGuildInvitation inst = new PDHeroGuildInvitation();
		inst.id = (Guid)m_id;
		inst.guildId = (Guid)m_guild.id;
		inst.guildName = m_guild.name;
		inst.inviterId = (Guid)m_inviterId;
		inst.inviterName = m_sInviterName;
		return inst;
	}
}
