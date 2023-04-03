using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class InfiniteWarBuffBoxInstance
{
	private long m_lnInstanceId;

	private InfiniteWarBuffBox m_buffBox;

	private InfiniteWarInstance m_infiniteWarInst;

	private Vector3 m_position = Vector3.zero;

	private Timer m_lifetimeTimer;

	protected bool m_bIsReleased;

	private static object s_instanceSyncObject = new object();

	private static long s_lnInstanceId = 0L;

	public long instanceId => m_lnInstanceId;

	public InfiniteWarBuffBox buffBox => m_buffBox;

	public InfiniteWarInstance infiniteWarInst => m_infiniteWarInst;

	public Vector3 position => m_position;

	public InfiniteWarBuffBoxInstance()
	{
		m_lnInstanceId = NewInstanceId();
	}

	public void Init(InfiniteWarInstance infiniteWarInst, InfiniteWarBuffBox buffBox, Vector3 position)
	{
		if (infiniteWarInst == null)
		{
			throw new ArgumentNullException("infiniteWarinst");
		}
		if (buffBox == null)
		{
			throw new ArgumentNullException("buffBox");
		}
		m_infiniteWarInst = infiniteWarInst;
		m_buffBox = buffBox;
		m_position = position;
		int nDuration = m_infiniteWarInst.infiniteWar.buffBoxLifetime * 1000;
		m_lifetimeTimer = new Timer(OnLifeTimeTimerTick);
		m_lifetimeTimer.Change(nDuration, -1);
	}

	private void OnLifeTimeTimerTick(object state)
	{
		m_infiniteWarInst.AddWork(new SFAction(ExpireLifetime), bGlobalLockRequired: false);
	}

	private void ExpireLifetime()
	{
		if (!m_bIsReleased)
		{
			m_infiniteWarInst.OnExpireBuffBoxLifetime(this);
		}
	}

	public bool IsBuffBoxAcquisitionRange(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_infiniteWarInst.infiniteWar.buffBoxAcquisitionRange * 1.1f + fRadius * 2f, position);
	}

	public void Release()
	{
		m_bIsReleased = true;
		DisposeLifetimeTimer();
	}

	private void DisposeLifetimeTimer()
	{
		if (m_lifetimeTimer != null)
		{
			m_lifetimeTimer.Dispose();
			m_lifetimeTimer = null;
		}
	}

	public PDInfiniteWarBuffBoxInstance ToPDInfiniteWarBuffBoxInstance()
	{
		PDInfiniteWarBuffBoxInstance inst = new PDInfiniteWarBuffBoxInstance();
		inst.instanceId = m_lnInstanceId;
		inst.id = m_buffBox.id;
		inst.position = m_position;
		return inst;
	}

	public static long NewInstanceId()
	{
		lock (s_instanceSyncObject)
		{
			return ++s_lnInstanceId;
		}
	}
}
