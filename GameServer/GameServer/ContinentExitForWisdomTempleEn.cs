using System;
using ClientCommon;

namespace GameServer;

public class ContinentExitForWisdomTempleEnterCommandHandler : InGameCommandHandler<ContinentExitForWisdomTempleEnterCommandBody, ContinentExitForWisdomTempleEnterResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_Dead = 102;

	public const short kResult_AlreadyRidingCart = 103;

	public const short kResult_NotEnoughStamina = 104;

	public const short kResult_EnterCountOverflowed = 105;

	public const short kResult_NotClearedMainQuest = 106;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		WisdomTemple wisdomTemple = Resource.instance.wisdomTemple;
		if (wisdomTemple.requiredConditionType == 1)
		{
			if (m_myHero.level < wisdomTemple.requiredHeroLevel)
			{
				throw new CommandHandleException(101, "영웅레벨이 부족합니다.");
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(wisdomTemple.requiredMainQuestNo))
		{
			throw new CommandHandleException(106, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(103, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.stamina < wisdomTemple.requiredStamina)
		{
			throw new CommandHandleException(104, "스태미너가 부족합니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetWisdomTempleAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(105, "입장횟수가 초과되었습니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new WisdomTempleEnterParam(currentTime));
		SendResponseOK(null);
	}
}
