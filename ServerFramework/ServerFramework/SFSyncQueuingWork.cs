using System;
using System.Threading;

namespace ServerFramework;

public class SFSyncQueuingWork : SFQueuingWork
{
	private ManualResetEvent m_startWaitHandle = new ManualResetEvent(initialState: false);

	private ManualResetEvent m_endWaitHandle = new ManualResetEvent(initialState: false);

	public ManualResetEvent startWaitHandle => m_startWaitHandle;

	public ManualResetEvent endWaitHandle => m_endWaitHandle;

	public SFSyncQueuingWork(int nTargetType, object targetId)
		: base(nTargetType, targetId)
	{
	}

	protected override void RunWork()
	{
		m_startWaitHandle.Set();
		m_endWaitHandle.WaitOne();
	}

	public override void Schedule()
	{
		throw new NotSupportedException();
	}
}
