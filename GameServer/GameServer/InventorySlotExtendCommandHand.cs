using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class InventorySlotExtendCommandHandler : InGameCommandHandler<InventorySlotExtendCommandBody, InventorySlotExtendResponseBody>
{
	public const short kResult_OverFlowMaxPaidInventorySlotCount = 101;

	public const short kResult_NotEnoughDia = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nExtendSlotCount = m_body.extendSlotCount;
		if (nExtendSlotCount <= 0)
		{
			throw new CommandHandleException(1, "확장할 슬롯 갯수가 유효하지 않습니다. nExtendSlotCount = " + nExtendSlotCount);
		}
		int nTargetSlotCount = nExtendSlotCount + m_myHero.paidInventorySlotCount;
		if (nTargetSlotCount > Resource.instance.lastInventorySlotExtnedRecipe.slotCount)
		{
			throw new CommandHandleException(101, "슬롯 카운트가 확장슬롯카운트의 최대를 넘어갑니다.");
		}
		int nTotalPriceDia = 0;
		for (int nRecipeSlotCount = m_myHero.paidInventorySlotCount + 1; nRecipeSlotCount <= nTargetSlotCount; nRecipeSlotCount++)
		{
			InventorySlotExtendRecipe inventorySlotExtendRecipe = Resource.instance.GetInventorySlotExtendRecipe(nRecipeSlotCount);
			nTotalPriceDia += inventorySlotExtendRecipe.dia;
		}
		if (m_myHero.dia < nTotalPriceDia)
		{
			throw new CommandHandleException(102, "다이아가 부족합니다.");
		}
		int nOldPaidInventorySlotCount = m_myHero.paidInventorySlotCount;
		m_myHero.CreateInventorySlots(nExtendSlotCount);
		m_myHero.paidInventorySlotCount += nExtendSlotCount;
		int nUsedOwnDia = 0;
		int nUsedUnOwnDia = 0;
		m_myHero.UseDia(nTotalPriceDia, m_currentTime, out nUsedOwnDia, out nUsedUnOwnDia);
		SaveToDB(nOldPaidInventorySlotCount, nUsedOwnDia, nUsedUnOwnDia);
		InventorySlotExtendResponseBody resBody = new InventorySlotExtendResponseBody();
		resBody.paidInventorySlotCount = m_myHero.paidInventorySlotCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nOldPaidInventorySlotCount, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_OwnDia(m_myHero.id, m_myHero.ownDia));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidInventorySlotCount(m_myHero.id, m_myHero.paidInventorySlotCount));
		dbWork.Schedule();
		SaveToDB_AddInventorySlotExtendLog(nOldPaidInventorySlotCount, nUsedOwnDia, nUsedUnOwnDia);
	}

	private void SaveToDB_AddInventorySlotExtendLog(int nOldPaidInventorySlotCount, int nUsedOwnDia, int nUsedUnOwnDia)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddInventorySlotExtendLog(Guid.NewGuid(), m_myHero.id, nOldPaidInventorySlotCount, m_myHero.paidInventorySlotCount, nUsedOwnDia, nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
