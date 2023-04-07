using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class RankPassiveSkillLevelUpCommandHandler : InGameCommandHandler<RankPassiveSkillLevelUpCommandBody, RankPassiveSkillLevelUpResponseBody>
{
	public const short kResult_MaxLevel = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kResult_NotEnoughSpiritStone = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nTargetSkillId;

	private int m_nOldLevel;

	private int m_nLevel;

	private long m_lnNextLevelUpRequiredGold;

	private int m_nNextLevelUpRequiredSpiritStone;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nTargetSkillId = m_body.targetSkillId;
		HeroRankPassiveSkill heroSkill = m_myHero.GetRankPassiveSkill(m_nTargetSkillId);
		if (heroSkill == null)
		{
			throw new CommandHandleException(1, "목표스킬ID가 유효하지 않습니다. m_nTargetSkillId = " + m_nTargetSkillId);
		}
		if (heroSkill.isMaxLevel)
		{
			throw new CommandHandleException(101, "이미 스킬레벨이 최대치입니다.");
		}
		RankPassiveSkillLevel heroSkillLevel = heroSkill.skillLevel;
		m_nOldLevel = heroSkill.level;
		m_lnNextLevelUpRequiredGold = heroSkillLevel.nextLevelUpRequiredGold;
		if (m_lnNextLevelUpRequiredGold > m_myHero.gold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		m_nNextLevelUpRequiredSpiritStone = heroSkillLevel.nextLevelUpRequiredSpiritStone;
		if (m_myHero.spiritStone < m_nNextLevelUpRequiredSpiritStone)
		{
			throw new CommandHandleException(103, "정령석이 부족합니다.");
		}
		m_myHero.UseGold(m_lnNextLevelUpRequiredGold);
		m_myHero.UseSpiritStone(m_nNextLevelUpRequiredSpiritStone);
		heroSkill.level++;
		m_nLevel = heroSkill.level;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToDB_Log();
		RankPassiveSkillLevelUpResponseBody resBody = new RankPassiveSkillLevelUpResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.gold = m_myHero.gold;
		resBody.spiritStone = m_myHero.spiritStone;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SpiritStone(m_myHero.id, m_myHero.spiritStone));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroRankPassiveSkill(m_myHero.id, m_nTargetSkillId, m_nLevel));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRankPassiveSkillLevelUpLog(Guid.NewGuid(), m_myHero.id, m_nTargetSkillId, m_nOldLevel, m_nLevel, m_lnNextLevelUpRequiredGold, m_nNextLevelUpRequiredSpiritStone, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}
}
