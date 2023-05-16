using System.Collections.Generic;
using Photon.SocketServer;

namespace ServerFramework;

public static class SFEvent
{
	public static SendResult Send(PeerBase peer, short snEventName, byte[] body, SendParameters sendParameters)
	{
		return peer.SendEvent(SFUtil.CreateEventData(snEventName, body), sendParameters);
	}

	public static SendResult Send(PeerBase peer, short snEventName, byte[] body)
	{
		return Send(peer, snEventName, body, default(SendParameters));
	}

	public static void Send<TPeer>(IEnumerable<TPeer> peers, short snEventName, byte[] body, SendParameters sendParameters) where TPeer : PeerBase
	{
		EventData.SendTo(SFUtil.CreateEventData(snEventName, body), (IEnumerable<TPeer>)peers, sendParameters);
	}

	public static void Send<TPeer>(IEnumerable<TPeer> peers, short snEventName, byte[] body) where TPeer : PeerBase
	{
		EventData.SendTo(SFUtil.CreateEventData(snEventName, body), (IEnumerable<TPeer>)peers, default(SendParameters));
	}
}
