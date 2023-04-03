using ClientCommon;

namespace GameServer;

public class GuildFarmQuestInteractionStartCommandHandler : InGameCommandHandler<GuildFarmQuestInteractionStartCommandBody, GuildFarmQuestInteractionStartResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyRidingMount = 102;

	public const short kResult_AlreadyRidingCart = 103;

	public const short kResult_OtherActionPerforming = 104;

	public const short kResult_PerformingQuestNotExist = 105;

	public const short kResult_AlreadyObjectiveCompleted = 106;

	private HeroGuildFarmQuest m_heroQuest;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is GuildTerritoryInstance))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRiding)
		{
			throw new CommandHandleException(102, "영웅이 탈것을 타고있는 상태입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(103, "영웅이 카트에 탑승중입니다.");
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
		m_heroQuest = m_myHero.guildFarmQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(105, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		if (m_heroQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(106, "이미 목표가 완료되었습니다.");
		}
		GuildFarmQuest quest = Resource.instance.guildFarmQuest;
		GuildTerritoryNpc targetNpc = quest.targetNpc;
		if (!targetNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "대상NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_heroQuest.StartInteraction();
		SendResponseOK(null);
	}
}
