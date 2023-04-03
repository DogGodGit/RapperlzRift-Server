using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationCall
{
	private long m_lnId;

	private NationInstance m_nationInst;

	private Guid m_callerId = Guid.Empty;

	private string m_sCallerName;

	private int m_nCallerNoblesseId;

	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private Timer m_lifetimeTimer;

	private bool m_bReleased;

	public static readonly SFSynchronizedLongFactory noFactory = new SFSynchronizedLongFactory();

	public long id => m_lnId;

	public NationInstance nationInst => m_nationInst;

	public Guid callerId => m_callerId;

	public string callerName => m_sCallerName;

	public int callerNoblesseId => m_nCallerNoblesseId;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public Timer lifeTimeTimer => m_lifetimeTimer;

	public NationCall(Hero caller, Continent continent, int nNationId, Vector3 position, float fRotationY)
	{
		if (caller == null)
		{
			throw new ArgumentNullException("caller");
		}
		if (continent == null)
		{
			throw new ArgumentNullException("continent");
		}
		m_lnId = noFactory.NewValue();
		m_nationInst = caller.nationInst;
		m_callerId = caller.id;
		m_sCallerName = caller.name;
		m_nCallerNoblesseId = caller.nationNoblesseId;
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_lifetimeTimer = new Timer(OnLifetimeTimerTick);
		m_lifetimeTimer.Change(Resource.instance.nationCallLifeTime * 1000, -1);
	}

	private void OnLifetimeTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessLifetimeTimerTick));
	}

	private void ProcessLifetimeTimerTick()
	{
		_ = m_bReleased;
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

	public PDNationCall ToPDNationCall()
	{
		PDNationCall inst = new PDNationCall();
		inst.id = m_lnId;
		inst.callerId = (Guid)m_callerId;
		inst.callerName = m_sCallerName;
		inst.callerNoblesseId = m_nCallerNoblesseId;
		return inst;
	}
}
