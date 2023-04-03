using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildMasterTransferCommandHandler : InGameCommandHandler<GuildMasterTransferCommandBody, GuildMasterTransferResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_TargetNotExist = 103;

	private Guild m_myGuild;

	private GuildMember m_myGuildMember;

	private GuildMember m_targetGuildMember;

	private int m_nOldTargetHeroGuildMemberGrade;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		Guid targetMemberId = (Guid)m_body.targetMemberId;
		if (targetMemberId == Guid.Empty)
		{
			throw new CommandHandleException(1, "목표멤버ID가 유효하지 않습니다. targetMemberId = " + targetMemberId);
		}
		if (targetMemberId == m_myHero.id)
		{
			throw new CommandHandleException(1, "나 자신을 임명할 수 없습니다.");
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되지 않았습니다.");
		}
		if (!m_myGuildMember.isMaster)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_targetGuildMember = m_myGuild.GetMember(targetMemberId);
		if (m_targetGuildMember == null)
		{
			throw new CommandHandleException(103, "존재하지 않는 길드멤버 입니다.");
		}
		if (!m_targetGuildMember.isViceMaster)
		{
			throw new CommandHandleException(1, "길드장 양도는 부길드장한테만 가능합니다.");
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
		m_targetGuildMember.grade = m_myGuildMember.grade;
		Hero targetHero = m_targetGuildMember.hero;
		if (targetHero != null && targetHero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildInfoUpdated(targetHero.currentPlace.GetDynamicClientPeers(targetHero.sector, targetHero.id), targetHero.id, m_myGuild.id, m_myGuild.name, m_targetGuildMember.grade.id);
		}
		m_myGuildMember.grade = Resource.instance.GetGuildMemberGrade(4);
		if (m_myHero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildInfoUpdated(m_myHero.currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, m_myGuild.id, m_myGuild.name, m_myGuildMember.grade.id);
		}
		m_myGuild.master = m_targetGuildMember;
		ServerEvent.SendGuildMasterTransferred(m_myGuild.GetClientPeers(Guid.Empty), m_myGuildMember.id, m_myGuildMember.name, m_targetGuildMember.id, m_targetGuildMember.name);
		SaveToDB();
		SaveToGameLogDB();
		GuildMasterTransferResponseBody resBody = new GuildMasterTransferResponseBody();
		resBody.memberGrade = m_myGuildMember.grade.id;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myGuildMember.id));
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_targetGuildMember.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildMemberGrade(m_myGuildMember.id, m_myGuildMember.grade.id));
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
