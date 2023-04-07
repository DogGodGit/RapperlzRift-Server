using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class GuildApplyCommandHandler : InGameCommandHandler<GuildApplyCommandBody, GuildApplyResponseBody>
{
    public const short kResult_GuildNotExist = 101;

    public const short kResult_GuildMemberFull = 102;

    public const short kResult_AlreadyGuildMember = 103;

    public const short kResult_JoinWaitTimeNotElapsed = 104;

    public const short kResult_NotEnoughHeroLevel = 105;

    public const short kResult_GuildApplicationFull = 106;

    public const short kResult_AlreadyAppliedGuild = 107;

    public const short kResult_DailyApplicationCountOverflowed = 108;

    private GuildApplication m_addedApplication;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid guildId = m_body.guildId;
        if (guildId == Guid.Empty)
        {
            throw new CommandHandleException(1, "길드ID가 유효하지 않습니다. guildId = " + guildId);
        }
        if (m_myHero.guildMember != null)
        {
            throw new CommandHandleException(103, "이미 길드멤버입니다.");
        }
        if (m_myHero.GetGuildJoinWaitTime(m_currentTime) > 0f)
        {
            throw new CommandHandleException(104, "아직 길드가입 대기시간이 종료되지 않았습니다.");
        }
        if (m_myHero.level < Resource.instance.guildRequiredHeroLevel)
        {
            throw new CommandHandleException(105, "레벨이 부족합니다.");
        }
        if (m_myHero.ContainsGuildApplication(guildId))
        {
            throw new CommandHandleException(107, "이미 신청한 길드입니다. guildId = " + guildId);
        }
        m_myHero.RefreshDailyGuildApplicationCount(m_currentTime.Date);
        DateValuePair<int> dailyGuildApplicationCount = m_myHero.dailyGuildApplicationCount;
        if (dailyGuildApplicationCount.value >= Resource.instance.guildDailyApplicationMaxCount)
        {
            throw new CommandHandleException(108, "길드 신청가능한 횟수를 넘어갑니다.");
        }
        Guild targetGuild = Cache.instance.GetGuild(guildId);
        if (targetGuild == null)
        {
            throw new CommandHandleException(101, "길드가 존재하지 않습니다.");
        }
        if (targetGuild.isMemberFull)
        {
            throw new CommandHandleException(102, "만원입니다.");
        }
        if (targetGuild.nationInst.nationId != m_myHero.nationId)
        {
            throw new CommandHandleException(1, "같은 국가만 신청할 수 있습니다.");
        }
        if (targetGuild.applications.Count >= Resource.instance.guildApplicationReceptionMaxCount)
        {
            throw new CommandHandleException(106, "길드 가입신청접수건수가 최대치입니다.");
        }
        m_addedApplication = new GuildApplication(targetGuild);
        m_addedApplication.Init(m_myHero);
        targetGuild.AddApplication(m_addedApplication);
        m_myHero.AddGuildApplication(m_addedApplication);
        dailyGuildApplicationCount.value++;
        SaveToDB();
        GuildApplyResponseBody resBody = new GuildApplyResponseBody();
        resBody.application = m_addedApplication.ToPDHeroGuildApplication();
        resBody.date = m_currentTime.Date;
        resBody.dailyApplicationCount = dailyGuildApplicationCount.value;
        SendResponseOK(resBody);
    }

    public void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_addedApplication.guild.id);
        dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
        dbWork.AddSqlCommand(GameDac.CSC_AddGuildApplication(m_addedApplication.id, m_addedApplication.guild.id, m_addedApplication.heroId, m_currentTime));
        dbWork.Schedule();
    }
}
