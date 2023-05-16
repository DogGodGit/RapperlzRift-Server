using Photon.SocketServer;

namespace ServerFramework;

public interface ISFEventHandler : ISFHandler, ISFRunnable
{
	void Init(ISFPeer sender, short snEventName, byte[] rawBody, SendParameters sendParameters);
}
