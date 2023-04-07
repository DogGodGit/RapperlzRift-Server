using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class CreatureReleaseCommandHandler : InGameCommandHandler<CreatureReleaseCommandBody, CreatureReleaseResponseBody>
{
    public const int kResult_NotEnoughInventory = 101;

    private HeroCreature m_heroCreature;

    private ResultItemCollection m_resultItemCollection;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid instanceId = m_body.instanceId;
        if (instanceId == Guid.Empty)
        {
            throw new CommandHandleException(1, "영웅크리처ID가 유효하지 않습니다. instanceId = " + instanceId);
        }
        m_heroCreature = m_myHero.GetCreature(instanceId);
        if (m_heroCreature == null)
        {
            throw new CommandHandleException(1, "영웅크리처가 존재하지 않습니다. instanceId = " + instanceId);
        }
        if (m_heroCreature.participated)
        {
            throw new CommandHandleException(1, "영웅크리처가 이미 출전중입니다. instanceId = " + instanceId);
        }
        if (m_heroCreature.cheered)
        {
            throw new CommandHandleException(1, "영웅크리처가 이미 응원중입니다. instanceId = " + instanceId);
        }
        long lnTotalCreatueExp = m_heroCreature.GetAccumulationExp();
        lnTotalCreatueExp = (long)(lnTotalCreatueExp * Resource.instance.creatureReleaseExpRetrievalRate / 10000f);
        m_resultItemCollection = new ResultItemCollection();
        Item[] creatureFeeds = Resource.instance.creatureFeeds;
        foreach (Item creatureFeed in creatureFeeds)
        {
            int nItemCount = (int)(lnTotalCreatueExp / creatureFeed.value1);
            lnTotalCreatueExp -= creatureFeed.value1 * nItemCount;
            if (nItemCount > 0)
            {
                m_resultItemCollection.AddResultItemCount(creatureFeed, bOwned: true, nItemCount);
            }
        }
        if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
        {
            throw new CommandHandleException(101, "인벤토리가 부족합니다.");
        }
        foreach (ResultItem result in m_resultItemCollection.resultItems)
        {
            m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
        }
        m_myHero.RemoveCreature(m_heroCreature.instanceId);
        SaveToDB();
        SaveToLogDB();
        CreatureReleaseResponseBody resBody = new CreatureReleaseResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_DeleteHeroCreature(m_heroCreature));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        dbWork.Schedule();
    }

    private void SaveToLogDB()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureReleaseLog(m_heroCreature.instanceId, m_myHero.id, m_heroCreature.level, m_heroCreature.exp, m_heroCreature.injectionLevel, m_heroCreature.injectionExp, m_currentTime));
            foreach (ResultItem result in m_resultItemCollection.resultItems)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureReleaseDetailLog(Guid.NewGuid(), m_heroCreature.instanceId, result.item.id, result.owned, result.count));
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
