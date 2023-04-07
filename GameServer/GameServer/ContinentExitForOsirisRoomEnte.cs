using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class ContinentExitForOsirisRoomEnterCommandHandler : InGameCommandHandler<ContinentExitForOsirisRoomEnterCommandBody, ContinentExitForOsirisRoomEnterResponseBody>
{
	public const short kResult_LevelUnderflowed = 101;

	public const short kResult_Dead = 102;

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
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nDifficulty = m_body.difficulty;
		OsirisRoom osirisRoom = Resource.instance.osirisRoom;
		OsirisRoomDifficulty difficulty = osirisRoom.GetDifficulty(nDifficulty);
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "해당 난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
		}
		if (m_myHero.nationId != currentPlace.nationId)
		{
			throw new CommandHandleException(1, "자신의 국가에서만 입장할 수 있습니다.");
		}
		if (difficulty.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = difficulty.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(101, "영웅의 레벨이 낮아 해당 던전난이도에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(difficulty.requiredMainQuestNo))
		{
			throw new CommandHandleException(106, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
		}
		if (m_myHero.stamina < osirisRoom.requiredStamina)
		{
			throw new CommandHandleException(104, "스태미너가 부족합니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetOsirisRoomAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(105, "입장횟수가 초과되었습니다.");
		}
		currentPlace.Exit(m_myHero, isLogOut: false, new OsirisRoomEnterParam(difficulty, currentTime));
		SendResponseOK(null);
	}
}
