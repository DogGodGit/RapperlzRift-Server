using System;
using ClientCommon;

namespace GameServer;

public class DragonNestMatchingStartCommandHandler : InGameCommandHandler<DragonNestMatchingStartCommandBody, DragonNestMatchingStartResponseBody>
{
	public const short kResult_NotPartyMember = 101;

	public const short kResult_NotPartyMaster = 102;

	public const short kResult_Matching = 103;

	public const short kResult_LevelUnderflowed = 104;

	public const short kResult_AlreadyRindingCart = 105;

	public const short kResult_NotEnoughItem = 106;

	public const short kResult_NotClearedMainQuest = 107;

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
		DragonNest dragonNest = Resource.instance.dragonNest;
		if (dragonNest.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = dragonNest.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(104, "영웅의 레벨이 낮아 해당 던전에 입장할수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(dragonNest.requiredMainQuestNo))
		{
			throw new CommandHandleException(107, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(105, "영웅이 카트 탑승중입니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		_ = currentTime.Date;
		if (m_myHero.GetItemCount(dragonNest.enterRequiredItemId) <= 0)
		{
			throw new CommandHandleException(106, "입장에 필요한 아이템이 부족합니다.");
		}
		if (bIsPartyEntrance)
		{
			Cache.instance.dragonNestMatchingManager.EnterRoom_Party(party, currentTime, new DragonNestMatchingRoomEnterParam());
		}
		else
		{
			Cache.instance.dragonNestMatchingManager.EnterRoom_Single(m_myHero, new DragonNestMatchingRoomEnterParam());
		}
		MatchingRoom matchingRoom = m_myHero.matchingRoom;
		DragonNestMatchingStartResponseBody resBody = new DragonNestMatchingStartResponseBody();
		resBody.matchingStatus = (int)matchingRoom.status;
		resBody.remainingTime = matchingRoom.GetRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
