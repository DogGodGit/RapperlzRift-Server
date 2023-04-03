using System;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildCreateCommandHandler : InGameCommandHandler<GuildCreateCommandBody, GuildCreateResponseBody>
{
	public const short kResult_AlreadyGuildMember = 101;

	public const short kResult_NotEnoughHeroLevel = 102;

	public const short kResult_NotEnoughVipLevel = 103;

	public const short kResult_NotEnoughDia = 104;

	public const short kResult_NameExist = 105;

	public const short kResult_NameIsBanWord = 106;

	public const short kResult_JoinWaitTimeNotElapsed = 107;

	private string m_sGuildName;

	private Guild m_guild;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_sGuildName = m_body.guildName;
		if (string.IsNullOrEmpty(m_sGuildName))
		{
			throw new CommandHandleException(1, "길드이름이 필요합니다.");
		}
		if (!Resource.instance.IsValidGuildName(m_sGuildName))
		{
			throw new CommandHandleException(1, "길드이름이 유효하지 않습니다.");
		}
		if (Resource.instance.IsNameBanWord(m_sGuildName))
		{
			throw new CommandHandleException(106, "해당 이름은 금지어입니다.");
		}
		SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
		work.runnable = new SFAction(RegisterName);
		RunWork(work);
	}

	private void RegisterName()
	{
		SqlConnection conn = null;
		SqlTransaction trans = null;
		try
		{
			conn = DBUtil.OpenUserDBConnection();
			trans = conn.BeginTransaction();
			int nGuildNameCount = UserDac.GuildNameCount_x(conn, trans, m_sGuildName);
			if (nGuildNameCount > 0)
			{
				throw new CommandHandleException(105, "해당 길드이름이 이미 존재합니다.", null, bLoggingEnabled: false);
			}
			if (UserDac.AddGuildName(conn, trans, m_sGuildName) != 0)
			{
				throw new CommandHandleException(1, "길드이름 추가 실패.");
			}
			SFDBUtil.Commit(ref trans);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Rollback(ref trans);
			SFDBUtil.Close(ref conn);
		}
	}

	protected override void OnWork_Success(SFWork work)
	{
		base.OnWork_Success(work);
		int nRequiredDia = 0;
		try
		{
			if (m_myHero.guildMember != null)
			{
				throw new CommandHandleException(101, "이미 길드멤버입니다.");
			}
			if (m_myHero.level < Resource.instance.guildRequiredHeroLevel)
			{
				throw new CommandHandleException(102, "레벨이 부족합니다.");
			}
			if (m_myHero.vipLevel.level < Resource.instance.guildCreationRequiredVipLevel)
			{
				throw new CommandHandleException(103, "VIP 레벨이 부족합니다.");
			}
			nRequiredDia = Resource.instance.guildCreationRequiredDia;
			if (m_myHero.dia < nRequiredDia)
			{
				throw new CommandHandleException(104, "다이아가 부족합니다.");
			}
			if (m_myHero.GetGuildJoinWaitTime(m_currentTime) > 0f)
			{
				throw new CommandHandleException(107, "아직 재가입시간이 남았습니다.");
			}
		}
		catch (Exception)
		{
			DeleteGuildName();
			throw;
		}
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		m_guild = new Guild(m_sGuildName);
		m_guild.Create(m_myHero, m_currentTime);
		SaveToDB();
		UpdateGuildName();
		SaveToGameLogDB();
		GuildCreateResponseBody resBody = new GuildCreateResponseBody();
		resBody.guild = m_guild.ToPDGuild(m_currentTime);
		resBody.guildMemberGrade = m_myHero.guildMember.grade.id;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.ownDia = m_myHero.dia;
		resBody.unOnwDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddGuild(m_guild, m_currentTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SetGuild(m_myHero.id, m_myHero.guildMember.guild.id, m_myHero.guildMember.grade.id));
		dbWork.Schedule();
	}

	private void UpdateGuildName()
	{
		SFSqlStandaloneWork dbWork = SqlStandaloneWorkUtil.CreateUserDBWork();
		dbWork.AddSqlCommand(UserDac.CSC_UpdateGuildName_GuildInfo(m_sGuildName, m_guild.id, m_myAccount.virtualGameServerId));
		dbWork.Schedule();
	}

	private void DeleteGuildName()
	{
		SFSqlStandaloneWork dbWork = SqlStandaloneWorkUtil.CreateUserDBWork();
		dbWork.AddSqlCommand(UserDac.CSC_DeleteGuildName(m_sGuildName));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildCreationLog(Guid.NewGuid(), m_guild.id, m_myHero.id, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
