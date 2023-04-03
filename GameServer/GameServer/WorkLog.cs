using System;

namespace GameServer;

public class WorkLog
{
	private Type m_type;

	private long m_lnRequestCount;

	public Type type => m_type;

	public long requestCount
	{
		get
		{
			return m_lnRequestCount;
		}
		set
		{
			m_lnRequestCount = value;
		}
	}

	public WorkLog(Type type)
	{
		m_type = type;
	}

	public WorkLog Clone()
	{
		WorkLog inst = new WorkLog(m_type);
		inst.requestCount = m_lnRequestCount;
		return inst;
	}
}
