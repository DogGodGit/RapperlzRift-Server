using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class DisplayTitleSetCommandHandler : InGameCommandHandler<DisplayTitleSetCommandBody, DisplayTitleSetResponseBody>
{
    public const short kResult_NotExistTitle = 101;

    protected override void HandleInGameCommand()
    {
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
        m_myHero.SetDisplayTitle(heroTitle);
        SaveToDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_DisplayTitle(m_myHero.id, m_myHero.displayTitleId));
        dbWork.Schedule();
    }
}
