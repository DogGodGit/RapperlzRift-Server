using System;

namespace GameServer;

public class GoldDungeonEnterParam : PlaceEntranceParam
{
	private GoldDungeonDifficulty m_difficulty;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public GoldDungeonDifficulty difficulty => m_difficulty;

	public DateTimeOffset enterTime => m_enterTime;

	public GoldDungeonEnterParam(GoldDungeonDifficulty difficulty, DateTimeOffset enterTime)
	{
		m_difficulty = difficulty;
		m_enterTime = enterTime;
	}
}
