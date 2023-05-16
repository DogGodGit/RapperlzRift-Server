using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerFramework;

public class SFStaticWorker : SFQueuingWorker
{
	private Thread m_thread;

	private Queue<ISFRunnable> m_works = new Queue<ISFRunnable>();

	private ManualResetEvent m_waitHandle = new ManualResetEvent(initialState: false);

	private bool m_bRunning;

	public override int workCount
	{
		get
		{
			lock (this)
			{
				return m_works.Count;
			}
		}
	}

	public SFStaticWorker()
	{
		m_thread = new Thread(Run);
	}

	public override bool EnqueueWork(ISFRunnable work)
	{
		if (work == null)
		{
			throw new ArgumentNullException("work");
		}
		lock (this)
		{
			if (!m_bRunning)
			{
				return false;
			}
			m_works.Enqueue(work);
			m_waitHandle.Set();
		}
		return true;
	}

	private ISFRunnable DequeueWork()
	{
		lock (this)
		{
			if (m_works.Count == 0)
			{
				m_waitHandle.Reset();
				return null;
			}
			return m_works.Dequeue();
		}
	}

	private void Run()
	{
		while (true)
		{
			ISFRunnable iSFRunnable = DequeueWork();
			if (iSFRunnable != null)
			{
				try
				{
					iSFRunnable.Run();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex, bStackTrace: true, m_bIsAsyncErrorLogging);
				}
			}
			else
			{
				if (!m_bRunning)
				{
					break;
				}
				m_waitHandle.WaitOne();
			}
		}
	}

	public override void Start()
	{
		lock (this)
		{
			if (!m_bRunning)
			{
				m_bRunning = true;
				m_thread.Start();
			}
		}
	}

	public override void Stop(bool bClearQueue)
	{
		lock (this)
		{
			if (m_bRunning)
			{
				m_bRunning = false;
				if (bClearQueue)
				{
					m_works.Clear();
				}
				m_waitHandle.Set();
			}
		}
	}

	public override void WaitFinish()
	{
		m_thread.Join();
	}
}
