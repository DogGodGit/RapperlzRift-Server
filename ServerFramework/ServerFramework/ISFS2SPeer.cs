using Photon.SocketServer;

namespace ServerFramework;

public interface ISFS2SPeer : ISFPeer
{
	SendResult SendOperationRequest(OperationRequest operationRequest, SendParameters sendParameters);

	SendResult SendCommand(short snCommandName, byte[] body, object state, SendParameters sendParameters);
}
