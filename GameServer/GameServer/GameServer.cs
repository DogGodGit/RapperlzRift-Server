using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class GameServer
{
	private int m_nId;

	private string m_sGameDBConnectionString;

	private string m_sGameLogDBConnectionString;

	private Dictionary<int, VirtualGameServer> m_virtualGameServers = new Dictionary<int, VirtualGameServer>();

	public int id => m_nId;

	public string gameDBConnectionString => m_sGameDBConnectionString;

	public string gameLogDBConnectionString => m_sGameLogDBConnectionString;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["serverId"]);
		m_sGameDBConnectionString = Convert.ToString(dr["gameDBConnection"]);
		m_sGameLogDBConnectionString = Convert.ToString(dr["gameLogDBConnection"]);
	}

	public void AddVirtualGameServer(VirtualGameServer server)
	{
		if (server == null)
		{
			throw new ArgumentNullException("server");
		}
		m_virtualGameServers.Add(server.id, server);
	}

	public VirtualGameServer GetVirtualGameServer(int nVirtualGameServerId)
	{
		if (!m_virtualGameServers.TryGetValue(nVirtualGameServerId, out var value))
		{
			return null;
		}
		return value;
	}
}
