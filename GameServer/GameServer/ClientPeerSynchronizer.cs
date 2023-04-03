using System;
using ServerFramework;

namespace GameServer;

public class ClientPeerSynchronizer : Synchronizer
{
	private ClientPeer m_peer;

	private Account m_account;

	protected override object syncObject => m_peer.syncObject;

	protected override Hero hero
	{
		get
		{
			if (m_account == null)
			{
				return null;
			}
			return m_account.currentHero;
		}
	}

	public ClientPeerSynchronizer(ClientPeer peer, ISFRunnable runnable)
		: base(runnable)
	{
		if (peer == null)
		{
			throw new ArgumentNullException("peer");
		}
		m_peer = peer;
	}

	protected override void Init()
	{
		m_account = m_peer.account;
		base.Init();
	}

	protected override bool IsValid()
	{
		if (m_account == m_peer.account)
		{
			return base.IsValid();
		}
		return false;
	}

	public static void Exec(ClientPeer peer, ISFRunnable runnable)
	{
		ClientPeerSynchronizer inst = new ClientPeerSynchronizer(peer, runnable);
		inst.Run();
	}
}
