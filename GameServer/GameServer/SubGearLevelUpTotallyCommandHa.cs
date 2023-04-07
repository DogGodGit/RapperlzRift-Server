using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class SubGearLevelUpTotallyCommandHandler : InGameCommandHandler<SubGearLevelUpTotallyCommandBody, SubGearLevelUpTotallyResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HeroSubGear m_heroSubGear;

	private long m_lnTotalUsedGold;

	private int m_nOldLevel;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSubGearId = m_body.subGearId;
		if (nSubGearId <= 0)
		{
			throw new CommandHandleException(1, "보조장비 ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
		}
		m_heroSubGear = m_myHero.GetSubGear(nSubGearId);
		if (m_heroSubGear == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 보조장비 입니다. nSubGearId = " + nSubGearId);
		}
		if (!m_heroSubGear.equipped)
		{
			throw new CommandHandleException(1, "장착되지 않은 영웅보조장비입니다.");
		}
		m_nOldLevel = m_heroSubGear.level;
		while (m_heroSubGear.level < m_myHero.level && !m_heroSubGear.isLastLevel && m_heroSubGear.isLastQualityOfCurrentLevel && !m_heroSubGear.isLastLevelOfCurrentGrade)
		{
			int nRequiredGold = m_heroSubGear.subGearLevel.nextLevelUpRequiredGold;
			if (m_myHero.gold < nRequiredGold)
			{
				break;
			}
			m_myHero.UseGold(nRequiredGold);
			SubGearLevel nextLevel = m_heroSubGear.subGear.GetLevel(m_heroSubGear.level + 1);
			m_heroSubGear.subGearLevel = nextLevel;
			m_heroSubGear.quality = nextLevel.firstQuality.quality;
			m_lnTotalUsedGold += nRequiredGold;
		}
		if (m_nOldLevel != m_heroSubGear.level)
		{
			m_heroSubGear.RefreshAttrTotalValues();
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			SaveToDB();
		}
		SubGearLevelUpTotallyResponseBody resBody = new SubGearLevelUpTotallyResponseBody();
		resBody.subGearLevel = m_heroSubGear.level;
		resBody.subGearQuality = m_heroSubGear.quality;
		resBody.gold = m_myHero.gold;
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubGear_LevelUp(m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_heroSubGear.level, m_heroSubGear.quality));
		dbWork.Schedule();
		SaveToDB_AddSubGearLevelUpLog();
	}

	private void SaveToDB_AddSubGearLevelUpLog()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSubGearLevelUpLog(Guid.NewGuid(), m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_nOldLevel, m_heroSubGear.level, m_lnTotalUsedGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
