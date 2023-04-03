using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildSkillLevelUpCommandHandler : InGameCommandHandler<GuildSkillLevelUpCommandBody, GuildSkillLevelUpResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NotOpenedGuildSkillLevel = 102;

	public const short kResult_NotEnoughContributionPoint = 103;

	private GuildMember m_myGuildMember;

	private HeroGuildSkill m_heroGuildSkill;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSkillId = m_body.skillId;
		if (nSkillId <= 0)
		{
			throw new CommandHandleException(1, "스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		Guild myGuild = m_myGuildMember.guild;
		GuildSkill guildSkill = Resource.instance.GetGuildSkill(nSkillId);
		if (guildSkill == null)
		{
			throw new CommandHandleException(1, "길드스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
		}
		int nLaboratoryLevel = myGuild.laboratory.level.level;
		m_heroGuildSkill = m_myHero.GetGuildSkill(guildSkill.id);
		if (m_heroGuildSkill != null && m_heroGuildSkill.isMaxLevel)
		{
			throw new CommandHandleException(1, "이미 최대레벨입니다.");
		}
		int nOldLevel = ((m_heroGuildSkill != null) ? m_heroGuildSkill.level : 0);
		int nTargetLevel = nOldLevel + 1;
		GuildSkillLevel targetSkillLevel = guildSkill.GetLevel(nTargetLevel);
		if (targetSkillLevel == null)
		{
			throw new CommandHandleException(1, "길드스킬레벨이 존재하지 않습니다.");
		}
		if (nLaboratoryLevel < targetSkillLevel.requiredLaboratoryLevel)
		{
			throw new CommandHandleException(102, "개방되지 않은 길드스킬레벨입니다.");
		}
		int nRequiredGuildContributionPoint = targetSkillLevel.requiredGuildContributionPoint;
		if (m_myHero.guildContributionPoint < nRequiredGuildContributionPoint)
		{
			throw new CommandHandleException(103, "공헌도가 부족합니다.");
		}
		m_myHero.UseGuildContributionPoint(nRequiredGuildContributionPoint);
		if (m_heroGuildSkill == null)
		{
			m_heroGuildSkill = new HeroGuildSkill(m_myHero, guildSkill);
			m_myHero.AddGuildSkill(m_heroGuildSkill);
		}
		m_heroGuildSkill.level = nTargetLevel;
		m_heroGuildSkill.RefreshRealLevel();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToGameLogDB(nOldLevel, nRequiredGuildContributionPoint);
		GuildSkillLevelUpResponseBody resBody = new GuildSkillLevelUpResponseBody();
		resBody.level = m_heroGuildSkill.level;
		resBody.contributionPoint = m_myHero.guildContributionPoint;
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_GuildContributionPoint(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateGuildSkillLevel(m_heroGuildSkill.hero.id, m_heroGuildSkill.skill.id, m_heroGuildSkill.level));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nOldLevel, int nUsedGuildContributionPoint)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroGuildSkillLevelUpLog(Guid.NewGuid(), m_myHero.id, m_myGuildMember.guild.id, m_heroGuildSkill.skill.id, nOldLevel, m_heroGuildSkill.level, nUsedGuildContributionPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
