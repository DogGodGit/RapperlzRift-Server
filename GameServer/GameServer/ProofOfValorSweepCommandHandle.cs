using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ProofOfValorSweepCommandHandler : InGameCommandHandler<ProofOfValorSweepCommandBody, ProofOfValorSweepResponseBody>
{
	public const short kResult_LevelUnderflowed = 101;

	public const short kResult_Dead = 102;

	public const short kResult_NotEnoughSweepItem = 103;

	public const short kResult_NotEnoughStamina = 104;

	public const short kResult_EnterCountOverflowed = 105;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private bool m_bIsFreeSweep;

	private DateValuePair<int> m_freeSweepDailyCount;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private HeroProofOfValorInstance m_heroProofOfValorInst;

	private HeroCreatureCard m_heroCreatureCard;

	private long m_lnRewardExp;

	private int m_nRewardSoulPowder;

	private int m_nSpecialRewardSoulPowder;

	protected override void HandleInGameCommand()
	{
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (!m_myHero.proofOfValorCleared)
		{
			throw new CommandHandleException(1, "영웅이 소탕에 필요한 조건을 만족하지 못했습니다.");
		}
		if (m_myHero.isDead)
		{
			throw new CommandHandleException(102, "영웅이 죽은상태 입니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myHero.RefreshFreeSweepDailyCount(currentDate);
		m_freeSweepDailyCount = m_myHero.freeSweepDailyCount;
		if (m_freeSweepDailyCount.value < Resource.instance.dungeonFreeSweepDailyCount)
		{
			m_bIsFreeSweep = true;
		}
		int nDungeonSweepItemId = Resource.instance.dungeonSweepItemId;
		if (!m_bIsFreeSweep && m_myHero.GetItemCount(nDungeonSweepItemId) == 0)
		{
			throw new CommandHandleException(103, "소탕령이 부족합니다.");
		}
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		int nRequiredStamina = proofOfValor.requiredStamina;
		if (m_myHero.stamina < nRequiredStamina)
		{
			throw new CommandHandleException(104, "스태미너가 부족합니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_myHero.GetProofOfValorAvailableEnterCount(currentTime.Date) <= 0)
		{
			throw new CommandHandleException(105, "입장횟수가 초과되었습니다.");
		}
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		if (!m_bIsFreeSweep)
		{
			m_myHero.UseItem(nDungeonSweepItemId, bFisetUseOwn: true, 1, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
		}
		else
		{
			m_freeSweepDailyCount.value++;
		}
		m_myHero.UseStamina(nRequiredStamina, m_currentTime);
		m_myHero.dailyProofOfValorPlayCount.value++;
		m_heroProofOfValorInst = m_myHero.heroProofOfValorInst;
		m_heroProofOfValorInst.level = m_myHero.level;
		m_heroCreatureCard = m_myHero.IncreaseCreatureCardCount(m_heroProofOfValorInst.creatureCard);
		ProofOfValorReward reward = proofOfValor.GetReward(m_myHero.level);
		m_lnRewardExp = 0L;
		if (reward != null)
		{
			m_lnRewardExp = reward.successExpRewardValue;
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		ProofOfValorBossMonsterArrange bossMonsterArrange = m_heroProofOfValorInst.bossMonsterArrange;
		m_nRewardSoulPowder = bossMonsterArrange.rewardSoulPowder;
		m_nSpecialRewardSoulPowder = 0;
		if (bossMonsterArrange.isSpecial)
		{
			m_nSpecialRewardSoulPowder = bossMonsterArrange.specialRewardSoulPowder;
		}
		m_myHero.AddSoulPowder(m_nRewardSoulPowder + m_nSpecialRewardSoulPowder);
		m_heroProofOfValorInst.status = 5;
		SaveToDB();
		SaveToDB_Log(nUsedOwnCount, nUsedUnOwnCount);
		m_myHero.CreateHeroProofOfValorInstance(m_currentTime, bIsRefreshPaidCount: true);
		ProofOfValorSweepResponseBody resBody = new ProofOfValorSweepResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.stamina = m_myHero.stamina;
		resBody.playCount = m_myHero.dailyProofOfValorPlayCount.value;
		resBody.freeSweepDailyCount = m_freeSweepDailyCount.value;
		resBody.changedInventorySlot = ((m_changedInventorySlots.Count > 0) ? m_changedInventorySlots[0].ToPDInventorySlot() : null);
		resBody.acquiredExp = m_lnRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.soulPowder = m_myHero.soulPowder;
		resBody.changedCreatureCard = m_heroCreatureCard.ToPDHeroCreatureCard();
		resBody.heroProofOfValorInst = m_myHero.heroProofOfValorInst.ToPDHeroProofOfValorInstance();
		resBody.proofOfValorPaidRefreshCount = m_myHero.proofOfValorPaidRefreshCount;
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(9, currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(8);
		m_myHero.ProcessMainQuestForContent(16);
		m_myHero.ProcessSubQuestForContent(16);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.status, m_heroProofOfValorInst.level, 0, m_currentTime));
		if (m_bIsFreeSweep)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FreeSweepCount(m_myHero.id, m_freeSweepDailyCount.date, m_freeSweepDailyCount.value));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(m_heroCreatureCard));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(int nItemOwnCount, int nItemUnOwnCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, Resource.instance.dungeonSweepItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRewardLog(Guid.NewGuid(), m_myHero.id, m_heroProofOfValorInst.id, 4, m_heroCreatureCard.card.id, m_nRewardSoulPowder, m_lnRewardExp, m_nSpecialRewardSoulPowder, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
