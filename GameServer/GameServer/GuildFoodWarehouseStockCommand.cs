using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildFoodWarehouseStockCommandHandler : InGameCommandHandler<GuildFoodWarehouseStockCommandBody, GuildFoodWarehouseStockResponseBody>
{
	public const short kResult_DailyStockCountIsMax = 101;

	public const short kResult_NotEnoughItem = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	private int m_nItemId;

	private Guild m_guild;

	private int m_nOldFoodWareshouseLevel;

	private int m_nOldFoodWareshouseExp;

	private DateValuePair<int> m_dailyStockCount;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_changedInventorySlot;

	private long m_lnRewardExp;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nItemId = m_body.itemId;
		if (m_nItemId <= 0)
		{
			throw new CommandHandleException(1, "아이템ID가 유효하지 않습니다. m_nItemId = " + m_nItemId);
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		GuildFoodWarehouse warehouse = Resource.instance.guildFoodWarehouse;
		Item item = Resource.instance.GetItem(m_nItemId);
		if (item == null)
		{
			throw new CommandHandleException(1, "아이템이 존재하지 않습니다. m_nItemId = " + m_nItemId);
		}
		if (item.type.id != warehouse.levelUpRequiredItemType)
		{
			throw new CommandHandleException(1, "필요한 아이템이 아닙니다. m_nItemId = " + m_nItemId);
		}
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		m_guild = currentPlace.guild;
		GuildTerritoryNpc npc = warehouse.npc;
		if (!npc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_myHero.RefreshDailyGuildFoodWarehouseStockCount(m_currentDate);
		m_dailyStockCount = m_myHero.dailyGuildFoodWarehouseStockCount;
		if (m_dailyStockCount.value >= warehouse.limitCount)
		{
			throw new CommandHandleException(101, "금일 납부횟수가 최대입니다.");
		}
		if (m_myHero.GetItemCount(m_nItemId) < 1)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다. m_nItemId = " + m_nItemId);
		}
		m_dailyStockCount.value++;
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(m_nItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_changedInventorySlot = changedInventorySlots[0];
		m_nOldFoodWareshouseLevel = m_guild.foodWarehouseLevel;
		m_nOldFoodWareshouseExp = m_guild.foodWarehouseExp;
		int nExp = item.value1;
		m_guild.AddFoodWarehouseExp(nExp);
		GuildFoodWareshouseStockReward stockReward = warehouse.GetStockReward(m_nItemId, m_myHero.level);
		if (stockReward != null)
		{
			m_lnRewardExp = stockReward.expRewardValue;
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
		}
		m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		SaveToDB();
		SaveToGameLogDB();
		GuildFoodWarehouseStockResponseBody resBody = new GuildFoodWarehouseStockResponseBody();
		resBody.addedFoodWarehouseExp = nExp;
		resBody.foodWarehouseLevel = m_guild.foodWarehouseLevel;
		resBody.foodWarehouseExp = m_guild.foodWarehouseExp;
		resBody.acquiredExp = m_lnRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
		resBody.date = (DateTime)m_dailyStockCount.date;
		resBody.dailyStockCount = m_dailyStockCount.value;
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(24, m_currentDate);
		if (m_dailyStockCount.value >= warehouse.limitCount)
		{
			m_guild.CompleteGuildDailyObjective(m_currentDate, 4, m_myHero.guildMember);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateGuild_FoodWarehouse(m_guild));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildFoodWarehouseStockCount(m_myHero.id, m_dailyStockCount.date, m_dailyStockCount.value));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildFoodWarehouseStockLog(Guid.NewGuid(), m_guild.id, m_myHero.id, m_nItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_nOldFoodWareshouseLevel, m_nOldFoodWareshouseExp, m_guild.foodWarehouseLevel, m_guild.foodWarehouseExp, m_lnRewardExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
