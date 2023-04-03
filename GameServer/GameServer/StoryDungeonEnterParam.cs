using System;

namespace GameServer;

public class StoryDungeonEnterParam : PlaceEntranceParam
{
	private StoryDungeonDifficulty m_difficulty;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public StoryDungeonDifficulty difficulty => m_difficulty;

	public DateTimeOffset enterTime => m_enterTime;

	public StoryDungeonEnterParam(StoryDungeonDifficulty difficulty, DateTimeOffset enterTime)
	{
		m_difficulty = difficulty;
		m_enterTime = enterTime;
	}
}
