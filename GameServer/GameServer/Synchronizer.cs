using System;
using ServerFramework;

namespace GameServer;

public abstract class Synchronizer
{
	private ISFRunnable m_runnable;

	private Hero m_hero;

	private Place m_place;

	protected abstract object syncObject { get; }

	protected abstract Hero hero { get; }

	public Synchronizer(ISFRunnable runnable)
	{
		if (runnable == null)
		{
			throw new ArgumentNullException("runnable");
		}
		m_runnable = runnable;
	}

	public void Run()
	{
		do
		{
			lock (syncObject)
			{
				Init();
			}
		}
		while (!Run_Place());
	}

	protected virtual void Init()
	{
		m_hero = hero;
		if (m_hero != null)
		{
			m_place = m_hero.currentPlace;
		}
	}

	private bool Run_Place()
	{
		if (m_place == null)
		{
			return Run_SyncObject();
		}
		lock (m_place.syncObject)
		{
			return Run_SyncObject();
		}
	}

	private bool Run_SyncObject()
	{
		lock (syncObject)
		{
			return Run_Runnable();
		}
	}

	private bool Run_Runnable()
	{
		if (!IsValid())
		{
			return false;
		}
		m_runnable.Run();
		return true;
	}

	protected virtual bool IsValid()
	{
		if (m_hero != hero)
		{
			return false;
		}
		if (m_hero != null && m_place != m_hero.currentPlace)
		{
			return false;
		}
		return true;
	}
}
