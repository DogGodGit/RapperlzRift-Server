using System;

namespace GameServer;

public class ExpDungeonEnterParam : PlaceEntranceParam
{
	private ExpDungeonDifficulty m_difficulty;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public ExpDungeonDifficulty difficulty => m_difficulty;

	public DateTimeOffset enterTime => m_enterTime;

	public ExpDungeonEnterParam(ExpDungeonDifficulty difficulty, DateTimeOffset enterTime)
	{
		m_difficulty = difficulty;
		m_enterTime = enterTime;
	}
}
