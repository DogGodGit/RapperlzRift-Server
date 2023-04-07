using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class GuildExitCommandHandler : InGameCommandHandler<GuildExitCommandBody, GuildExitResponseBody>
{
    public const short kResult_NoGuildMember = 101;

    public const short kResult_MasterNotAllowed = 102;

    public const short kResult_ProgressingGuildSuppySupportQuest = 103;

    private Guild m_guild;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        GuildMember member = m_myHero.guildMember;
        if (member == null)
        {
            throw new CommandHandleException(101, "길드에 가입하지 않았습니다.");
        }
        if (member.isMaster)
        {
            throw new CommandHandleException(102, "길드장은 탈퇴할 수 없습니다.");
        }
        m_guild = member.guild;
        GuildSupplySupportQuestPlay guildQuest = m_guild.guildSupplySupportQuestPlay;
        if (guildQuest != null && m_myHero.id == guildQuest.heroId)
        {
            throw new CommandHandleException(103, "영웅이 길드물자지원퀘스트를 진행중입니다");
        }
        m_guild.Exit(member, bIsBanished: false, m_currentTime);
        SaveToDB();
        GuildExitResponseBody resBody = new GuildExitResponseBody();
        resBody.maxHp = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        resBody.previousContinentId = m_myHero.previousContinentId;
        resBody.previousNationId = m_myHero.previousNationId;
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
        dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ExitGuild(m_myHero.id, m_currentTime));
        dbWork.Schedule();
    }
}
