namespace GameServer;

public class BiographyQuestDungeonEnterParam : PlaceEntranceParam
{
	private BiographyQuestDungeon m_dungeon;

	public BiographyQuestDungeon dungeon => m_dungeon;

	public BiographyQuestDungeonEnterParam(BiographyQuestDungeon dungeon)
	{
		m_dungeon = dungeon;
	}
}
