using System;
using Photon.SocketServer;

namespace ServerFramework;

public abstract class SFCommandHandler : SFHandler, ISFCommandHandler, ISFHandler, ISFRunnable
{
	protected const short kResult_OK = 0;

	protected const short kResult_Error = 1;

	protected long m_lnPacketId;

	protected bool m_bResponseSended;

	public long packetId => m_lnPacketId;

	public void Init(ISFPeer sender, short snCommandName, long lnPacketId, byte[] rawBody, SendParameters sendParameters)
	{
		m_lnPacketId = lnPacketId;
		InitHandler(sender, snCommandName, rawBody, sendParameters);
	}

	protected override void Handle()
	{
		try
		{
			PreHandleCommand();
			HandleCommand();
		}
		catch (SFCommandHandleException ex)
		{
			SendResponse(ex.result, ex.Message, null);
			if (ex.loggingEnabled)
			{
				LogError(null, ex);
			}
		}
		catch (Exception ex2)
		{
			SendResponse(1, ex2.Message, null);
			LogError(null, ex2);
		}
	}

	protected virtual void PreHandleCommand()
	{
	}

	protected abstract void HandleCommand();

	public void SendResponse(short snResult, string sErrorMessage, byte[] body, SendParameters sendParameters)
	{
		if (!m_bResponseSended)
		{
			SendResult sendResult = m_sender.SendResponse(snResult, sErrorMessage, m_snPacketName, m_lnPacketId, body, sendParameters);
			switch (sendResult)
			{
			default:
				SFLogUtil.Error(GetType(), "SendOperationResponse 실패. SendResult = " + sendResult, null, bStackTrace: true);
				break;
			case SendResult.Disconnected:
				break;
			case SendResult.Ok:
				m_bResponseSended = true;
				break;
			}
		}
	}

	public void SendResponse(short snResult, string sErrorMessage, byte[] body)
	{
		SendResponse(snResult, sErrorMessage, body, default(SendParameters));
	}
}
