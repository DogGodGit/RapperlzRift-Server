using System;

namespace GameServer;

public struct GameServerName
{
	public const string kPrefix = "GameServer";

	public int serverId;

	public GameServerName(int nServerId)
	{
		serverId = nServerId;
	}

	public override string ToString()
	{
		return MakeString(serverId);
	}

	public static GameServerName Parse(string sApplicationName)
	{
		if (sApplicationName == null)
		{
			throw new ArgumentNullException("sApplicationName");
		}
		string[] parts = sApplicationName.Split('_');
		if (parts.Length != 2)
		{
			throw new ArgumentException("sApplicationName이 유효하지 않습니다.");
		}
		string sPrefix = parts[0];
		string sServerId = parts[1];
		if (sPrefix != "GameServer")
		{
			throw new ArgumentException("sApplicationName의 접두어가 유효하지 않습니다.");
		}
		return new GameServerName(int.Parse(sServerId));
	}

	public static string MakeString(int nServerId)
	{
		return string.Format("{0}_{1}", "GameServer", nServerId);
	}
}
