using ServerFramework;

namespace GameServer;

public class Global
{
	private bool m_bReleased;

	private SFDynamicWorker m_worker = new SFDynamicWorker();

	private static object s_syncObject = new object();

	private static Global s_instance = new Global();

	public static object syncObject => s_syncObject;

	public static Global instance => s_instance;

	private Global()
	{
	}

	public void Init()
	{
		m_worker.isAsyncErrorLogging = true;
		m_worker.Start();
	}

	public void AddWork(ISFRunnable work)
	{
		m_worker.EnqueueWork(new SFAction<ISFRunnable>(RunWork, work));
	}

	private void RunWork(ISFRunnable work)
	{
		lock (s_syncObject)
		{
			if (!m_bReleased)
			{
				work.Run();
			}
		}
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			m_worker.Stop(bClearQueue: true);
			m_bReleased = true;
		}
	}
}
