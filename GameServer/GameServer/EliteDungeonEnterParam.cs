using System;

namespace GameServer;

public class EliteDungeonEnterParam : PlaceEntranceParam
{
	private EliteMonsterMaster m_master;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public EliteMonsterMaster master => m_master;

	public DateTimeOffset enterTime => m_enterTime;

	public EliteDungeonEnterParam(EliteMonsterMaster master, DateTimeOffset enterTime)
	{
		m_master = master;
		m_enterTime = enterTime;
	}
}
