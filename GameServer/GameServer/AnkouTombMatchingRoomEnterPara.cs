namespace GameServer;

public class AnkouTombMatchingRoomEnterParam : MatchingRoomEntranceParam
{
	private AnkouTombDifficulty m_difficulty;

	private AnkouTombSchedule m_schedule;

	public AnkouTombDifficulty difficulty => m_difficulty;

	public AnkouTombSchedule schedule => m_schedule;

	public AnkouTombMatchingRoomEnterParam(AnkouTombDifficulty difficulty, AnkouTombSchedule schedule)
	{
		m_difficulty = difficulty;
		m_schedule = schedule;
	}
}
