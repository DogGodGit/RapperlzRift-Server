using ClientCommon;

namespace GameServer.CommandHandlers;

public class FishingStartCommandHandler : InGameCommandHandler<FishingStartCommandBody, FishingStartResponseBody>
{
    public const short kResult_Dead = 101;

    public const short kResult_AlreadyRidingMount = 103;

    public const short kResult_OtherActionPerforming = 104;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSpotId = m_body.spotId;
        if (nSpotId <= 0)
        {
            throw new CommandHandleException(1, "지점ID가 유효하지 않습니다. nSpotId = " + nSpotId);
        }
        if (m_myHero.isDead)
        {
            throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
        }
        if (m_myHero.isRiding)
        {
            throw new CommandHandleException(103, "영웅이 탈것을 타고있는 상태입니다.");
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
            throw new CommandHandleException(104, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
        }
        HeroFishingQuest heroFishingQuest = m_myHero.fishingQuest;
        if (heroFishingQuest == null)
        {
            throw new CommandHandleException(1, "영웅 낚시퀘스트가 존재하지 않습니다.");
        }
        Place currentPlace = m_myHero.currentPlace;
        if (currentPlace is ContinentInstance)
        {
            ContinentInstance continentInstance = (ContinentInstance)currentPlace;
            FishingQuestSpot spot2 = Resource.instance.fishingQuest.GetSpot(nSpotId);
            if (spot2 == null)
            {
                throw new CommandHandleException(1, "현재 대륙에 없는 지점입니다. nSpotId = " + nSpotId);
            }
            if (!continentInstance.IsSame(spot2.continentId, m_myHero.nationId))
            {
                throw new CommandHandleException(1, "낚시가 가능한 지역이 아닙니다.");
            }
            if (!spot2.IsTargetAreaPosition(m_myHero.position, m_myHero.radius))
            {
                throw new CommandHandleException(1, "낚시 지점으로부터 멀리떨어져 있습니다.");
            }
        }
        else
        {
            if (!(currentPlace is GuildTerritoryInstance))
            {
                throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
            }
            _ = (GuildTerritoryInstance)currentPlace;
            FishingQuestGuildTerritorySpot spot = Resource.instance.fishingQuest.GetGuildTerritorySpot(nSpotId);
            if (spot == null)
            {
                throw new CommandHandleException(1, "현재 길드영지에 없는 지점입니다. nSpotId = " + nSpotId);
            }
            if (!spot.IsTargetAreaPosition(m_myHero.position, m_myHero.radius))
            {
                throw new CommandHandleException(1, "낚시 지점으로부터 멀리떨어져 있습니다.");
            }
        }
        m_myHero.StartFishing();
        SendResponseOK(null);
    }
}
