namespace GameServer;

public class SoulCoveterMatchingRoomEnterParam : MatchingRoomEntranceParam
{
	private SoulCoveterDifficulty m_difficulty;

	public SoulCoveterDifficulty difficulty => m_difficulty;

	public SoulCoveterMatchingRoomEnterParam(SoulCoveterDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}
}
