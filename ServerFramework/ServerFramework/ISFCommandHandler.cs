using Photon.SocketServer;

namespace ServerFramework;

public interface ISFCommandHandler : ISFHandler, ISFRunnable
{
	void Init(ISFPeer sender, short snCommandName, long lnPacketId, byte[] rawBody, SendParameters sendParameters);
}
