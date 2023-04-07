using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentExitForMainQuestDungeonEnterCommandHandler : InGameCommandHandler<ContinentExitForMainQuestDungeonEnterCommandBody, ContinentExitForMainQuestDungeonEnterResponseBody>
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
		HeroMainQuest heroMainQuest = m_myHero.currentHeroMainQuest;
		if (heroMainQuest == null)
		{
			throw new CommandHandleException(1, "현재 진행중인 메인퀘스트가 없습니다.");
		}
		if (heroMainQuest.completed)
		{
			throw new CommandHandleException(1, "현재 메인퀘스트는 이미 완료했습니다.");
		}
		if (heroMainQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(1, "현재 메인퀘스트의 목표를 달성했습니다.");
		}
		MainQuest mainQuest = heroMainQuest.mainQuest;
		if (mainQuest.type != 5)
		{
			throw new CommandHandleException(1, "현재 진행중인 메인퀘스트가 메인퀘스트던전 타입이 아닙니다.");
		}
		MainQuestDungeon dungeon = mainQuest.targetDungeon;
		if (dungeon == null)
		{
			throw new CommandHandleException(1, "해당 던전이 존재하지 않습니다.");
		}
		Npc startNpc = mainQuest.startNpc;
		if (startNpc != null)
		{
			if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
			{
				throw new CommandHandleException(1, "시작 NPC가 있는 장소가 아닙니다.");
			}
			if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
			{
				throw new CommandHandleException(101, "입장할 수 없는 위치입니다.");
			}
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new MainQuestDungeonEnterParam(dungeon));
		SendResponseOK(null);
	}
}
