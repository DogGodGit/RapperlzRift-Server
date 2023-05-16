namespace ServerFramework;

public class SFRunnableStandaloneWork : SFStandaloneWork
{
	private ISFRunnable m_runnable;

	public ISFRunnable runnable
	{
		get
		{
			return m_runnable;
		}
		set
		{
			m_runnable = value;
		}
	}

	public SFRunnableStandaloneWork()
		: this(null)
	{
	}

	public SFRunnableStandaloneWork(ISFRunnable runnable)
	{
		m_runnable = runnable;
	}

	protected override void RunWork()
	{
		if (m_runnable != null)
		{
			m_runnable.Run();
		}
	}
}
