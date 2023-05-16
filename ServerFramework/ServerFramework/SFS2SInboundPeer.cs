using System;
using System.Runtime.CompilerServices;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;

namespace ServerFramework;

public abstract class SFS2SInboundPeer : InboundS2SPeer, ISFS2SPeer, ISFPeer
{
	protected ISFHandlerFactory<short, ISFCommandHandler> m_commandHandlerFactory;

	protected ISFHandlerFactory<short, ISFEventHandler> m_eventHandlerFactory;

	protected SFS2SPeerImpl m_impl;

	public Guid id => m_impl.id;

	public ISFHandlerFactory<short, ISFCommandHandler> commandHandlerFactory => m_commandHandlerFactory;

	public ISFHandlerFactory<short, ISFEventHandler> eventHandlerFactory => m_eventHandlerFactory;

	public int workCount => m_impl.workCount;

	public SFS2SInboundPeer(InitRequest initRequest, ISFHandlerFactory<short, ISFCommandHandler> commandHandlerFactory, ISFHandlerFactory<short, ISFEventHandler> eventHandlerFactory)
		: base(initRequest)
	{
		m_commandHandlerFactory = commandHandlerFactory;
		m_eventHandlerFactory = eventHandlerFactory;
		m_impl = CreatePeerImpl();
		if (m_impl == null)
		{
			throw new Exception("피어구현이 null입니다.");
		}
		m_impl.StartWorker();
	}

	public virtual void AddWork(ISFRunnable work)
	{
		m_impl.AddWork(work);
	}

	protected abstract SFS2SPeerImpl CreatePeerImpl();

	public SendResult SendCommand(short snCommandName, byte[] body, object state, SendParameters sendParameters)
	{
		return m_impl.SendCommand(snCommandName, body, state, sendParameters);
	}

	public SendResult SendResponse(short snResult, string sErrorMessage, short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters)
	{
		return m_impl.SendResponse(snResult, sErrorMessage, snCommandName, lnPacketId, body, sendParameters);
	}

	public SendResult SendEvent(short snEventName, byte[] body, SendParameters sendParameters)
	{
		return m_impl.SendEvent(snEventName, body, sendParameters);
	}

	public virtual void OnCommand(short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters)
	{
		m_impl.OnCommand(snCommandName, lnPacketId, body, sendParameters);
	}

	public virtual void OnEvent(short snEventName, byte[] body, SendParameters sendParameters)
	{
		m_impl.OnEvent(snEventName, body, sendParameters);
	}

	protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
	{
		m_impl.OnOperationRequest(operationRequest, sendParameters);
	}

	protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
	{
		m_impl.OnOperationResponse(operationResponse, sendParameters);
	}

	protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
	{
		m_impl.OnEvent(eventData, sendParameters);
	}

	protected override void OnDisconnect(DisconnectReason reasonCode, string sReasonDetail)
	{
		m_impl.OnDisconnected(reasonCode, sReasonDetail);
	}

	SendResult ISFS2SPeer.SendOperationRequest(OperationRequest P_0, SendParameters P_1)
	{
		return SendOperationRequest(P_0, P_1);
	}

	SendResult ISFPeer.SendOperationResponse(OperationResponse P_0, SendParameters P_1)
	{
		return SendOperationResponse(P_0, P_1);
	}

	SendResult ISFPeer.SendEvent(IEventData P_0, SendParameters P_1)
	{
		return SendEvent(P_0, P_1);
	}
}
