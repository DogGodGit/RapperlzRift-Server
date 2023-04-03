using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildMemberBanishCommandHandler : InGameCommandHandler<GuildMemberBanishCommandBody, GuildMemberBanishResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_TargetNotExist = 103;

	public const short kResult_DailyMemberBanishmentCountOverflowed = 104;

	public const short kResult_ProgressingGuildSuppySupportQuest = 105;

	private Guild m_myGuild;

	private GuildMember m_myGuildMember;

	private GuildMember m_targetMember;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid targetMemberId = (Guid)m_body.targetMemberId;
		if (targetMemberId == Guid.Empty)
		{
			throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다. targetMemberId = " + targetMemberId);
		}
		if (targetMemberId == m_myHero.id)
		{
			throw new CommandHandleException(1, "자기자신은 추방시킬 수 없습니다. targetMemberId = " + targetMemberId);
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_targetMember = m_myGuild.GetMember(targetMemberId);
		if (m_targetMember == null)
		{
			throw new CommandHandleException(103, "대상멤버가 존재하지 않습니다.");
		}
		GuildSupplySupportQuestPlay guildQuest = m_myGuild.guildSupplySupportQuestPlay;
		if (guildQuest != null && targetMemberId == guildQuest.heroId)
		{
			throw new CommandHandleException(105, "대상멤버가 길드물자지원퀘스트를 진행중입니다");
		}
		m_myGuild.RefreshDailyBanishmentCount(m_currentDate);
		if (m_myGuild.dailyBanishmentCount.value >= Resource.instance.guildDailyBanishmentMaxCount)
		{
			throw new CommandHandleException(104, "길드 일일추방횟수가 최대횟수를 넘어갑니다.");
		}
		if (m_targetMember.grade.id <= m_myGuildMember.grade.id)
		{
			throw new CommandHandleException(102, string.Concat("권한이 없습니다. targetMemberId = ", m_targetMember.id, ", targetMemberGrade = ", m_targetMember.grade.id));
		}
		if (m_targetMember.isLoggedIn)
		{
			HeroSynchronizer.Exec(m_targetMember.hero, new SFAction(Process));
		}
		else
		{
			Process();
		}
	}

	private void Process()
	{
		Hero targetHero = m_targetMember.hero;
		m_myGuild.Exit(m_targetMember, bIsBanished: true, m_currentTime);
		m_myGuild.dailyBanishmentCount.value++;
		if (targetHero != null)
		{
			ServerEvent.SendGuildBanished(targetHero.account.peer, targetHero.realMaxHP, targetHero.hp, targetHero.previousContinentId, targetHero.previousNationId);
		}
		SaveToDB();
		SaveToGameLogDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_targetMember.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_MemberBanishmentDateCount(m_myGuild.id, m_myGuild.dailyBanishmentCount.date, m_myGuild.dailyBanishmentCount.value));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ExitGuild(m_targetMember.id, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildBanishmentLog(Guid.NewGuid(), m_myGuild.id, m_myGuildMember.id, m_myGuildMember.grade.id, m_targetMember.id, m_targetMember.grade.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
