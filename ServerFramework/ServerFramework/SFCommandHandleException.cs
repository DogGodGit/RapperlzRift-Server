using System;

namespace ServerFramework;

public class SFCommandHandleException : Exception
{
	private short m_snResult;

	private bool m_bLoggingEnabled = true;

	public short result => m_snResult;

	public bool loggingEnabled => m_bLoggingEnabled;

	public SFCommandHandleException(short snResult, string sMessage)
		: this(snResult, sMessage, null)
	{
	}

	public SFCommandHandleException(short snResult, string sMessage, Exception innerException)
		: this(snResult, sMessage, innerException, bLoggingEnabled: true)
	{
	}

	public SFCommandHandleException(short snResult, string sMessage, Exception innerException, bool bLoggingEnabled)
		: base(sMessage, innerException)
	{
		m_snResult = snResult;
		m_bLoggingEnabled = bLoggingEnabled;
	}
}
