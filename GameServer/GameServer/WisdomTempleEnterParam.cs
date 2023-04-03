using System;

namespace GameServer;

public class WisdomTempleEnterParam : PlaceEntranceParam
{
	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public DateTimeOffset enterTime => m_enterTime;

	public WisdomTempleEnterParam(DateTimeOffset enterTime)
	{
		m_enterTime = enterTime;
	}
}
