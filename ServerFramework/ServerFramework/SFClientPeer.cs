using System;
using System.Runtime.CompilerServices;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace ServerFramework;

public abstract class SFClientPeer : ClientPeer, ISFClientPeer, ISFPeer
{
	protected ISFHandlerFactory<short, ISFCommandHandler> m_commandHandlerFactory;

	protected ISFHandlerFactory<short, ISFEventHandler> m_eventHandlerFactory;

	protected SFClientPeerImpl m_impl;

	public Guid id => m_impl.id;

	public ISFHandlerFactory<short, ISFCommandHandler> commandHandlerFactory => m_commandHandlerFactory;

	public ISFHandlerFactory<short, ISFEventHandler> eventHandlerFactory => m_eventHandlerFactory;

	public int workCount => m_impl.workCount;

	public SFClientPeer(InitRequest initRequest, ISFHandlerFactory<short, ISFCommandHandler> commandHandlerFactory, ISFHandlerFactory<short, ISFEventHandler> eventHandlerFactory)
		: base(initRequest)
	{
		m_commandHandlerFactory = commandHandlerFactory;
		m_eventHandlerFactory = eventHandlerFactory;
		m_impl = new SFClientPeerImpl(this);
		m_impl.StartWorker();
	}

	public virtual void AddWork(ISFRunnable work)
	{
		m_impl.AddWork(work);
	}

	public SendResult SendResponse(short snResult, string sErrorMessage, short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters)
	{
		return m_impl.SendResponse(snResult, sErrorMessage, snCommandName, lnPacketId, body, sendParameters);
	}

	public SendResult SendResponse(short snResult, string sErrorMessage, short snCommandName, long lnPacketId, byte[] body)
	{
		return SendResponse(snResult, sErrorMessage, snCommandName, lnPacketId, body, default(SendParameters));
	}

	public SendResult SendEvent(short snEventName, byte[] body, SendParameters sendParameters)
	{
		return m_impl.SendEvent(snEventName, body, sendParameters);
	}

	public SendResult SendEvent(short snEventName, byte[] body)
	{
		return SendEvent(snEventName, body, default(SendParameters));
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

	protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
	{
		m_impl.StopWorker(bClearQueue: true);
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
