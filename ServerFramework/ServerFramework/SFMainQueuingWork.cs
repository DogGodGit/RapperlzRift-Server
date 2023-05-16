using System;
using System.Collections.Generic;
using Photon.SocketServer;

namespace ServerFramework;

public abstract class SFMainQueuingWork : SFQueuingWork
{
	protected HashSet<SFSyncQueuingWork> m_syncWorks = new HashSet<SFSyncQueuingWork>((IEqualityComparer<SFSyncQueuingWork>)SFQueuingWorkEqualityComparer.defaultComparer);

	public HashSet<SFSyncQueuingWork> syncWorks => m_syncWorks;

	public SFMainQueuingWork(int nTargetType, object targetId)
		: base(nTargetType, targetId)
	{
	}

	public void AddSyncWork(SFSyncQueuingWork work)
	{
		if (work == null)
		{
			throw new ArgumentNullException("work");
		}
		if (m_nTargetType == work.targetType && m_targetId.Equals(work.targetId))
		{
			throw new Exception("메인작업과 같은 대상에 대한 동기작업을 추가할 수 없습니다.");
		}
		m_syncWorks.Add(work);
	}

	protected override void RunWork()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		var enumerator = m_syncWorks.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SFSyncQueuingWork current = enumerator.Current;
				current.startWaitHandle.WaitOne();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		RunMainWork();
	}

	protected abstract void RunMainWork();

	protected override void OnPostRun()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.OnPostRun();
		var enumerator = m_syncWorks.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SFSyncQueuingWork current = enumerator.Current;
				current.endWaitHandle.Set();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public override void Schedule()
	{
		((SFApplication)ApplicationBase.Instance).AddQueuingWork(this);
	}
}
