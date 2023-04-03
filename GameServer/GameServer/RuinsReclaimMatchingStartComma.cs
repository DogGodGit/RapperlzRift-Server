using System;
using ClientCommon;

namespace GameServer;

public class RuinsReclaimMatchingStartCommandHandler : InGameCommandHandler<RuinsReclaimMatchingStartCommandBody, RuinsReclaimMatchingStartResponseBody>
{
	public const short kResult_NotEnterableTime = 101;

	public const short kResult_Matching = 102;

	public const short kResult_LevelUnderflowed = 103;

	public const short kResult_NotEnoughItem = 104;

	public const short kResult_AlreadyRindingCart = 105;

	public const short kResult_NotPartyMember = 106;

	public const short kResult_NotPartyMaster = 107;

	public const short kResult_NotClearedMainQuest = 108;

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
				throw new CommandHandleException(106, "파티에 가입된 상태가 아닙니다.");
			}
			if (m_myHero.id != partyMember.party.master.id)
			{
				throw new CommandHandleException(107, "파티장이 아닐경우 파티입장을 사용할 수 없습니다.");
			}
			party = partyMember.party;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		RuinsReclaim ruinsReclaim = Resource.instance.ruinsReclaim;
		RuinsReclaimOpenSchedule openSchedule = ruinsReclaim.GetEnterableOpenSchedule(currentTime);
		if (openSchedule == null)
		{
			throw new CommandHandleException(101, "입장 가능한 시간이 아닙니다.");
		}
		if (m_myHero.isMatching)
		{
			throw new CommandHandleException(102, "현재 매칭중입니다.");
		}
		if (ruinsReclaim.requiredConditionType == 1)
		{
			int nHeroLevel = m_myHero.level;
			int nRequiredHeroLevel = ruinsReclaim.requiredHeroLevel;
			if (nHeroLevel < nRequiredHeroLevel)
			{
				throw new CommandHandleException(103, "영웅의 레벨이 낮아 해당 던전에 입장할수 없습니다. nHeroLevel = " + nHeroLevel + ", nRequiredHeroLevel = " + nRequiredHeroLevel);
			}
		}
		else if (!m_myHero.IsMainQuestCompleted(ruinsReclaim.requiredMainQuestNo))
		{
			throw new CommandHandleException(108, "입장에 필요한 메인퀘스트를 클리어하지 않았습니다.");
		}
		if (m_myHero.isRidingCart)
		{
			throw new CommandHandleException(105, "영웅이 카트 탑승중입니다.");
		}
		DateTime currentDate = currentTime.Date;
		if (m_myHero.GetRuinsReclaimAvailableFreeEnterCount(currentDate) <= 0 && m_myHero.GetItemCount(ruinsReclaim.enterRequiredItemId) <= 0)
		{
			throw new CommandHandleException(104, "입장에 필요한 아이템이 부족합니다.");
		}
		if (bIsPartyEntrance)
		{
			Cache.instance.ruinsReclaimMatchingManager.EnterRoom_Party(party, currentTime, new RuinsReclaimMatchingRoomEnterParam(openSchedule));
		}
		else
		{
			Cache.instance.ruinsReclaimMatchingManager.EnterRoom_Single(m_myHero, new RuinsReclaimMatchingRoomEnterParam(openSchedule));
		}
		MatchingRoom matchingRoom = m_myHero.matchingRoom;
		RuinsReclaimMatchingStartResponseBody resBody = new RuinsReclaimMatchingStartResponseBody();
		resBody.matchingStatus = (int)matchingRoom.status;
		resBody.remainingTime = matchingRoom.GetRemainingTime(currentTime);
		SendResponseOK(resBody);
	}
}
