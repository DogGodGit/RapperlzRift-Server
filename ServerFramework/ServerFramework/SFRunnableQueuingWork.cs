namespace ServerFramework;

public class SFRunnableQueuingWork : SFMainQueuingWork
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

	public SFRunnableQueuingWork(int nTargetType, object targetId)
		: this(nTargetType, targetId, null)
	{
	}

	public SFRunnableQueuingWork(int nTargetType, object targetId, ISFRunnable runnable)
		: base(nTargetType, targetId)
	{
		m_runnable = runnable;
	}

	protected override void RunMainWork()
	{
		if (m_runnable != null)
		{
			m_runnable.Run();
		}
	}
}
