using System;

namespace ServerFramework;

public abstract class SFQueuingWork : SFWork
{
	protected int m_nTargetType;

	protected object m_targetId;

	public int targetType => m_nTargetType;

	public object targetId => m_targetId;

	public SFQueuingWork(int nTargetType, object targetId)
	{
		if (targetId == null)
		{
			throw new ArgumentNullException("targetId");
		}
		m_nTargetType = nTargetType;
		m_targetId = targetId;
	}
}
