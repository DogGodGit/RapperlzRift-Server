using System;
using System.Collections.Generic;

namespace ServerFramework;

public class SFQueuingWorkManager
{
    private Dictionary<int, SFQueuingWorkerCollection<object, SFDynamicWorker>> m_workerCollections = new Dictionary<int, SFQueuingWorkerCollection<object, SFDynamicWorker>>();

    public bool CreateWorkCollection(int nTargetType)
    {
        if (m_workerCollections.ContainsKey(nTargetType))
        {
            return false;
        }
        m_workerCollections.Add(nTargetType, new SFQueuingWorkerCollection<object, SFDynamicWorker>());
        return true;
    }

    private SFQueuingWorkerCollection<object, SFDynamicWorker> GetWorkerCollection(int nTargetType)
    {
        if (!m_workerCollections.TryGetValue(nTargetType, out var value))
        {
            return null;
        }
        return value;
    }

    public void AddWork(SFMainQueuingWork work)
    {
        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
        //IL_002c: Unknown result type (might be due to invalid IL or missing references)
        if (work == null)
        {
            throw new ArgumentNullException("work");
        }
        AddWork(work.targetType, work.targetId, work);
        var enumerator = work.syncWorks.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                SFSyncQueuingWork current = enumerator.Current;
                AddWork(current.targetType, current.targetId, current);
            }
        }
        finally
        {
            ((IDisposable)enumerator).Dispose();
        }
    }

    private void AddWork(int nTargetType, object targetId, ISFRunnable work)
    {
        SFQueuingWorkerCollection<object, SFDynamicWorker> workerCollection = GetWorkerCollection(nTargetType);
        if (workerCollection == null)
        {
            throw new Exception(Resource.Exception.SFQueuingWorkManager_01);
        }
        workerCollection.AddWork(targetId, work);
    }

    public void StopAndWaitFinish()
    {
        foreach (SFQueuingWorkerCollection<object, SFDynamicWorker> value in m_workerCollections.Values)
        {
            value.StopAndWaitFinish(bClearQueue: false);
        }
    }
}