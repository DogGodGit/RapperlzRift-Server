using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class HeroDeleteCommandHandler : LobbyCommandHandler<HeroDeleteCommandBody, HeroDeleteResponseBody>
{
    public const short kResult_HeroNotExist = 101;

    private Guid m_heroId = Guid.Empty;

    private int m_nNationId;

    private int m_nHeroCount;

    protected override void HandleLobbyCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "바디가 null입니다.");
        }
        m_heroId = m_body.heroId;
        if (m_heroId == Guid.Empty)
        {
            throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다.");
        }
        SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
        work.runnable = new SFAction(Process);
        RunWork(work);
    }

    private void Process()
    {
        SqlConnection conn = null;
        SqlTransaction trans = null;
        try
        {
            conn = DBUtil.OpenGameDBConnection();
            trans = conn.BeginTransaction();
            DataRow drHero = GameDac.Hero_x(conn, trans, m_heroId);
            if (drHero == null)
            {
                throw new CommandHandleException(101, "해당 영웅이 존재하지 않습니다. id = " + m_heroId);
            }
            Guid accountId = (Guid)drHero["accountId"];
            if (accountId != m_myAccount.id)
            {
                throw new CommandHandleException(1, "나의 영웅이 아닙니다. id = " + m_heroId);
            }
            m_nNationId = Convert.ToInt32(drHero["nationId"]);
            if (GameDac.DeleteHero(conn, trans, m_heroId, DateTimeUtil.currentTime) != 0)
            {
                throw new CommandHandleException(1, "영웅 삭제 실패.");
            }
            m_nHeroCount = GameDac.HeroCount(conn, trans, m_myAccount.id);
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
        lock (Global.syncObject)
        {
            Cache.instance.GetNationInstance(m_nNationId).nationHeroCount--;
        }
        SendResponseOK(null);
        try
        {
            SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateUserWork(m_myAccount.userId);
            dbWork.AddSqlCommand(UserDac.CSC_DeleteUserHero(m_heroId));
            dbWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex);
        }
    }
}
