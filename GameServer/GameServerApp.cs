using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using GameServer.CommandHandlers;
using Photon.SocketServer;
using ServerFramework;

namespace GameServer;

public class GameServerApp : SFApplication
{
	public const int kWorkQueueLogTimerInterval = 1000;

	private int m_nServerId;

	private TimeSpan m_currentTimeOffset = TimeSpan.Zero;

	private Dictionary<Guid, ClientPeer> m_clientPeers = new Dictionary<Guid, ClientPeer>();

	private Timer m_workQueueLogTimer;

	public int serverId => m_nServerId;

	public TimeSpan currentTimeOffset => m_currentTimeOffset;

	public int clientPeerCount
	{
		get
		{
			lock (m_clientPeers)
			{
				return m_clientPeers.Count;
			}
		}
	}

	public static GameServerApp inst => (GameServerApp)ApplicationBase.Instance;

	protected override void Initialize()
	{
		base.Initialize();
		m_nServerId = GameServerName.Parse(base.ApplicationName).serverId;
		Global.instance.Init();
		SystemResource.instance.Init();
		InitCurrentTimeOffset();
		Resource.instance.Init();
		ClientCommandHandlerFactory.instance.Init();
		ClientEventHandlerFactory.instance.Init();
		WorkLogManager.instance.Init();
		m_workQueueLogTimer = new Timer(OnWorkQueueLogTimerTick);
		m_workQueueLogTimer.Change(1000, 1000);
		lock (Global.syncObject)
		{
			Cache.instance.Init();
		}
	}

	protected override void InitQueuingWorkManager()
	{
		foreach (int nTargetType in Enum.GetValues(typeof(QueuingWorkTargetType)))
		{
			m_queuingWorkManager.CreateWorkCollection(nTargetType);
		}
	}

	private void InitCurrentTimeOffset()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			DateTimeOffset dbCurrentTime = GameDac.CurrentTime(conn, null);
			m_currentTimeOffset = dbCurrentTime - DateTimeOffset.Now;
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	protected override PeerBase CreatePeer(InitRequest initRequest)
	{
		ClientPeer clientPeer = new ClientPeer(initRequest);
		AddClientPeer(clientPeer);
		return clientPeer;
	}

	private void AddClientPeer(ClientPeer clientPeer)
	{
		lock (m_clientPeers)
		{
			m_clientPeers.Add(clientPeer.id, clientPeer);
		}
	}

	public void RemoveClientPeer(Guid id)
	{
		lock (m_clientPeers)
		{
			m_clientPeers.Remove(id);
		}
	}

	private void StopWorkQueueLogTimer()
	{
		if (m_workQueueLogTimer != null)
		{
			m_workQueueLogTimer.Dispose();
			m_workQueueLogTimer = null;
		}
	}

	private void OnWorkQueueLogTimerTick(object state)
	{
		try
		{
			int nTotalWorkCount = 0;
			lock (m_clientPeers)
			{
				foreach (ClientPeer peer in m_clientPeers.Values)
				{
					nTotalWorkCount += peer.workCount;
				}
			}
			SFSqlStandaloneWork dbWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			dbWork.AddSqlCommand(GameLogDac.CSC_AddWorkQueueLog(Guid.NewGuid(), nTotalWorkCount, DateTimeUtil.currentTime));
			dbWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(((object)this).GetType(), null, ex);
		}
	}

	protected override void OnTearDown()
	{
		lock (Global.syncObject)
		{
			Cache.instance.OnTearDown();
			WorkLogManager.instance.Release();
			StopWorkQueueLogTimer();
			Cache.instance.Release();
			Global.instance.Release();
		}
		base.OnTearDown();
	}
}
