using System;

namespace ServerFramework;

public class SFEventHandleException : Exception
{
	private bool m_bLoggingEnabled = true;

	public bool loggingEnabled => m_bLoggingEnabled;

	public SFEventHandleException(string sMessage)
		: this(sMessage, null)
	{
	}

	public SFEventHandleException(string sMessage, Exception innerException)
		: this(sMessage, innerException, bLoggingEnabled: true)
	{
	}

	public SFEventHandleException(string sMessage, Exception innerException, bool bLoggingEnabled)
		: base(sMessage, innerException)
	{
		m_bLoggingEnabled = bLoggingEnabled;
	}
}
