using System;
using System.Data;

namespace GameServer;

public class VirtualGameServer
{
	private int m_nId;

	private GameServer m_gameServer;

	private string m_sDisplayName;

	private int m_nDisplayNo;

	public int id => m_nId;

	public GameServer gameServer => m_gameServer;

	public string displayName => m_sDisplayName;

	public int displayNo => m_nDisplayNo;

	public VirtualGameServer(GameServer gameServer)
	{
		if (gameServer == null)
		{
			throw new ArgumentNullException("gameServer");
		}
		m_gameServer = gameServer;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["virtualGameServerId"]);
		m_sDisplayName = Convert.ToString(dr["displayName"]);
		m_nDisplayNo = Convert.ToInt32(dr["displayNo"]);
	}
}
