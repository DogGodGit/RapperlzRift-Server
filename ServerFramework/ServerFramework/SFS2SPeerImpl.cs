using System;
using System.Collections.Generic;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace ServerFramework;

public abstract class SFS2SPeerImpl : SFPeerImpl
{
	protected Dictionary<long, SFCommand> m_commands = new Dictionary<long, SFCommand>();

	private static readonly SFSynchronizedLongFactory commandPacketIdFactory = new SFSynchronizedLongFactory();

	public SFS2SPeerImpl(ISFS2SPeer peer)
		: base(peer)
	{
	}

	protected void AddCommand(long lnPacketId, SFCommand command)
	{
		lock (m_commands)
		{
			m_commands.Add(lnPacketId, command);
		}
	}

	protected void RemoveCommand(long lnPacketId)
	{
		lock (m_commands)
		{
			m_commands.Remove(lnPacketId);
		}
	}

	protected SFCommand PopCommand(long lnPacketId)
	{
		lock (m_commands)
		{
			if (m_commands.TryGetValue(lnPacketId, out var value))
			{
				m_commands.Remove(lnPacketId);
				return value;
			}
			return value;
		}
	}

	public SendResult SendCommand(short snCommandName, byte[] body, object state, SendParameters sendParameters)
	{
		long lnPacketId = commandPacketIdFactory.NewValue();
		SFCommand command = new SFCommand(snCommandName, state);
		AddCommand(lnPacketId, command);
		SendResult sendResult = ((ISFS2SPeer)m_peer).SendOperationRequest(SFUtil.CreateOperationRequest(snCommandName, lnPacketId, body), sendParameters);
		if (sendResult != 0)
		{
			RemoveCommand(lnPacketId);
		}
		return sendResult;
	}

	protected abstract void OnCommandSuccess(short snCommandName, object state, short snResponseResult, string sResponseErrorMessage, byte[] responseBody);

	protected abstract void OnCommandFail(short snCommandName, object state, SFCommandFailReason reason);

	public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
	{
		HandleCommand((Dictionary<byte, object>)(object)operationRequest.Parameters, sendParameters);
	}

	public void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
	{
		try
		{
			short returnCode = operationResponse.ReturnCode;
			string debugMessage = operationResponse.DebugMessage;
			Dictionary<byte, object> parameters = (Dictionary<byte, object>)(object)operationResponse.Parameters;
			short num = (short)parameters[0];
			long num2 = (long)parameters[1];
			byte[] responseBody = (byte[])parameters[2];
			SFCommand sFCommand = PopCommand(num2);
			if (sFCommand == null)
			{
				SFLogUtil.Warn(GetType(), "해당 패킷ID의 명령이 존재하지 않습니다. snCommandName = " + num + ", lnPacketId = " + num2);
			}
			else
			{
				OnCommandSuccess(num, sFCommand.state, returnCode, debugMessage, responseBody);
			}
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void OnEvent(IEventData eventData, SendParameters sendParameters)
	{
		HandleEvent((Dictionary<byte, object>)(object)eventData.Parameters, sendParameters);
	}

	public override void OnDisconnected(DisconnectReason reasonCode, string sReasonDetail)
	{
		base.OnDisconnected(reasonCode, sReasonDetail);
		lock (m_commands)
		{
			foreach (SFCommand value in m_commands.Values)
			{
				OnCommandFail(value.name, value.state, SFCommandFailReason.Disconnected);
			}
			m_commands.Clear();
		}
	}
}
