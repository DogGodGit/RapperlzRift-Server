using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class GuildAppointCommandHandler : InGameCommandHandler<GuildAppointCommandBody, GuildAppointResponseBody>
{
    public const short kResult_NoGuildMember = 101;

    public const short kResult_NoAuthority = 102;

    public const short kResult_ViceMasterIsFull = 103;

    public const short kResult_LordIsFull = 104;

    public const short kResult_TargetNotExist = 105;

    public const short kResult_AlreadyAppointedGrade = 106;

    private Guild m_myGuild;

    private GuildMember m_myGuildMember;

    private GuildMember m_targetGuildMember;

    private GuildMemberGrade m_targetMemberGrade;

    private int m_nOldTargetHeroGuildMemberGrade;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid targetMemberId = m_body.targetMemberId;
        int targetMemberGrade = m_body.targetMemberGrade;
        if (targetMemberId == Guid.Empty)
        {
            throw new CommandHandleException(1, "목표 멤버의 ID가 유효하지 않습니다. targetMemberId = " + targetMemberId);
        }
        if (targetMemberId == m_myHero.id)
        {
            throw new CommandHandleException(1, "자기 자신은 임명할 수 없습니다.");
        }
        if (targetMemberGrade <= 0)
        {
            throw new CommandHandleException(1, "목표 멤버등급이 유효하지 않습니다. targetMemberGrade = " + targetMemberGrade);
        }
        m_myGuildMember = m_myHero.guildMember;
        if (m_myGuildMember == null)
        {
            throw new CommandHandleException(101, "길드에 가입되지 않았습니다.");
        }
        m_myGuild = m_myGuildMember.guild;
        m_targetMemberGrade = Resource.instance.GetGuildMemberGrade(targetMemberGrade);
        if (m_targetMemberGrade == null)
        {
            throw new CommandHandleException(1, "멤버등급이 존재하지 않습니다. targetMemberGrade = " + targetMemberGrade);
        }
        if (m_targetMemberGrade.id <= m_myGuildMember.grade.id)
        {
            throw new CommandHandleException(102, "권한이 없습니다.");
        }
        switch (m_targetMemberGrade.id)
        {
            case 2:
                if (m_myGuild.isViceMasterFull)
                {
                    throw new CommandHandleException(103, "부길드장 인원이 최대입니다.");
                }
                break;
            case 3:
                if (m_myGuild.isLordFull)
                {
                    throw new CommandHandleException(104, "로드 인원이 최대입니다.");
                }
                break;
        }
        m_targetGuildMember = m_myGuild.GetMember(targetMemberId);
        if (m_targetGuildMember == null)
        {
            throw new CommandHandleException(105, "목표멤버가 존재하지 않습니다. targetMemberId = " + targetMemberId);
        }
        if (m_targetGuildMember.grade == m_targetMemberGrade)
        {
            throw new CommandHandleException(106, "이미 해당 등급의 멤버입니다.");
        }
        if (m_targetGuildMember.grade.id <= m_myGuildMember.grade.id)
        {
            throw new CommandHandleException(102, string.Concat("권한이 없습니다. targetMemberId = ", m_targetGuildMember.id, ", targetMemberGrade = ", m_targetGuildMember.grade.id));
        }
        m_nOldTargetHeroGuildMemberGrade = m_targetGuildMember.grade.id;
        if (m_targetGuildMember.isLoggedIn)
        {
            HeroSynchronizer.Exec(m_targetGuildMember.hero, new SFAction(Process));
        }
        else
        {
            Process();
        }
    }

    private void Process()
    {
        m_targetGuildMember.grade = m_targetMemberGrade;
        Hero hero = m_targetGuildMember.hero;
        if (hero != null && hero.currentPlace != null)
        {
            ServerEvent.SendHeroGuildInfoUpdated(hero.currentPlace.GetDynamicClientPeers(hero.sector, hero.id), hero.id, m_myGuild.id, m_myGuild.name, m_targetGuildMember.grade.id);
        }
        ServerEvent.SendGuildAppointed(m_myGuild.GetClientPeers(Guid.Empty), m_myGuildMember.id, m_myGuildMember.name, m_myGuildMember.grade.id, m_targetGuildMember.id, m_targetGuildMember.name, m_targetGuildMember.grade.id);
        SaveToDB();
        SaveToGameLogDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
        dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_targetGuildMember.id));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildMemberGrade(m_targetGuildMember.id, m_targetGuildMember.grade.id));
        dbWork.Schedule();
    }

    private void SaveToGameLogDB()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddGuildAppointmentLog(Guid.NewGuid(), m_myGuild.id, m_myGuildMember.id, m_myGuildMember.grade.id, m_targetGuildMember.id, m_nOldTargetHeroGuildMemberGrade, m_targetGuildMember.grade.id, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
