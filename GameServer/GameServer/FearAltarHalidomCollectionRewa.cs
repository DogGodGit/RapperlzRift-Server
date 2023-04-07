using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class FearAltarHalidomCollectionReward
{
	private FearAltar m_fearAltar;

	private int m_nNo;

	private int m_nCollectionCount;

	private ItemReward m_itemReward;

	public FearAltar fearAltar => m_fearAltar;

	public int no => m_nNo;

	public int collectionCount => m_nCollectionCount;

	public ItemReward itemReward => m_itemReward;

	public FearAltarHalidomCollectionReward(FearAltar fearAltar)
	{
		m_fearAltar = fearAltar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		m_nCollectionCount = Convert.ToInt32(dr["collectionCount"]);
		if (m_nCollectionCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "수집수량이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nCollectionCount = " + m_nCollectionCount);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
public class FearAltarHalidomCollectionRewardReceiveCommandHandler : InGameCommandHandler<FearAltarHalidomCollectionRewardReceiveCommandBody, FearAltarHalidomCollectionRewardReceiveResponseBody>
{
	public const short kResult_NotEnoughHalidomCount = 101;

	public const short kResult_NotEnoughInventorySlot = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private PDItemBooty m_booty;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private DateValuePair<int> m_weeklyFearAltarHalidomCollectionRewardNo;

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
		int nRewardNo = m_body.rewardNo;
		FearAltar fearAltar = Resource.instance.fearAltar;
		FearAltarHalidomCollectionReward halidomCollectionReward = fearAltar.GetHalidomCollectionReward(nRewardNo);
		if (halidomCollectionReward == null)
		{
			throw new CommandHandleException(1, "원소ID가 유효하지 않습니다. nRewardNo = " + nRewardNo);
		}
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_myHero.fearAltarHalidomCount < halidomCollectionReward.collectionCount)
		{
			throw new CommandHandleException(101, "성물의 수집갯수가 부족합니다.");
		}
		m_myHero.RefreshWeeklyFearAltarHalidomCollectionRewardNo(currentDate);
		m_weeklyFearAltarHalidomCollectionRewardNo = m_myHero.weeklyFearAltarHalidomCollectionRewardNo;
		int nFearAltarHalidomCollectionRewardNo = m_weeklyFearAltarHalidomCollectionRewardNo.value;
		if (nRewardNo <= nFearAltarHalidomCollectionRewardNo)
		{
			throw new CommandHandleException(1, "이미 해당수집보상을 받았습니다.");
		}
		if (nRewardNo != nFearAltarHalidomCollectionRewardNo + 1)
		{
			throw new CommandHandleException(1, "이전 보상을 받지 않았습니다.");
		}
		ItemReward itemReward = halidomCollectionReward.itemReward;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			bool bOwned = itemReward.owned;
			int nCount = itemReward.count;
			if (nCount > m_myHero.GetInventoryAvailableSpace(item, bOwned))
			{
				throw new CommandHandleException(102, "인벤토리슬롯이 부족합니다.");
			}
			m_booty = new PDItemBooty();
			m_booty.id = item.id;
			m_booty.owned = bOwned;
			m_booty.count = nCount;
			m_myHero.AddItem(item, bOwned, nCount, m_changedInventorySlots);
		}
		m_weeklyFearAltarHalidomCollectionRewardNo.value++;
		SaveToDB();
		SaveToDB_Log();
		FearAltarHalidomCollectionRewardReceiveResponseBody resBody = new FearAltarHalidomCollectionRewardReceiveResponseBody();
		resBody.weekStartDate = (DateTime)DateTimeUtil.GetWeekStartDate(currentDate);
		resBody.booty = m_booty;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FearAltarHalidomCollectionReward(m_myHero.id, m_weeklyFearAltarHalidomCollectionRewardNo.date, m_weeklyFearAltarHalidomCollectionRewardNo.value));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroFearAltarHalidomCollectionRewardLog(Guid.NewGuid(), m_myHero.id, m_weeklyFearAltarHalidomCollectionRewardNo.value, m_myHero.fearAltarHalidomCount, m_booty.id, m_booty.owned, m_booty.count, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex);
		}
	}
}
