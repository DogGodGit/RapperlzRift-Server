using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildCall
{
	private long m_lnId;

	private Guild m_guild;

	private Guid m_callerId = Guid.Empty;

	private string m_sCallerName;

	private int m_nCallerMemberGrade;

	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private Timer m_lifetimeTimer;

	private bool m_bReleased;

	private static object s_noSyncObject = new object();

	private static long s_lnNo = 0L;

	public long id => m_lnId;

	public Guild guild => m_guild;

	public Guid callerId => m_callerId;

	public string callerName => m_sCallerName;

	public int callerMemberGrade => m_nCallerMemberGrade;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public Timer lifeTimeTimer => m_lifetimeTimer;

	public GuildCall(Hero caller, Continent continent, int nNationId, Vector3 position, float fRotationY)
	{
		if (caller == null)
		{
			throw new ArgumentNullException("caller");
		}
		m_lnId = NewNo();
		m_guild = caller.guildMember.guild;
		m_callerId = caller.id;
		m_sCallerName = caller.name;
		m_nCallerMemberGrade = caller.guildMember.grade.id;
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_lifetimeTimer = new Timer(OnLifetimeTimerTick);
		m_lifetimeTimer.Change(Resource.instance.guildCallLifetime * 1000, -1);
	}

	private void OnLifetimeTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessLifetimeTimerTick));
	}

	private void ProcessLifetimeTimerTick()
	{
		if (!m_bReleased)
		{
			m_guild.OnGuildCallLifetimeEnded(m_lnId);
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

	public Vector3 SelectPosition()
	{
		float fRadius = Resource.instance.guildCallRadius;
		return Util.SelectPoint(m_position, fRadius);
	}

	public PDGuildCall ToPDGuildCall()
	{
		PDGuildCall inst = new PDGuildCall();
		inst.id = m_lnId;
		inst.callerId = (Guid)m_callerId;
		inst.callerName = m_sCallerName;
		inst.callerMemberGrade = m_nCallerMemberGrade;
		return inst;
	}

	public static long NewNo()
	{
		lock (s_noSyncObject)
		{
			return ++s_lnNo;
		}
	}
}
