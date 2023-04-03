using ClientCommon;

namespace GameServer;

public class GuildApplicationListCommandHandler : InGameCommandHandler<GuildApplicationListCommandBody, GuildApplicationListResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입된 상태가 아닙니다.");
		}
		if (!guildMember.grade.applicationAccecptanceEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		Guild guild = guildMember.guild;
		GuildApplicationListResponseBody resBody = new GuildApplicationListResponseBody();
		resBody.applications = GuildApplication.GetPDGuildApplications(guild.applications.Values).ToArray();
		SendResponseOK(resBody);
	}
}
