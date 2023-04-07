using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class AnkouTombMatchingStartCommandHandler : InGameCommandHandler<AnkouTombMatchingStartCommandBody, AnkouTombMatchingStartResponseBody>
{
	public const short kResult_NotPartyMember = 101;

	public const short kResult_NotPartyMaster = 102;

	public const short kResult_NotEnterableTime = 103;

	public const short kResult_Matching = 104;

	public const short kResult_NotEnoughLevel = 105;

	public const short kResult_LevelUnderflowed = 106;

	public const short kResult_LevelOverflowed = 107;

	public const short kResult_NotClearedMainQuest = 108;

	public const short kResult_AlreadyRindingCart = 109;

	public const short kResult_NotEnoughStamina = 110;

	public const short kResult_EnterCountOverflowed = 111;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		bool bIsPartyEntrance = m_body.isPartyEntrance;
		int nDifficulty = m_body.difficulty;
		AnkouTomb ankouTomb = Resource.instance.ankouTomb;
		AnkouTombDifficulty difficulty = ankouTomb.GetDifficulty(nDifficulty);
		if (difficulty == null)
		{
			throw new CommandHandleException(1, "목표 난이도가 유효하지 않습니다. nDifficulty = " + nDifficulty);
		}
		Party party = null;
		if (bIsPartyEntrance)
		{
			PartyMember partyMember = m_myHero.partyMember;
			if (partyMember == null)
			{
				throw new CommandHandleException(101, "파티에 가입된 상태가 아닙니다.");
			}
			if (m_myHero.id != partyMember.party.master.id)
			{
				throw new CommandHandleException(102, "파티장이 아닐경우 파티입장을 사용할 수 없습니다.");
			}
			party = partyMember.party;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		AnkouTombSchedule schedule = ankouTomb.GetEnterableSchedule(currentTime);
		if (schedule == null)
		{
			throw new CommandHandleException(103, "입장 가능한 시간이 아닙니다.");
		}
		if (m_myHero.isMatching)
		{
			throw new CommandHandleException(104, "현재 매칭중입니다.");
		}
		int nHeroLevel = m_myHero.level;
		if (ankouTomb.requiredConditionType == 1)
		{
			int nRequiredHeroLevel = ankouTomb.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(105, "영웅의 레벨이 낮아 해당 던전에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(ankouTomb.requiredMainQuestNo))
		{
			throw new CommandHandleException(108, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		int nDifficultyMinHeroLevel = difficulty.minHeroLevel;
		if (nHeroLevel < nDifficultyMinHeroLevel)
		{
			throw new CommandHandleException(106, "영웅의 레벨이 낮아 해당 난이도에 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nDifficultyMinHeroLevel = " + nDifficultyMinHeroLevel);
		}
		int nDifficultyMaxHeroLevel = difficulty.maxHeroLevel;
		if (nHeroLevel > nDifficultyMaxHeroLevel)
		{
			throw new CommandHandleException(107, "영웅의 레벨이 높아 해당 난이도가 입장할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nDifficultyMaxHeroLevel = " + nDifficultyMaxHeroLevel);
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(109, "영웅이 카트 탑승중입니다.");
		}
		if (m_myHero.stamina < ankouTomb.requiredStamina)
		{
			throw new CommandHandleException(110, "스태미너가 부족합니다.");
		}
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetAnkouTombAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(111, "입장횟수가 초과되었습니다.");
		}
		if (bIsPartyEntrance)
		{
			Cache.instance.ankouTombMatchingManager.EnterRoom_Party(party, currentTime, new AnkouTombMatchingRoomEnterParam(difficulty, schedule));
		}
		else
		{
			Cache.instance.ankouTombMatchingManager.EnterRoom_Single(m_myHero, new AnkouTombMatchingRoomEnterParam(difficulty, schedule));
		}
		MatchingRoom matchingRoom = m_myHero.matchingRoom;
		AnkouTombMatchingStartResponseBody resBody = new AnkouTombMatchingStartResponseBody();
		resBody.matchingStatus = (int)matchingRoom.status;
		resBody.remainingTime = matchingRoom.GetRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
