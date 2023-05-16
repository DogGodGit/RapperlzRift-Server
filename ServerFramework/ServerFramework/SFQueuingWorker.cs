namespace ServerFramework;

public abstract class SFQueuingWorker
{
	protected bool m_bIsAsyncErrorLogging = true;

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

	public abstract int workCount { get; }

	public abstract bool EnqueueWork(ISFRunnable work);

	public abstract void Start();

	public abstract void Stop(bool bClearQueue);

	public abstract void WaitFinish();

	public void StopAndWaitFinish(bool bClearQueue)
	{
		Stop(bClearQueue);
		WaitFinish();
	}
}
