using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildApplicationAcceptCommandHandler : InGameCommandHandler<GuildApplicationAcceptCommandBody, GuildApplicationAcceptResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_ApplicationNotExist = 103;

	public const short kResult_GuildMemberFull = 104;

	public const short kResult_AlreadyGuildMember = 105;

	private GuildMember m_myGuildMember;

	private GuildApplication m_targetApplication;

	private Guild m_targetGuild;

	private GuildMember m_newGuildMember;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid applicationId = (Guid)m_body.applicationId;
		if (applicationId == Guid.Empty)
		{
			throw new CommandHandleException(1, "신청ID가 유효하지 않습니다. applicationId = " + applicationId);
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입하지 않았습니다.");
		}
		if (!m_myGuildMember.grade.applicationAccecptanceEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		m_targetGuild = m_myGuildMember.guild;
		if (m_targetGuild.isMemberFull)
		{
			throw new CommandHandleException(104, "길드가 만원입니다. guildId = " + m_targetGuild.id);
		}
		m_targetApplication = m_targetGuild.GetApplication(applicationId);
		if (m_targetApplication == null)
		{
			throw new CommandHandleException(103, string.Concat("길드 신청이 존재하지 않습니다. guildId = ", m_targetGuild.id, ", applicationId = ", applicationId));
		}
		if (Cache.instance.GetGuildMember(m_targetApplication.heroId) != null)
		{
			throw new CommandHandleException(105, string.Concat("신청자가 이미 길드에 가입되어 있습니다. guildId = ", m_targetGuild.id, ", applicationId = ", applicationId));
		}
		if (m_targetApplication.isLoggedIn)
		{
			HeroSynchronizer.Exec(m_targetApplication.hero, new SFAction(Process));
		}
		else
		{
			Process();
		}
	}

	private void Process()
	{
		Hero applicant = m_targetApplication.hero;
		m_targetGuild.RemoveApplication(m_targetApplication);
		applicant?.RemoveGuildApplication(m_targetApplication.id);
		m_newGuildMember = new GuildMember(m_targetGuild);
		if (applicant != null)
		{
			m_newGuildMember.Init(4, applicant);
		}
		else
		{
			m_newGuildMember.Init(4, m_targetApplication.heroId, m_targetApplication.heroName, m_targetApplication.heroJobId, m_targetApplication.heroLevel, m_targetApplication.heroVipLevel, m_targetApplication.heroLogoutTime);
		}
		m_targetGuild.Enter(m_newGuildMember);
		if (applicant != null)
		{
			ServerEvent.SendGuildApplicationAccepted(applicant.account.peer, m_targetApplication.id, m_targetGuild.ToPDGuild(m_currentTime), m_newGuildMember.grade.id, applicant.realMaxHP);
		}
		SaveToDB();
		SaveToGameLogDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_targetApplication.guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_targetApplication.heroId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SetGuild(m_targetApplication.heroId, m_targetApplication.guild.id, 4));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuildApplication(m_targetApplication.id, 1));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildApplicationAcceptanceLog(Guid.NewGuid(), m_targetGuild.id, m_myHero.id, m_myGuildMember.grade.id, m_newGuildMember.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
