using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ServerFramework;

namespace GameServer;

public class ClientPeer : SFClientPeer
{
	private object m_syncObject = new object();

	private Account m_account;

	public object syncObject => m_syncObject;

	public Account account
	{
		get
		{
			return m_account;
		}
		set
		{
			m_account = value;
		}
	}

	public ClientPeer(InitRequest initRequest)
		: base(initRequest, ClientCommandHandlerFactory.instance, ClientEventHandlerFactory.instance)
	{
	}

	public override void AddWork(ISFRunnable work)
	{
		base.AddWork(work);
		WorkLogManager.instance.AddLog(work.GetType());
	}

	protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
	{
		base.OnDisconnect(reasonCode, reasonDetail);
		GameServerApp.inst.RemoveClientPeer(base.id);
		lock (Global.syncObject)
		{
			ClientPeerSynchronizer.Exec(this, new SFAction(ProcessDisconnect));
		}
	}

	private void ProcessDisconnect()
	{
		if (m_account != null)
		{
			m_account.LogOut();
		}
	}
}
