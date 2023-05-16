using System;
using Photon.SocketServer;

namespace ServerFramework;

public interface ISFPeer
{
	Guid id { get; }

	ISFHandlerFactory<short, ISFCommandHandler> commandHandlerFactory { get; }

	ISFHandlerFactory<short, ISFEventHandler> eventHandlerFactory { get; }

	int workCount { get; }

	string RemoteIP { get; }

	int RemotePort { get; }

	void AddWork(ISFRunnable work);

	SendResult SendOperationResponse(OperationResponse operationResponse, SendParameters sendParameters);

	SendResult SendEvent(IEventData eventData, SendParameters sendParameters);

	SendResult SendResponse(short snResult, string sErrorMessage, short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters);

	SendResult SendEvent(short snEventName, byte[] body, SendParameters sendParameters);

	void OnCommand(short snCommandName, long lnPacketId, byte[] body, SendParameters sendParameters);

	void OnEvent(short snEventName, byte[] body, SendParameters sendParameters);
}
