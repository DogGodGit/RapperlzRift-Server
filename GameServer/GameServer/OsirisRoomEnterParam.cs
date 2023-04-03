using System;

namespace GameServer;

public class OsirisRoomEnterParam : PlaceEntranceParam
{
	private OsirisRoomDifficulty m_difficulty;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public OsirisRoomDifficulty difficulty => m_difficulty;

	public DateTimeOffset enterTime => m_enterTime;

	public OsirisRoomEnterParam(OsirisRoomDifficulty difficulty, DateTimeOffset enterTime)
	{
		m_difficulty = difficulty;
		m_enterTime = enterTime;
	}
}
