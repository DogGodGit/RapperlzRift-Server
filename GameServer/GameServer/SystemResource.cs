using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class SystemResource
{
	private Dictionary<int, GameServer> m_gameServers = new Dictionary<int, GameServer>();

	private Dictionary<int, VirtualGameServer> m_virtualGameServers = new Dictionary<int, VirtualGameServer>();

	private static SystemResource s_instance = new SystemResource();

	public static SystemResource instance => s_instance;

	public void Init()
	{
		SFLogUtil.Info(GetType(), "SystemResource.Init() started.");
		LoadUserDBResources();
		SFLogUtil.Info(GetType(), "SystemResource.Init() finished.");
	}

	private void LoadUserDBResources()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenUserDBConnection();
			foreach (DataRow dr2 in UserDac.GameServers(conn, null))
			{
				GameServer gameServer2 = new GameServer();
				gameServer2.Set(dr2);
				AddGameServer(gameServer2);
			}
			foreach (DataRow dr in UserDac.VirtualGameServers(conn, null))
			{
				int nGameServerId = Convert.ToInt32(dr["serverId"]);
				GameServer gameServer = GetGameServer(nGameServerId);
				if (gameServer == null)
				{
					SFLogUtil.Warn(GetType(), "[가상게임서버 목록] 게임서버가 존재하지 않습니다. nGameServerId = " + nGameServerId);
					continue;
				}
				VirtualGameServer virtualGameServer = new VirtualGameServer(gameServer);
				virtualGameServer.Set(dr);
				AddVirtualGameServer(virtualGameServer);
				gameServer.AddVirtualGameServer(virtualGameServer);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void AddGameServer(GameServer server)
	{
		m_gameServers.Add(server.id, server);
	}

	public GameServer GetGameServer(int nId)
	{
		if (!m_gameServers.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddVirtualGameServer(VirtualGameServer server)
	{
		m_virtualGameServers.Add(server.id, server);
	}

	public VirtualGameServer GetVirtualGameServer(int nId)
	{
		if (!m_virtualGameServers.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
