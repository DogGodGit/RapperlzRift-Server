using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class DistortionScrollUseCommandHandler : InGameCommandHandler<DistortionScrollUseCommandBody, DistortionScrollUseResponseBody>
{
	public const short kResult_NotEnoughItem = 102;

	public const short kResult_DailyUseCountOverflowed = 103;

	public const short kResult_NationWar = 104;

	public const short kResult_NotAvailableHeroState = 105;

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
		int nSlotIndex = m_body.slotIndex;
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (!m_myHero.currentPlace.distortionScrollUseEnabled)
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.nationInst.nationWarInst != null)
		{
			throw new CommandHandleException(104, "국가전이 진행중입니다.");
		}
		if (!m_myHero.isUseableDistortionScroll)
		{
			throw new CommandHandleException(105, "왜곡주문서를 사용할 수 있는 상태가 아닙니다.");
		}
		InventorySlot targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "인벤토리슬롯이 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (targetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "해당 인벤토리슬롯 오브젝트 타입이 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject inventoryItem = (ItemInventoryObject)targetInventorySlot.obj;
		Item item = inventoryItem.item;
		bool bOwned = inventoryItem.owned;
		if (inventoryItem.count < 1)
		{
			throw new CommandHandleException(102, "아이템 수량이 부족합니다.");
		}
		if (item.type.id != 16)
		{
			throw new CommandHandleException(1, "아이템 타입이 왜곡주문서가 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (!item.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "아이템을 사용할 수 있는 레벨이 아닙니다.");
		}
		m_myHero.RefreshDistortionScrollDailyUseCount(currentDate);
		DateValuePair<int> distortionScrollDailyUseCount = m_myHero.distortionScrollDailyUseCount;
		if (distortionScrollDailyUseCount.value >= m_myHero.vipLevel.distortionScrollUseMaxCount)
		{
			throw new CommandHandleException(103, "일일사용횟수가 최대사용횟수를 넘어갑니다.");
		}
		m_myHero.UseItem(nSlotIndex, 1);
		distortionScrollDailyUseCount.value++;
		m_myHero.StartDistortion(m_currentTime, item.value1);
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		if (bOwned)
		{
			nUsedOwnCount = 1;
		}
		else
		{
			nUsedUnOwnCount = 1;
		}
		SaveToDB(targetInventorySlot, item.id, nUsedOwnCount, nUsedUnOwnCount);
		DistortionScrollUseResponseBody resBody = new DistortionScrollUseResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.distortionScrollDailyUseCount = distortionScrollDailyUseCount.value;
		resBody.distortionRemainingTime = m_myHero.GetRemainingDistortionTime(m_currentTime);
		resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB(InventorySlot targetInventorySlot, int nItemId, int nItemOwnCount, int nItemUnOwnCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_UseDistortionScroll(m_myHero.id, m_myHero.distortionScrollDailyUseCount.date, m_myHero.distortionScrollDailyUseCount.value, m_myHero.distortionScrollStartTime.Value, m_myHero.distortionScrollDuration));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(targetInventorySlot));
		dbWork.Schedule();
		SaveToDB_AddItemUseLog(nItemId, nItemOwnCount, nItemUnOwnCount);
	}

	private void SaveToDB_AddItemUseLog(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
