using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildWeeklyObjectiveSetCommandHandler : InGameCommandHandler<GuildWeeklyObjectiveSetCommandBody, GuildWeeklyObjectiveSetResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_AlreadyObjectiveSet = 103;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nObjectiveId = m_body.objectiveId;
		if (nObjectiveId <= 0)
		{
			throw new CommandHandleException(1, "보상ID가 유효하지 않습니다. nObjectiveId = " + nObjectiveId);
		}
		if (currentDate.DayOfWeek != DayOfWeek.Monday)
		{
			throw new CommandHandleException(1, "설정가능한 요일이 아닙니다.");
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어 있지 않습니다.");
		}
		if (!m_myGuildMember.grade.weeklyObjectiveSettingEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		if (m_myGuild.weeklyObjectiveDate != currentDate)
		{
			throw new CommandHandleException(1, "아직 주간목표가 초기화가 되지 않았습니다.");
		}
		if (m_myGuild.weeklyObjectiveId > 0)
		{
			throw new CommandHandleException(103, "이미 길드주간목표가 설정되어있습니다.");
		}
		GuildWeeklyObjective guildWeeklyObjective = Resource.instance.GetGuildWeeklyObjective(nObjectiveId);
		if (guildWeeklyObjective == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 길드주간목표입니다.");
		}
		m_myGuild.weeklyObjectiveId = nObjectiveId;
		ServerEvent.SendGuildWeeklyObjectiveSet(m_myGuild.GetClientPeers(Guid.Empty), currentDate, nObjectiveId);
		SaveToDB();
		SaveToGameLogDB(nObjectiveId);
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_WeeklyObjective(m_myGuild.id, m_myGuild.weeklyObjectiveDate, m_myGuild.weeklyObjectiveId));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nObjectiveId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildWeeklyObjectiveLog(Guid.NewGuid(), m_myGuild.id, 2, m_myHero.id, nObjectiveId, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
