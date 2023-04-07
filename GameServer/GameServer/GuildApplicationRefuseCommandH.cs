using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildApplicationRefuseCommandHandler : InGameCommandHandler<GuildApplicationRefuseCommandBody, GuildApplicationRefuseResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_ApplicationNotExist = 103;

	private GuildApplication m_app;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid applicationId = (Guid)m_body.applicationId;
		if (applicationId == Guid.Empty)
		{
			throw new CommandHandleException(1, "신청ID가 유효하지 않습니다. applicationId = " + applicationId);
		}
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입하지 않았습니다.");
		}
		if (!guildMember.grade.applicationAccecptanceEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		Guild guild = guildMember.guild;
		m_app = guild.GetApplication(applicationId);
		if (m_app == null)
		{
			throw new CommandHandleException(103, "신청이 존재하지 않습니다. applicationId = " + applicationId);
		}
		guild.RemoveApplication(m_app);
		if (m_app.isLoggedIn)
		{
			Hero applicant = m_app.hero;
			lock (applicant.syncObject)
			{
				applicant.RemoveGuildApplication(m_app.id);
				ServerEvent.SendGuildApplicationRefused(applicant.account.peer, m_app.id);
				Finish();
				return;
			}
		}
		Finish();
	}

	private void Finish()
	{
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_app.guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_app.heroId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuildApplication(m_app.id, 2));
		dbWork.Schedule();
	}
}
