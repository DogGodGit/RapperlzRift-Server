using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class GoldDungeonSweepCommandHandler : InGameCommandHandler<GoldDungeonSweepCommandBody, GoldDungeonSweepResponseBody>
{
    public const short kResult_Dead = 101;

    public const short kResult_NotEnoughSweepItem = 103;

    public const short kResult_NotEnoughStamina = 104;

    public const short kResult_EnterCountOverflowed = 105;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private GoldDungeonDifficulty m_difficulty;

    private bool m_bIsFreeSweep;

    private DateValuePair<int> m_freeSweepDailyCount;

    private long m_lnRewardGold;

    private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

    protected override void HandleInGameCommand()
    {
        if (!(m_myHero.currentPlace is ContinentInstance))
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nDifficulty = m_body.difficulty;
        GoldDungeon goldDungeon = Resource.instance.goldDungeon;
        m_difficulty = goldDungeon.GetDifficulty(nDifficulty);
        if (m_difficulty == null)
        {
            throw new CommandHandleException(1, "해당 난이도는 존재하지 않습니다. nDifficulty = " + nDifficulty);
        }
        if (!m_myHero.IsClearedGoldDungeonDifficulty(nDifficulty))
        {
            throw new CommandHandleException(1, "영웅이 해당 난이도 소탕에 필요한 조건을 만족하지 못했습니다. nDifficulty = " + nDifficulty);
        }
        if (m_myHero.isDead)
        {
            throw new CommandHandleException(101, "영웅이 죽은상태 입니다.");
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
        int nRequiredStamina = goldDungeon.requiredStamina;
        if (m_myHero.stamina < nRequiredStamina)
        {
            throw new CommandHandleException(104, "스태미너가 부족합니다.");
        }
        if (m_myHero.GetGoldDungeonAvailableEnterCount(currentDate) <= 0)
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
        m_myHero.RefreshDailyGoldDungeonPlayCount(currentDate);
        DateValuePair<int> dailyGoldDungeonPlayCount = hero.dailyGoldDungeonPlayCount;
        dailyGoldDungeonPlayCount.value++;
        SweepReward();
        SaveToDB(nUsedOwnCount, nUsedUnOwnCount, dailyGoldDungeonPlayCount.value);
        GoldDungeonSweepResponseBody resBody = new GoldDungeonSweepResponseBody();
        resBody.date = currentDate;
        resBody.stamina = m_myHero.stamina;
        resBody.playCount = dailyGoldDungeonPlayCount.value;
        resBody.freeSweepDailyCount = m_freeSweepDailyCount.value;
        resBody.changedInventorySlot = m_changedInventorySlots.Count > 0 ? m_changedInventorySlots[0].ToPDInventorySlot() : null;
        resBody.rewardGold = m_lnRewardGold;
        resBody.gold = m_myHero.gold;
        resBody.maxGold = m_myHero.maxGold;
        SendResponseOK(resBody);
    }

    private void SweepReward()
    {
        m_lnRewardGold = m_difficulty.goldReward.value;
        m_myHero.AddGold(m_lnRewardGold);
    }

    private void SaveToDB(int nUsedOwnSweepItemCount, int nUsedUnOwnSweepItemCount, int nEnterCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateGoldDungeonPlay(m_myHero.id, m_currentTime.Date, nEnterCount));
        if (m_bIsFreeSweep)
        {
            dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FreeSweepCount(m_myHero.id, m_freeSweepDailyCount.date, m_freeSweepDailyCount.value));
        }
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
        }
        dbWork.Schedule();
        SaveToDB_Log(nUsedOwnSweepItemCount, nUsedUnOwnSweepItemCount);
    }

    private void SaveToDB_Log(int nItemOwnCount, int nItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, Resource.instance.dungeonSweepItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddGoldDungeonPlayLog(logId, Guid.Empty, m_myHero.id, m_difficulty.difficulty, 2, m_lnRewardGold, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
