using System;
using ServerFramework;

namespace GameServer;

public class EventHandleException : SFEventHandleException
{
	public EventHandleException(string sMessage)
		: this(sMessage, null)
	{
	}

	public EventHandleException(string sMessage, Exception innerException)
		: this(sMessage, innerException, bLoggingEnabled: true)
	{
	}

	public EventHandleException(string sMessage, Exception innerException, bool bLoggingEnabled)
		: base(sMessage, innerException, bLoggingEnabled)
	{
	}
}
