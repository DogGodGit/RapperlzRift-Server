using System;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class NationWarJoinCommandHandler : InGameCommandHandler<NationWarJoinCommandBody, NationWarJoinResponseBody>
{
    public const short kResult_NoNationWar = 101;

    public const short kResult_Dead = 102;

    public const short kResult_AlreadRidingCart = 103;

    public const short kResult_LevelUnderflowed = 104;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        NationWarInstance nationWarInst = m_myHero.nationInst.nationWarInst;
        if (nationWarInst == null)
        {
            throw new CommandHandleException(101, "자신의 국가가 국가전 진행중이 아닙니다.");
        }
        Resource res = Resource.instance;
        _ = Cache.instance;
        if (m_myHero.isDead)
        {
            throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
        }
        if (m_myHero.isRidingCart)
        {
            throw new CommandHandleException(103, "영웅이 카트에 탑승중입니다.");
        }
        int nHeroLevel = m_myHero.level;
        int nPvpMinHeroLevel = res.pvpMinHeroLevel;
        if (nHeroLevel < nPvpMinHeroLevel)
        {
            throw new CommandHandleException(104, "영웅의 레벨이 낮아 해당 국가전전장이동을 할 수 없습니다. nHeroLevel = " + nHeroLevel + ", nPvpMinHeroLevel = " + nPvpMinHeroLevel);
        }
        NationWar nationWar = res.nationWar;
        DateTimeOffset currentTime = DateTimeUtil.currentTime;
        Continent targetContinent = null;
        int nTargetNationId = 0;
        Vector3 targetPosition = Vector3.zero;
        float fTargetRotationY = 0f;
        if (nationWarInst.offenseNation.id == m_myHero.nationId)
        {
            targetContinent = nationWar.offenseStartContinent;
            nTargetNationId = nationWarInst.defenseNation.id;
            targetPosition = nationWar.SelectOffenseStartPosition();
            fTargetRotationY = nationWar.SelectDefenseStartRotationY();
        }
        else
        {
            targetContinent = nationWar.defenseStartContinent;
            nTargetNationId = nationWarInst.defenseNation.id;
            targetPosition = nationWar.SelectDefenseStartPosition();
            fTargetRotationY = nationWar.SelectDefenseStartRotationY();
        }
        currentPlace.Exit(m_myHero, isLogOut: false, new ContinentEnterForNationWarJoinParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY, currentTime));
        NationWarJoinResponseBody resBody = new NationWarJoinResponseBody();
        resBody.targetContinentId = targetContinent.id;
        resBody.targetNationId = nTargetNationId;
        SendResponseOK(resBody);
    }
}
