using System;

namespace ServerFramework;

public abstract class SFWork : ISFRunnable
{
	protected Action<SFWork, Exception> m_finishHandler;

	private object m_state;

	public Action<SFWork, Exception> finishHandler
	{
		get
		{
			return m_finishHandler;
		}
		set
		{
			m_finishHandler = value;
		}
	}

	public object state
	{
		get
		{
			return m_state;
		}
		set
		{
			m_state = value;
		}
	}

	public void Run()
	{
		Exception ex = null;
		try
		{
			RunWork();
		}
		catch (Exception ex2)
		{
			ex = ex2;
		}
		try
		{
			if (m_finishHandler != null)
			{
				m_finishHandler(this, ex);
			}
			else if (ex != null)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
		}
		catch (Exception ex3)
		{
			SFLogUtil.Error(GetType(), null, ex3, bStackTrace: true);
		}
		try
		{
			OnPostRun();
		}
		catch (Exception ex4)
		{
			SFLogUtil.Error(GetType(), null, ex4, bStackTrace: true);
		}
	}

	protected abstract void RunWork();

	protected virtual void OnPostRun()
	{
	}

	public abstract void Schedule();
}
