using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerFramework;

public class SFStandaloneWorker
{
    private HashSet<ISFRunnable> m_works = new HashSet<ISFRunnable>();

    private ManualResetEvent m_emptySignalHandle = new ManualResetEvent(initialState: true);

    private bool m_bRunning;

    private bool m_bIsAsyncErrorLogging = true;

    public bool isAsyncErrorLogging
    {
        get
        {
            return m_bIsAsyncErrorLogging;
        }
        set
        {
            m_bIsAsyncErrorLogging = value;
        }
    }

    public void AddWork(ISFRunnable work)
    {
        if (work == null)
        {
            throw new ArgumentNullException("work");
        }
        lock (this)
        {
            if (m_bRunning && m_works.Add(work))
            {
                ThreadPool.QueueUserWorkItem(Run, work);
                if (m_works.Count == 1)
                {
                    m_emptySignalHandle.Reset();
                }
            }
        }
    }

    private void Run(object state)
    {
        ISFRunnable iSFRunnable = (ISFRunnable)state;
        try
        {
            iSFRunnable.Run();
        }
        catch (Exception ex)
        {
            SFLogUtil.Error(GetType(), null, ex, bStackTrace: true, m_bIsAsyncErrorLogging);
        }
        lock (this)
        {
            m_works.Remove(iSFRunnable);
            if (m_works.Count == 0)
            {
                m_emptySignalHandle.Set();
            }
        }
    }

    public void Start()
    {
        lock (this)
        {
            if (!m_bRunning)
            {
                m_bRunning = true;
            }
        }
    }

    public void Stop()
    {
        lock (this)
        {
            if (m_bRunning)
            {
                m_bRunning = false;
            }
        }
    }

    public void StopAndWaitFinish()
    {
        Stop();
        WaitFinish();
    }

    public void WaitFinish()
    {
        m_emptySignalHandle.WaitOne();
    }
}