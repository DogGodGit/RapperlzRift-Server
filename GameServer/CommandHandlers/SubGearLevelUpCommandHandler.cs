using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class SubGearLevelUpCommandHandler : InGameCommandHandler<SubGearLevelUpCommandBody, SubGearLevelUpResponseBody>
{
    public const short kResult_LastLevel = 101;

    public const short kResult_QualityUpRequired = 102;

    public const short kResult_GradeUpRequired = 103;

    public const short kResult_NotEnoughGold = 104;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private HeroSubGear m_heroSubGear;

    private long m_lnUsedGold;

    private int m_nOldLevel;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSubGearId = m_body.subGearId;
        if (nSubGearId <= 0)
        {
            throw new CommandHandleException(1, "보조장비 ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
        }
        m_heroSubGear = m_myHero.GetSubGear(nSubGearId);
        if (m_heroSubGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅보조장비입니다. nSubGearId = " + nSubGearId);
        }
        if (!m_heroSubGear.equipped)
        {
            throw new CommandHandleException(1, "장착하지 않은 영웅보조장비입니다.");
        }
        if (m_heroSubGear.level >= m_myHero.level)
        {
            throw new CommandHandleException(1, "보조장비 레벨은 영웅 레벨을 넘어갈 수 없습니다.");
        }
        if (m_heroSubGear.isLastLevel)
        {
            throw new CommandHandleException(101, "최대레벨입니다.");
        }
        if (!m_heroSubGear.isLastQualityOfCurrentLevel)
        {
            throw new CommandHandleException(102, "품질을 올려야합니다.");
        }
        if (m_heroSubGear.isLastLevelOfCurrentGrade)
        {
            throw new CommandHandleException(103, "등급을 올려야합니다.");
        }
        m_nOldLevel = m_heroSubGear.level;
        int nRequiredGold = m_heroSubGear.subGearLevel.nextLevelUpRequiredGold;
        if (m_myHero.gold < nRequiredGold)
        {
            throw new CommandHandleException(104, "골드가 부족합니다.");
        }
        m_myHero.UseGold(nRequiredGold);
        SubGearLevel nextLevel = m_heroSubGear.subGear.GetLevel(m_heroSubGear.level + 1);
        m_heroSubGear.subGearLevel = nextLevel;
        m_heroSubGear.quality = nextLevel.firstQuality.quality;
        m_lnUsedGold += nRequiredGold;
        m_heroSubGear.RefreshAttrTotalValues();
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB();
        SubGearLevelUpResponseBody resBody = new SubGearLevelUpResponseBody();
        resBody.subGearLevel = m_heroSubGear.level;
        resBody.subGearQuality = m_heroSubGear.quality;
        resBody.gold = m_myHero.gold;
        resBody.maxHp = m_myHero.realMaxHP;
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubGear_LevelUp(m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_heroSubGear.level, m_heroSubGear.quality));
        dbWork.Schedule();
        SaveToDB_AddSubGearLevelUpLog();
    }

    private void SaveToDB_AddSubGearLevelUpLog()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddSubGearLevelUpLog(Guid.NewGuid(), m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_nOldLevel, m_heroSubGear.level, m_lnUsedGold, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
