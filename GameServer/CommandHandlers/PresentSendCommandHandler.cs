using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class PresentSendCommandHandler : InGameCommandHandler<PresentSendCommandBody, PresentSendResponseBody>
{
    public const short kResult_TargetHeroNotExist = 101;

    public const short kResult_TargetHeroCreating = 102;

    public const short kResult_NotEnoughVipLevel = 103;

    public const short kResult_NotEnoughDia = 104;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private DateTime m_currentWeekStartDate = DateTime.MinValue.Date;

    private Guid m_targetHeroId = Guid.Empty;

    private string m_sTargetName;

    private int m_nTargetNationId;

    private Hero m_targetHero;

    private int m_nPresentId;

    private Present m_present;

    private int m_nUsedOwnDia;

    private int m_nUsedUnOwnDia;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        m_targetHeroId = m_body.targetHeroid;
        if (m_targetHeroId == Guid.Empty)
        {
            throw new CommandHandleException(1, "대상영웅ID가 유효하지 않습니다.");
        }
        m_nPresentId = m_body.presentId;
        if (m_nPresentId <= 0)
        {
            throw new CommandHandleException(1, "선물ID가 유효하지 않습니다. m_nPresentId = " + m_nPresentId);
        }
        m_present = Resource.instance.GetPresent(m_nPresentId);
        if (m_present == null)
        {
            throw new CommandHandleException(1, "선물이 존재하지 않습니다. m_nPresentId = " + m_nPresentId);
        }
        m_targetHero = Cache.instance.GetLoggedInHero(m_targetHeroId);
        if (m_targetHero != null)
        {
            lock (m_targetHero.syncObject)
            {
                Process();
                return;
            }
        }
        SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
        work.runnable = new SFAction(GetTargetHeroInfo);
        RunWork(work);
    }

    private void GetTargetHeroInfo()
    {
        SqlConnection conn = null;
        try
        {
            conn = DBUtil.OpenGameDBConnection();
            DataRow drHero = GameDac.Hero(conn, null, m_targetHeroId);
            if (drHero == null)
            {
                throw new CommandHandleException(101, "대상영웅이 존재하지 않습니다. m_targetHeroId = " + m_targetHeroId);
            }
            if (!Convert.ToBoolean(drHero["created"]))
            {
                throw new CommandHandleException(102, "대상영웅은 생성중입니다. m_targetHeroId = " + m_targetHeroId);
            }
            m_sTargetName = Convert.ToString(drHero["name"]);
            m_nTargetNationId = Convert.ToInt32(drHero["nationId"]);
            SFDBUtil.Close(ref conn);
        }
        finally
        {
            SFDBUtil.Close(ref conn);
        }
    }

    protected override void OnWork_Success(SFWork work)
    {
        base.OnWork_Success(work);
        m_targetHero = Cache.instance.GetLoggedInHero(m_targetHeroId);
        if (m_targetHero != null)
        {
            lock (m_targetHero.syncObject)
            {
                Process();
                return;
            }
        }
        Process();
    }

    private void Process()
    {
        m_currentTime = DateTimeUtil.currentTime;
        m_currentWeekStartDate = DateTimeUtil.GetWeekStartDate(m_currentTime);
        if (m_targetHero != null)
        {
            m_sTargetName = m_targetHero.name;
            m_nTargetNationId = m_targetHero.nationId;
        }
        if (m_myHero.vipLevel.level < m_present.requiredVipLevel)
        {
            throw new CommandHandleException(103, "Vip레벨이 부족합니다.");
        }
        if (m_myHero.dia < m_present.requiredDia)
        {
            throw new CommandHandleException(104, "다이아가 부족합니다.");
        }
        m_myHero.UseDia(m_present.requiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
        m_myHero.RefreshWeeklyPresentContributionPoint(m_currentWeekStartDate);
        m_myHero.AddWeeklyPresentContributionPoint(m_present.contributionPoint, m_currentTime);
        if (m_targetHero != null)
        {
            m_targetHero.RefreshWeeklyPresentPopularityPoint(m_currentWeekStartDate);
            m_targetHero.AddWeeklyPresentPopularityPoint(m_present.popularityPoint, m_currentTime);
            ServerEvent.SendPresentReceived(m_targetHero.account.peer, m_myHero.id, m_myHero.name, m_myHero.nationId, m_nPresentId, m_targetHero.weeklyPresentPopularityPointStartDate, m_targetHero.weeklyPresentPopularityPoint);
        }
        if (m_present.isEffectDisplay || m_present.isMessageSend)
        {
            ServerEvent.SendHeroPresent(Cache.instance.GetClientPeers(Guid.Empty), m_myHero.id, m_myHero.name, m_myHero.nationId, m_targetHeroId, m_sTargetName, m_nTargetNationId, m_nPresentId);
        }
        SaveToDB();
        SaveToGameLogDB();
        PresentSendResponseBody resBody = new PresentSendResponseBody();
        resBody.weekStartDate = m_currentWeekStartDate;
        resBody.weeklyPresentContributionPoint = m_myHero.weeklyPresentContributionPoint;
        resBody.ownDia = m_myHero.ownDia;
        resBody.unOwnDia = m_myHero.unOwnDia;
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
        dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_targetHeroId));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
        dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWeeklyPresentContributionPoint(m_myHero.id, m_myHero.weeklyPresentContributionPointStartDate, m_myHero.weeklyPresentContributionPoint, m_myHero.weeklyPresentContributionPointUpdateTime));
        if (m_targetHero != null)
        {
            dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWeeklyPresentPopularityPoint(m_targetHeroId, m_targetHero.weeklyPresentPopularityPointStartDate, m_targetHero.weeklyPresentPopularityPoint, m_targetHero.weeklyPresentPopularityPointUpdateTime));
        }
        else
        {
            dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWeeklyPresentPopularityPoint_Add(m_targetHeroId, m_currentWeekStartDate, m_present.popularityPoint, m_currentTime));
        }
        dbWork.Schedule();
    }

    private void SaveToGameLogDB()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroPresentLog(Guid.NewGuid(), m_myHero.id, m_targetHeroId, m_present.id, m_present.contributionPoint, m_present.popularityPoint, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
