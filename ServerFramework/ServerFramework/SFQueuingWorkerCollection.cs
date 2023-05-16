using System;
using System.Collections.Generic;

namespace ServerFramework;

public class SFQueuingWorkerCollection<TWorkerId, TWorker> : ISFQueuingWorkerCollection where TWorker : SFQueuingWorker
{
	private Dictionary<TWorkerId, TWorker> m_workers = new Dictionary<TWorkerId, TWorker>();

	void ISFQueuingWorkerCollection.AddWork(object workerId, ISFRunnable work)
	{
		AddWork((TWorkerId)workerId, work);
	}

	private TWorker GetWorker(TWorkerId workerId)
	{
		if (!m_workers.TryGetValue(workerId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddWork(TWorkerId workerId, ISFRunnable work)
	{
		TWorker val = null;
		lock (this)
		{
			val = GetWorker(workerId);
			if (val == null)
			{
				val = Activator.CreateInstance<TWorker>();
				val.Start();
				m_workers.Add(workerId, val);
			}
		}
		val.EnqueueWork(work);
	}

	public void StopAndWaitFinish(bool bClearQueue)
	{
		lock (this)
		{
			foreach (TWorker value in m_workers.Values)
			{
				TWorker current = value;
				current.StopAndWaitFinish(bClearQueue);
			}
		}
	}
}
