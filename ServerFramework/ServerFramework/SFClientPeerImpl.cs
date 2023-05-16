using System.Collections.Generic;
using Photon.SocketServer;

namespace ServerFramework;

public class SFClientPeerImpl : SFPeerImpl
{
	public SFClientPeerImpl(ISFClientPeer peer)
		: base(peer)
	{
	}

	public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
	{
		switch (operationRequest.OperationCode)
		{
		case 0:
			HandleCommand((Dictionary<byte, object>)(object)operationRequest.Parameters, sendParameters);
			return;
		case 1:
			HandleEvent((Dictionary<byte, object>)(object)operationRequest.Parameters, sendParameters);
			return;
		}
		string text = $"operationRequest.OperationCode [{operationRequest.OperationCode}]는 유효하지 않습니다.";
		m_peer.SendResponse(1, text, 0, 0L, null, sendParameters);
		SFLogUtil.Error(GetType(), text);
	}
}
