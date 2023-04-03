using System;

namespace GameServer;

public class ArtifactRoomEnterParam : PlaceEntranceParam
{
	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public DateTimeOffset enterTime => m_enterTime;

	public ArtifactRoomEnterParam(DateTimeOffset enterTime)
	{
		m_enterTime = enterTime;
	}
}
