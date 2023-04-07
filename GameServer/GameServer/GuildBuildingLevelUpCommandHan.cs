using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildBuildingLevelUpCommandHandler : InGameCommandHandler<GuildBuildingLevelUpCommandBody, GuildBuildingLevelUpResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_MaxLevel = 103;

	public const short kResult_NotEnounghGuildLevel = 104;

	public const short kResult_NotEnounghGuildBuildingPoint = 105;

	public const short kResult_NotEnounghGuildFund = 106;

	private Guild m_myGuild;

	private GuildMember m_myGuildMember;

	private GuildBuildingInstance m_targetGuildBuildingInst;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nBuildingId = m_body.buildingId;
		if (nBuildingId <= 0)
		{
			throw new CommandHandleException(1, "건물ID가 유효하지 않습니다. nBuildingId = " + nBuildingId);
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		if (!m_myGuildMember.grade.buildingLevelUpEnabled)
		{
			throw new CommandHandleException(102, "권한이 없습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_targetGuildBuildingInst = m_myGuild.GetBuildingInstance(nBuildingId);
		if (m_targetGuildBuildingInst == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 길드건물입니다.");
		}
		GuildBuildingLevel buildingLevel = m_targetGuildBuildingInst.level;
		if (buildingLevel.isMaxLevel)
		{
			throw new CommandHandleException(103, "이미 최대레벨입니다.");
		}
		if (!m_targetGuildBuildingInst.isLobby && buildingLevel.level >= m_myGuild.level)
		{
			throw new CommandHandleException(104, "길드 레벨이 부족합니다.");
		}
		if (buildingLevel.nextLevelUpGuildBuildingPoint > m_myGuild.buildingPoint)
		{
			throw new CommandHandleException(105, "길드건설도가 부족합니다.");
		}
		if (buildingLevel.nextLevelUpGuildFund > m_myGuild.fund)
		{
			throw new CommandHandleException(106, "길드자금이 부족합니다.");
		}
		int nNextLevelUpGuildFund = buildingLevel.nextLevelUpGuildFund;
		m_myGuild.UseFund(nNextLevelUpGuildFund, m_myHero.id);
		GuildBuildingLevel nextBuildingLevel = m_targetGuildBuildingInst.building.GetLevel(buildingLevel.level + 1);
		m_targetGuildBuildingInst.level = nextBuildingLevel;
		if (m_targetGuildBuildingInst.isLaboratory)
		{
			foreach (GuildMember member in m_myGuild.members.Values)
			{
				if (member.isLoggedIn)
				{
					HeroSynchronizer.Exec(member.hero, new SFAction<Hero>(RefreshGuildSkillRealLevels, member.hero));
				}
			}
		}
		ServerEvent.SendGuildBuildingLevelUp(m_myGuild.GetClientPeers(m_myHero.id), m_targetGuildBuildingInst.building.id, m_targetGuildBuildingInst.level.level);
		SaveToDB();
		SaveToGameLogDB(buildingLevel.level, nNextLevelUpGuildFund);
		GuildBuildingLevelUpResponseBody resBody = new GuildBuildingLevelUpResponseBody();
		resBody.level = m_targetGuildBuildingInst.level.level;
		resBody.fund = m_myGuild.fund;
		SendResponseOK(resBody);
	}

	private void RefreshGuildSkillRealLevels(Hero hero)
	{
		if (hero.RefreshGuildSkillRealLevels())
		{
			hero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendMaxHpChanged(hero.account.peer, hero.realMaxHP, hero.hp);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_myGuild.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Fund(m_myGuild.id, m_myGuild.fund));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuildBuilding_Level(m_targetGuildBuildingInst.guild.id, m_targetGuildBuildingInst.building.id, m_targetGuildBuildingInst.level.level));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nOldLevel, int nUsedGuildFund)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildBuildingLevelUpLog(Guid.NewGuid(), m_myGuild.id, m_myHero.id, m_targetGuildBuildingInst.building.id, nOldLevel, m_targetGuildBuildingInst.level.level, m_myGuild.level, m_myGuild.buildingPoint, nUsedGuildFund, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
