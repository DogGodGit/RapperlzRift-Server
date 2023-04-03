using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildNoticeSetCommandHandler : InGameCommandHandler<GuildNoticeSetCommandBody, GuildNoticeSetResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_InvalidLength = 103;

	private Guild m_myGuild;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		string sNotice = m_body.notice;
		if (sNotice != null && sNotice.Length >= Resource.instance.guildNoticeMaxLength)
		{
			throw new CommandHandleException(103, "공지 길이가 유효하지 않습니다.");
		}
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		if (!myGuildMember.isMaster)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		m_myGuild = myGuildMember.guild;
		m_myGuild.notice = sNotice;
		SaveToDB();
		ServerEvent.SendGuildNoticeChanged(m_myGuild.GetClientPeers(m_myHero.id), m_myGuild.notice);
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Notice(m_myGuild.id, m_myGuild.notice));
		dbWork.Schedule();
	}
}
