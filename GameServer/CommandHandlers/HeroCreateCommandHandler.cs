using System;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class HeroCreateCommandHandler : LobbyCommandHandler<HeroCreateCommandBody, HeroCreateResponseBody>
{
    public const short kResult_HeroCountOverflow = 101;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private Job m_job;

    private Nation m_nation;

    private Guid m_heroId = Guid.Empty;

    private int m_nLevel;

    private int m_nHeroCount;

    private int m_nCustomPresetHair;

    private int m_nCustomFaceJawHeight;

    private int m_nCustomFaceJawWidth;

    private int m_nCustomFaceJawEndHeight;

    private int m_nCustomFaceWidth;

    private int m_nCustomFaceEyebrowHeight;

    private int m_nCustomFaceEyebrowRotation;

    private int m_nCustomFaceEyesWidth;

    private int m_nCustomFaceNoseHeight;

    private int m_nCustomFaceNoseWidth;

    private int m_nCustomFaceMouthHeight;

    private int m_nCustomFaceMouthWidth;

    private int m_nCustomBodyHeadSize;

    private int m_nCustomBodyArmsLength;

    private int m_nCustomBodyArmsWidth;

    private int m_nCustomBodyChestSize;

    private int m_nCustomBodyWaistWidth;

    private int m_nCustomBodyHipsSize;

    private int m_nCustomBodyPelvisWidth;

    private int m_nCustomBodyLegsLength;

    private int m_nCustomBodyLegsWidth;

    private int m_nCustomColorSkin;

    private int m_nCustomColorEyes;

    private int m_nCustomColorBeardAndEyebrow;

    private int m_nCustomColorHair;

    private int m_nWingStartStep;

    private int m_nWingStartLevel;

    protected override void HandleLobbyCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "바디가 null입니다.");
        }
        int nNationId = m_body.nationId;
        int nJobId = m_body.jobId;
        m_nation = Resource.instance.GetNation(nNationId);
        if (m_nation == null)
        {
            throw new CommandHandleException(1, "국가가 존재하지 않습니다. nationId = " + nNationId);
        }
        m_job = Resource.instance.GetJob(nJobId);
        if (m_job == null)
        {
            throw new CommandHandleException(1, "직업이 존재하지 않습니다. jobId = " + nJobId);
        }
        m_nCustomPresetHair = m_body.customPresetHair;
        m_nCustomFaceJawHeight = m_body.customFaceJawHeight;
        m_nCustomFaceJawWidth = m_body.customFaceJawWidth;
        m_nCustomFaceJawEndHeight = m_body.customFaceJawEndHeight;
        m_nCustomFaceWidth = m_body.customFaceWidth;
        m_nCustomFaceEyebrowHeight = m_body.customFaceEyebrowHeight;
        m_nCustomFaceEyebrowRotation = m_body.customFaceEyebrowRotation;
        m_nCustomFaceEyesWidth = m_body.customFaceEyesWidth;
        m_nCustomFaceNoseHeight = m_body.customFaceNoseHeight;
        m_nCustomFaceNoseWidth = m_body.customFaceNoseWidth;
        m_nCustomFaceMouthHeight = m_body.customFaceMouthHeight;
        m_nCustomFaceMouthWidth = m_body.customFaceMouthWidth;
        m_nCustomBodyHeadSize = m_body.customBodyHeadSize;
        m_nCustomBodyArmsLength = m_body.customBodyArmsLength;
        m_nCustomBodyArmsWidth = m_body.customBodyArmsWidth;
        m_nCustomBodyChestSize = m_body.customBodyChestSize;
        m_nCustomBodyWaistWidth = m_body.customBodyWaistWidth;
        m_nCustomBodyHipsSize = m_body.customBodyHipsSize;
        m_nCustomBodyPelvisWidth = m_body.customBodyPelvisWidth;
        m_nCustomBodyLegsLength = m_body.customBodyLegsLength;
        m_nCustomBodyLegsWidth = m_body.customBodyLegsWidth;
        m_nCustomColorSkin = m_body.customColorSkin;
        m_nCustomColorEyes = m_body.customColorEyes;
        m_nCustomColorBeardAndEyebrow = m_body.customColorBeardAndEyebrow;
        m_nCustomColorHair = m_body.customColorHair;
        m_nWingStartStep = 1;
        m_nWingStartLevel = 1;
        m_currentTime = DateTimeUtil.currentTime;
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
            m_nHeroCount = GameDac.HeroCount_x(conn, trans, m_myAccount.id);
            if (m_nHeroCount >= Resource.instance.maxHeroCount)
            {
                throw new CommandHandleException(101, "영웅수가 최대보유수를 초과합니다.");
            }
            m_heroId = Guid.NewGuid();
            m_nLevel = 1;
            if (GameDac.AddHero(conn, trans, m_heroId, m_myAccount.id, m_job.baseJobId, m_job.id, m_nation.id, m_nLevel, 0L, m_nWingStartStep, m_nWingStartLevel, m_currentTime) != 0)
            {
                throw new CommandHandleException(1, "영웅 등록 실패.");
            }
            foreach (JobSkill skill in m_job.skills)
            {
                if (GameDac.AddHeroSkill(conn, trans, m_heroId, skill.skillId, 1) != 0)
                {
                    throw new CommandHandleException(1, "영웅스킬 등록 실패.");
                }
            }
            JobLevel jobLevel = m_job.GetLevel(m_nLevel);
            if (GameDac.UpdateHero_HPAndStamina(conn, trans, m_heroId, jobLevel.maxHp, Resource.instance.maxStamina) != 0)
            {
                throw new CommandHandleException(1, "영웅 수정(HP, 스태미너) 실패");
            }
            if (GameDac.UpdateHero_Custom(conn, trans, m_heroId, m_nCustomPresetHair, m_nCustomFaceJawHeight, m_nCustomFaceJawWidth, m_nCustomFaceJawEndHeight, m_nCustomFaceWidth, m_nCustomFaceEyebrowHeight, m_nCustomFaceEyebrowRotation, m_nCustomFaceEyesWidth, m_nCustomFaceNoseHeight, m_nCustomFaceNoseWidth, m_nCustomFaceMouthHeight, m_nCustomFaceMouthWidth, m_nCustomBodyHeadSize, m_nCustomBodyArmsLength, m_nCustomBodyArmsWidth, m_nCustomBodyChestSize, m_nCustomBodyWaistWidth, m_nCustomBodyHipsSize, m_nCustomBodyPelvisWidth, m_nCustomBodyLegsLength, m_nCustomBodyLegsWidth, m_nCustomColorSkin, m_nCustomColorEyes, m_nCustomColorBeardAndEyebrow, m_nCustomColorHair) != 0)
            {
                throw new CommandHandleException(1, "영웅 수정(커스텀) 실패");
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
        lock (Global.syncObject)
        {
            Cache.instance.GetNationInstance(m_nation.id).nationHeroCount++;
        }
        HeroCreateResponseBody resBody = new HeroCreateResponseBody();
        resBody.heroId = m_heroId;
        resBody.level = m_nLevel;
        resBody.battlePower = 0L;
        SendResponseOK(resBody);
        try
        {
            SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateUserWork(m_myAccount.userId);
            dbWork.AddSqlCommand(UserDac.CSC_AddUserHero(m_heroId, m_myAccount.userId, m_myAccount.virtualGameServerId));
            dbWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex);
        }
    }
}
