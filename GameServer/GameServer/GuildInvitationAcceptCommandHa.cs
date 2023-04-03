using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildInvitationAcceptCommandHandler : InGameCommandHandler<GuildInvitationAcceptCommandBody, GuildInvitationAcceptResponseBody>
{
	public const short kResult_InvitationNotExist = 101;

	public const short kResult_GuildMemberFull = 102;

	public const short kResult_AlreadyGuildMember = 103;

	private GuildMember m_member;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid invitationId = (Guid)m_body.invitationId;
		if (invitationId == Guid.Empty)
		{
			throw new CommandHandleException(1, "초대ID가 유효하지 않습니다. invitationId = " + invitationId);
		}
		GuildInvitation invitation = m_myHero.GetGuildInvitation(invitationId);
		if (invitation == null)
		{
			throw new CommandHandleException(101, "초대가 존재하지 않습니다. invitationId = " + invitationId);
		}
		Guild guild = invitation.guild;
		if (guild.isMemberFull)
		{
			throw new CommandHandleException(102, "길드멤버수가 최대입니다.");
		}
		m_member = m_myHero.guildMember;
		if (m_member != null)
		{
			throw new CommandHandleException(103, "이미 길드에 가입되어있습니다.");
		}
		guild.RemoveInvitation(invitation);
		m_myHero.RemoveGuildInvitation(invitation.id);
		m_member = new GuildMember(guild);
		m_member.Init(4, m_myHero);
		guild.Enter(m_member);
		SaveToDB();
		SaveToGameLogDB(invitation);
		GuildInvitationAcceptResponseBody resBody = new GuildInvitationAcceptResponseBody();
		resBody.guild = guild.ToPDGuild(m_currentTime);
		resBody.memberGrade = m_myHero.guildMember.grade.id;
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_member.guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_member.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SetGuild(m_member.id, m_member.guild.id, m_member.grade.id));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(GuildInvitation invitation)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildInvitationAcceptanceLog(Guid.NewGuid(), invitation.guild.id, invitation.inviterId, m_member.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
