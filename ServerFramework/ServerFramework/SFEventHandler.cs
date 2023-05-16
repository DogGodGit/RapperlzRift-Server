using System;
using Photon.SocketServer;

namespace ServerFramework;

public abstract class SFEventHandler : SFHandler, ISFEventHandler, ISFHandler, ISFRunnable
{
	public void Init(ISFPeer sender, short snEventName, byte[] rawBody, SendParameters sendParameters)
	{
		InitHandler(sender, snEventName, rawBody, sendParameters);
	}

	protected override void Handle()
	{
		try
		{
			HandleEvent();
		}
		catch (SFEventHandleException ex)
		{
			if (ex.loggingEnabled)
			{
				throw new Exception(ex.Message, ex);
			}
		}
	}

	protected abstract void HandleEvent();
}
