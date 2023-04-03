using ClientCommon;

namespace GameServer;

public class MysteryBoxPickStartCommandHandler : InGameCommandHandler<MysteryBoxPickStartCommandBody, MysteryBoxPickStartResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyRidingMount = 103;

	public const short kResult_OtherActionPerforming = 104;

	private HeroMysteryBoxQuest m_heroQuest;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(103, "영웅이 탈것을 타고있는 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.moving)
		{
			throw new CommandHandleException(1, "영웅이 이동중입니다.");
		}
		if (m_myHero.autoHunting)
		{
			throw new CommandHandleException(1, "영웅이 자동사냥중입니다.");
		}
		HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
		if (currentExclusiveAction != 0)
		{
			throw new CommandHandleException(104, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		m_heroQuest = m_myHero.mysteryBoxQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		if (m_heroQuest.pickedBoxGrade == 5)
		{
			throw new CommandHandleException(1, "이미 최대등급의 의문의상자를 획득했습니다.");
		}
		MysteryBoxQuest quest = Resource.instance.mysteryBoxQuest;
		Npc targetNpc = quest.targetNpc;
		if (currentPlace.continent.id != targetNpc.continent.id)
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (currentPlace.nationId == m_myHero.nationId)
		{
			throw new CommandHandleException(1, "내 국가에서는 불가능합니다.");
		}
		if (!targetNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_heroQuest.StartPick();
		SendResponseOK(null);
	}
}
