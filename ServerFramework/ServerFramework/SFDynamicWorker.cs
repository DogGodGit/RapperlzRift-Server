using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerFramework;

public class SFDynamicWorker : SFQueuingWorker
{
	private Queue<ISFRunnable> m_works = new Queue<ISFRunnable>();

	private ManualResetEvent m_workQueueEmptySignalHandle = new ManualResetEvent(initialState: true);

	private ManualResetEvent m_noCurrentWorkSignalHandle = new ManualResetEvent(initialState: true);

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
			if (m_works.Count == 1)
			{
				m_workQueueEmptySignalHandle.Reset();
				ThreadPool.QueueUserWorkItem(Run);
			}
		}
		return true;
	}

	private ISFRunnable PeekWork()
	{
		lock (this)
		{
			return (m_works.Count > 0) ? m_works.Peek() : null;
		}
	}

	private ISFRunnable RemoveAndPeekWork()
	{
		lock (this)
		{
			if (m_works.Count > 0)
			{
				m_works.Dequeue();
			}
			if (m_works.Count == 0)
			{
				m_workQueueEmptySignalHandle.Set();
				return null;
			}
			return m_works.Peek();
		}
	}

	private void Run(object state)
	{
		for (ISFRunnable iSFRunnable = PeekWork(); iSFRunnable != null; iSFRunnable = RemoveAndPeekWork())
		{
			m_noCurrentWorkSignalHandle.Reset();
			try
			{
				iSFRunnable.Run();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true, m_bIsAsyncErrorLogging);
			}
			m_noCurrentWorkSignalHandle.Set();
		}
	}

	public override void Start()
	{
		lock (this)
		{
			if (!m_bRunning)
			{
				m_bRunning = true;
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
					m_workQueueEmptySignalHandle.Set();
				}
			}
		}
	}

	public override void WaitFinish()
	{
		m_workQueueEmptySignalHandle.WaitOne();
		m_noCurrentWorkSignalHandle.WaitOne();
	}
}
