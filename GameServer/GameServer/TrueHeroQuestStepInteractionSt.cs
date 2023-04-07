using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class TrueHeroQuestStepInteractionStartCommandHandler : InGameCommandHandler<TrueHeroQuestStepInteractionStartCommandBody, TrueHeroQuestStepInteractionStartResponseBody>
{
	public const short kResult_Dead = 101;

	public const short kResult_AlreadyRidingMount = 102;

	public const short kResult_OtherActionPerforming = 103;

	public const short kResult_AlreadyWaitingStep = 104;

	public const short kResult_IsAllianceNation = 105;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		NationInstance nationInst = m_myHero.nationInst;
		if (currentPlace.nationId == nationInst.nationId)
		{
			throw new CommandHandleException(1, "자기 국가에선 시작할 수 없습니다.");
		}
		if (currentPlace.nationId == nationInst.allianceNationId)
		{
			throw new CommandHandleException(105, "동맹 국가에선 시작할 수 없습니다.");
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
			throw new CommandHandleException(103, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
		}
		if (m_myHero.isTrueHeroQuestWaitingStep)
		{
			throw new CommandHandleException(104, "이미 진정한영웅 대기단계입니다.");
		}
		HeroTrueHeroQuest heroTrueHeroQuest = m_myHero.trueHeroQuest;
		if (heroTrueHeroQuest == null)
		{
			throw new CommandHandleException(1, "영웅진정한영웅퀘스트가 존재하지 않습니다.");
		}
		if (heroTrueHeroQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(1, "퀘스트의 목표가 완료되었습니다.");
		}
		TrueHeroQuestStep trueHeroQuestStep = Resource.instance.trueHeroQuest.GetStep(heroTrueHeroQuest.stepNo);
		if (trueHeroQuestStep.targetContinentId != currentPlace.continent.id)
		{
			throw new CommandHandleException(1, "목표 대륙이 아닙니다.");
		}
		if (!trueHeroQuestStep.TargetAreaContains(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "상호작용거리가 아닙니다.");
		}
		m_myHero.StartTrueHeroQuestInteraction();
		ServerEvent.SendHeroTrueHeroQuestStepInteractionStarted(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id);
		SendResponseOK(null);
	}
}
