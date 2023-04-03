using System;
using ServerFramework;

namespace GameServer;

public class CommandHandleException : SFCommandHandleException
{
	public CommandHandleException(short snResult, string sMessage)
		: this(snResult, sMessage, null)
	{
	}

	public CommandHandleException(short snResult, string sMessage, Exception innerException)
		: this(snResult, sMessage, innerException, bLoggingEnabled: true)
	{
	}

	public CommandHandleException(short snResult, string sMessage, Exception innerException, bool bLoggingEnabled)
		: base(snResult, sMessage, innerException, bLoggingEnabled)
	{
	}
}
