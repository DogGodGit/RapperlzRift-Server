using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class FearAltarMatchingStartCommandHandler : InGameCommandHandler<FearAltarMatchingStartCommandBody, FearAltarMatchingStartResponseBody>
{
	public const short kResult_NotPartyMember = 101;

	public const short kResult_NotPartyMaster = 102;

	public const short kResult_Matching = 103;

	public const short kResult_LevelUnderflowed = 104;

	public const short kResult_AlreadyRindingCart = 105;

	public const short kResult_NotEnoughStamina = 106;

	public const short kResult_EnterCountOverflowed = 107;

	public const short kResult_NotClearedMainQuest = 108;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "형재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		bool bIsPartyEntrance = m_body.isPartyEntrance;
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
		if (m_myHero.isMatching)
		{
			throw new CommandHandleException(103, "현재 매칭중입니다.");
		}
		FearAltar fearAltar = Resource.instance.fearAltar;
		if (fearAltar.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = fearAltar.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(104, "영웅의 레벨이 낮아 해당 던전에 입장할수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(fearAltar.requiredMainQuestNo))
		{
			throw new CommandHandleException(108, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(105, "영웅이 카트 탑승중입니다.");
		}
		if (m_myHero.stamina < fearAltar.requiredStamina)
		{
			throw new CommandHandleException(106, "스태미너가 부족합니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetFearAltarAvailableEnterCount(currentDate) <= 0)
		{
			throw new CommandHandleException(107, "입장횟수가 초과되었습니다.");
		}
		if (bIsPartyEntrance)
		{
			Cache.instance.fearAltarMatchingManager.EnterRoom_Party(party, currentTime, new FearAltarMatchingRoomEnterParam());
		}
		else
		{
			Cache.instance.fearAltarMatchingManager.EnterRoom_Single(m_myHero, new FearAltarMatchingRoomEnterParam());
		}
		MatchingRoom matchingRoom = m_myHero.matchingRoom;
		FearAltarMatchingStartResponseBody resBody = new FearAltarMatchingStartResponseBody();
		resBody.matchingStatus = (int)matchingRoom.status;
		resBody.remainingTime = matchingRoom.GetRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
