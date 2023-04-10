using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class ActivationTitleSetCommandHandler : InGameCommandHandler<ActivationTitleSetCommandBody, ActivationTitleSetResponseBody>
{
    public const short kResult_NotExistTitle = 101;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nTitleId = m_body.titleId;
        if (nTitleId < 0)
        {
            throw new CommandHandleException(1, "유효하지 않은 칭호ID 입니다. nTitleId = " + nTitleId);
        }
        HeroTitle heroTitle = null;
        if (nTitleId > 0)
        {
            heroTitle = m_myHero.GetTitle(nTitleId);
            if (heroTitle == null)
            {
                throw new CommandHandleException(101, "존재하지 않은 칭호입니다. nTitleId = " + nTitleId);
            }
        }
        m_myHero.SetActivationTitle(heroTitle);
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB();
        SaveToGameLogDB(nTitleId);
        ActivationTitleSetResponseBody resBody = new ActivationTitleSetResponseBody();
        resBody.maxHP = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ActivationTitle(m_myHero.id, m_myHero.activationTitleId));
        dbWork.Schedule();
    }

    private void SaveToGameLogDB(int nTitleId)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTitleActivationLog(Guid.NewGuid(), m_myHero.id, nTitleId, nTitleId > 0 ? 1 : 2, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
