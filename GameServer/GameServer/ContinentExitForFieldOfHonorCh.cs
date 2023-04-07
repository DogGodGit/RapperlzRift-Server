using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentExitForFieldOfHonorChallengeCommandHandler : InGameCommandHandler<ContinentExitForFieldOfHonorChallengeCommandBody, ContinentExitForFieldOfHonorChallengeResponseBody>
{
	public const short kResult_InvalidRanking = 101;

	public const short kResult_CannotChallengeSelf = 102;

	public const short kResult_LevelUnderflowed = 103;

	public const short kResult_Dead = 104;

	public const short kResult_NotEnoughStamina = 106;

	public const short kResult_EnterCountOverflowed = 107;

	public const short kResult_RidingCart = 108;

	public const short kResult_NotClearedMainQuest = 109;

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
		int nTargetRanking = m_body.targetRanking;
		if (!m_myHero.ContainsFieldOfHonorTargetRanking(nTargetRanking))
		{
			throw new CommandHandleException(101, "결투장타겟 목록에 존재하지 않은 랭킹입니다. nTargetRanking = " + nTargetRanking);
		}
		FieldOfHonorHero targetFieldOfHonorHero = Cache.instance.GetFieldOfHonorHero(nTargetRanking);
		if (targetFieldOfHonorHero == null)
		{
			throw new CommandHandleException(1, "목표랭커가 존재하지 않습니다.");
		}
		if (m_myHero.id == targetFieldOfHonorHero.id)
		{
			throw new CommandHandleException(102, "자기 자신에게 도전할 수 없습니다.");
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		FieldOfHonor fieldOfHonor = Resource.instance.fieldOfHonor;
		if (fieldOfHonor.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = fieldOfHonor.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(103, "영웅의 레벨이 낮아 해당 결투장에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(fieldOfHonor.requiredMainQuestNo))
		{
			throw new CommandHandleException(109, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(104, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(108, "영웅이 카트에 탑승중 입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetFieldOfHonorAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(107, "입장횟수가 초과되었습니다.");
		}
		Hero targetRanker = targetFieldOfHonorHero.ToHero(m_myHero, currentTime);
		currentPlace.Exit(m_myHero, isLogOut: false, new FieldOfHonorChallengeParam(currentTime, targetRanker, nTargetRanking));
		SendResponseOK(null);
	}
}
