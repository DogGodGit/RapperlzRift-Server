using System;
using ClientCommon;

namespace GameServer;

public class InfiniteWarMatchingStartCommandHandler : InGameCommandHandler<InfiniteWarMatchingStartCommandBody, InfiniteWarMatchingStartResponseBody>
{
	public const short kResult_NotEnterableTime = 103;

	public const short kResult_Matching = 104;

	public const short kResult_LevelUnderflowed = 105;

	public const short kResult_AlreadyRindingCart = 106;

	public const short kResult_NotEnoughStamina = 107;

	public const short kResult_EnterCountOverflowed = 108;

	public const short kResult_NotClearedMainQuest = 109;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		InfiniteWar infiniteWar = Resource.instance.infiniteWar;
		InfiniteWarOpenSchedule openSchedule = infiniteWar.GetEnterableOpenSchedule(currentTime);
		if (openSchedule == null)
		{
			throw new CommandHandleException(103, "입장 가능한 시간이 아닙니다.");
		}
		if (m_myHero.isMatching)
		{
			throw new CommandHandleException(104, "현재 매칭중입니다.");
		}
		if (infiniteWar.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = infiniteWar.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(105, "영웅의 레벨이 낮아 해당 던전에 입장할수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(infiniteWar.requiredMainQuestNo))
		{
			throw new CommandHandleException(109, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(106, "영웅이 카트 탑승중입니다.");
		}
		if (m_myHero.stamina < infiniteWar.requiredStamina)
		{
			throw new CommandHandleException(107, "스태미너가 부족합니다.");
		}
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetInfiniteWarAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(108, "입장횟수가 초과되었습니다.");
		}
		Cache.instance.infiniteWarMatchingManager.EnterRoom_Single(m_myHero, new InfiniteWarMatchingRoomEnterParam(openSchedule));
		MatchingRoom matchingRoom = m_myHero.matchingRoom;
		InfiniteWarMatchingStartResponseBody resBody = new InfiniteWarMatchingStartResponseBody();
		resBody.matchingStatus = (int)matchingRoom.status;
		resBody.remainingTime = matchingRoom.GetRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
