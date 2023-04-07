using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildBlessingBuffStartCommandHandler : InGameCommandHandler<GuildBlessingBuffStartCommandBody, GuildBlessingBuffStartResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_OverflowedActivationCount = 103;

	public const short kResult_RunningBlessingBuff = 104;

	public const short kResult_UnableInteractionPositionWithTargetNPC = 105;

	public const short kResult_NotEnoughDia = 106;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nBuffId = m_body.buffId;
		if (nBuffId <= 0)
		{
			throw new CommandHandleException(1, "버프ID가 유효하지 않습니다.");
		}
		if (!(m_myHero.currentPlace is GuildTerritoryInstance))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어 있지 않습니다.");
		}
		if (!m_myGuildMember.grade.guildBlessingBuffEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		if (m_myGuild.blessingBuffStartTime.Date == currentDate)
		{
			throw new CommandHandleException(103, "오늘은 더 이상 축복버프를 시작할 수 없습니다.");
		}
		if (m_myGuild.isBlessingBuffRunning)
		{
			throw new CommandHandleException(104, "축복버프가 진행중입니다.");
		}
		GuildTerritoryNpc blessingGuildTerrtoryNpc = Resource.instance.guildBlessingGuildTerritoryNpc;
		if (!blessingGuildTerrtoryNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(105, "축복NPC와 상호작용할 수 없는 위치입니다.");
		}
		GuildBlessingBuff guildBlessingBuff = Resource.instance.GetGuildBlessingBuff(nBuffId);
		if (guildBlessingBuff == null)
		{
			throw new CommandHandleException(1, "길드축복버프가 존재하지 않습니다. nBuffId = " + nBuffId);
		}
		int nRequiredDia = guildBlessingBuff.dia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(106, "다이아가 부족합니다.");
		}
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		m_myGuild.StartBlessingBuff(guildBlessingBuff, m_currentTime);
		SaveToDB();
		SaveToLogDB();
		GuildBlessingBuffStartResponseBody resBody = new GuildBlessingBuffStartResponseBody();
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateGuildWork(m_myGuild.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_BlessingBuff(m_myGuild.id, m_myGuild.blessingBuffStartTime, m_myGuild.blessingBuff.id));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildBlessingBuffBuyLog(Guid.NewGuid(), m_myGuild.id, m_myHero.id, m_myGuildMember.grade.id, m_myGuild.blessingBuff.id, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
