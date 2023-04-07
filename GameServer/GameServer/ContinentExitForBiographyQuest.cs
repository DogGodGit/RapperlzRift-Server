using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentExitForBiographyQuestDungeonEnterCommandHandler : InGameCommandHandler<ContinentExitForBiographyQuestDungeonEnterCommandBody, ContinentExitForBiographyQuestDungeonEnterResponseBody>
{
	public const short kResult_OutOfEnterRange = 101;

	public const short kResult_Dead = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nDungeonId = m_body.dungeonId;
		if (nDungeonId <= 0)
		{
			throw new CommandHandleException(1, "던전ID가 유효하지 않습니다. nDungeonId = " + nDungeonId);
		}
		BiographyQuestDungeon dungeon = Resource.instance.GetBiographyQuestDungeon(nDungeonId);
		if (dungeon == null)
		{
			throw new CommandHandleException(1, "전기퀘스트던전이 존재하지 않습니다. nDungeonId = " + nDungeonId);
		}
		BiographyQuest targetBiographyQuest = null;
		foreach (HeroBiography heroBiography in m_myHero.biographies.Values)
		{
			if (heroBiography.completed || heroBiography.quest == null)
			{
				continue;
			}
			HeroBiographyQuest heroBiographyQuest = heroBiography.quest;
			if (!heroBiographyQuest.isObjectiveCompleted)
			{
				BiographyQuest biographyQuest = heroBiographyQuest.quest;
				if (biographyQuest.type == BiographyQuestType.BiographyDungeon && biographyQuest.targetDungeonId == dungeon.id)
				{
					targetBiographyQuest = biographyQuest;
				}
			}
		}
		if (targetBiographyQuest == null)
		{
			throw new CommandHandleException(1, "진행 중인 퀘스트 던전이 아닙니다.");
		}
		Npc startNpc = targetBiographyQuest.startNpc;
		if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "해당 장소에 없는 NPC입니다.");
		}
		if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "입장할 수 없는 위치입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new BiographyQuestDungeonEnterParam(dungeon));
		SendResponseOK(null);
	}
}
