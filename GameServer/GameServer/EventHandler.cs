using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class EventHandler<TBody> : SFEventHandler, IEventHandler, ISFEventHandler, ISFHandler, ISFRunnable where TBody : CEBClientEventBody
{
	protected TBody m_body = null;

	public ClientPeer clientPeer => (ClientPeer)m_sender;

	public ClientEventName eventName => (ClientEventName)m_snPacketName;

	protected virtual bool globalLockRequired => false;

	protected override void DeserializeBody()
	{
		m_body = Body.DeserializeRaw<TBody>(m_rawBody);
	}

	protected override void HandleEvent()
	{
		if (globalLockRequired)
		{
			lock (Global.syncObject)
			{
				InvokeHandleEventInternal();
				return;
			}
		}
		InvokeHandleEventInternal();
	}

	private void InvokeHandleEventInternal()
	{
		ClientPeerSynchronizer.Exec(clientPeer, new SFAction(HandleEventInternal));
	}

	protected abstract void HandleEventInternal();
}
