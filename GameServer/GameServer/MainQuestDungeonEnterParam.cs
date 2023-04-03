namespace GameServer;

public class MainQuestDungeonEnterParam : PlaceEntranceParam
{
	private MainQuestDungeon m_dungeon;

	public MainQuestDungeon dungeon => m_dungeon;

	public MainQuestDungeonEnterParam(MainQuestDungeon dungeon)
	{
		m_dungeon = dungeon;
	}
}
