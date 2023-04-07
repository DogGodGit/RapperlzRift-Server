using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class HeroNameSetCommandHandler : LobbyCommandHandler<HeroNameSetCommandBody, HeroNameSetResponseBody>
{
    public const short kResult_InvalidName = 101;

    public const short kResult_HeroNotExist = 102;

    public const short kResult_AlreadyCreated = 103;

    public const short kResult_NameExist = 104;

    public const short kResult_NameIsBanWord = 105;

    private Guid m_heroId = Guid.Empty;

    private string m_sName;

    protected override void HandleLobbyCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "바디가 null입니다.");
        }
        m_heroId = m_body.heroId;
        m_sName = m_body.name;
        if (m_heroId == Guid.Empty)
        {
            throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다.");
        }
        if (string.IsNullOrEmpty(m_sName))
        {
            throw new CommandHandleException(1, "이름이 필요합니다.");
        }
        if (!Resource.instance.IsValidHeroName(m_sName))
        {
            throw new CommandHandleException(101, "이름이 유효하지 않습니다.");
        }
        if (Resource.instance.IsNameBanWord(m_sName))
        {
            throw new CommandHandleException(105, "해당 이름은 금지어입니다.");
        }
        SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
        work.runnable = new SFAction(Process);
        RunWork(work);
    }

    private void Process()
    {
        RegisterName();
        SqlConnection conn = null;
        SqlTransaction trans = null;
        try
        {
            conn = DBUtil.OpenGameDBConnection();
            trans = conn.BeginTransaction();
            DataRow drHero = GameDac.Hero_x(conn, trans, m_heroId);
            if (drHero == null)
            {
                throw new CommandHandleException(102, "해당 영웅이 존재하지 않습니다. id = " + m_heroId);
            }
            Guid accountId = (Guid)drHero["accountId"];
            bool bCreated = Convert.ToBoolean(drHero["created"]);
            if (accountId != m_myAccount.id)
            {
                throw new CommandHandleException(1, "나의 영웅이 아닙니다. id = " + m_heroId);
            }
            if (bCreated)
            {
                throw new CommandHandleException(103, "이미 생성완료되었습니다. id = " + m_heroId);
            }
            if (GameDac.UpdateHero_Name(conn, trans, m_heroId, m_sName) != 0)
            {
                throw new CommandHandleException(1, "영웅 수정(이름) 실패. id = " + m_heroId);
            }
            if (GameDac.UpdateHero_CompleteCreation(conn, trans, m_heroId) != 0)
            {
                throw new CommandHandleException(1, "영웅 수정(생성완료) 실패. id = " + m_heroId);
            }
            SFDBUtil.Commit(ref trans);
            SFDBUtil.Close(ref conn);
        }
        catch (Exception)
        {
            DeleteHeroName();
            throw;
        }
        finally
        {
            SFDBUtil.Rollback(ref trans);
            SFDBUtil.Close(ref conn);
        }
    }

    private void RegisterName()
    {
        SqlConnection conn = null;
        SqlTransaction trans = null;
        try
        {
            conn = DBUtil.OpenUserDBConnection();
            trans = conn.BeginTransaction();
            int nHeroNameCount = UserDac.HeroNameCount_x(conn, trans, m_sName);
            if (nHeroNameCount > 0)
            {
                throw new CommandHandleException(104, "해당 이름이 이미 존재합니다.", null, bLoggingEnabled: false);
            }
            if (UserDac.AddHeroName(conn, trans, m_sName) != 0)
            {
                throw new CommandHandleException(1, "영웅이름 추가 실패.");
            }
            SFDBUtil.Commit(ref trans);
            SFDBUtil.Close(ref conn);
        }
        finally
        {
            SFDBUtil.Rollback(ref trans);
            SFDBUtil.Close(ref conn);
        }
    }

    protected override void OnWork_Success(SFWork work)
    {
        base.OnWork_Success(work);
        SendResponseOK(null);
        ProcessUpdateHeroName();
    }

    private void DeleteHeroName()
    {
        SFSqlStandaloneWork dbWork = SqlStandaloneWorkUtil.CreateUserDBWork();
        dbWork.AddSqlCommand(UserDac.CSC_DeleteHeroName(m_sName));
        dbWork.Schedule();
    }

    private void ProcessUpdateHeroName()
    {
        try
        {
            SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateUserWork(m_myAccount.userId);
            dbWork.AddSqlCommand(UserDac.CSC_UpdateHeroName_HeroInfo(m_sName, m_heroId, m_myAccount.virtualGameServerId));
            dbWork.AddSqlCommand(UserDac.CSC_UpdateUserHero_Name(m_heroId, m_sName));
            dbWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex);
        }
    }
}
