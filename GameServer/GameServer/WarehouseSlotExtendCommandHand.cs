using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WarehouseSlotExtendCommandHandler : InGameCommandHandler<WarehouseSlotExtendCommandBody, WarehouseSlotExtendResponseBody>
{
	public const short kResult_OverFlowMaxPaidWarehouseSlotCount = 101;

	public const short kResult_NotEnoughDia = 102;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

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
			throw new CommandHandleException(1, "슬롯개수가 유효하지 않습니다. nExtendSlotCount= " + nExtendSlotCount);
		}
		int nTargetSlotCount = nExtendSlotCount + m_myHero.paidWarehouseSlotCount;
		if (nTargetSlotCount > Resource.instance.lastWarehouseSlotExtendRecipeSlotCount)
		{
			throw new CommandHandleException(101, "슬롯 카운트가 확장슬롯카운트의 최대를 넘어갑니다.");
		}
		int nTotalPriceDia = 0;
		for (int nRecipeSlotCount = m_myHero.paidWarehouseSlotCount + 1; nRecipeSlotCount <= nTargetSlotCount; nRecipeSlotCount++)
		{
			WarehouseSlotExtendRecipe warehouseSlotExtendRecipe = Resource.instance.GetWarehouseSlotExtendRecipe(nRecipeSlotCount);
			nTotalPriceDia += warehouseSlotExtendRecipe.dia;
		}
		if (m_myHero.dia < nTotalPriceDia)
		{
			throw new CommandHandleException(102, "다이아가 부족합니다.");
		}
		int nOldPaidWarehouseSlotCount = m_myHero.paidWarehouseSlotCount;
		m_myHero.CreateWarehouseSlots(nExtendSlotCount);
		m_myHero.paidWarehouseSlotCount += nExtendSlotCount;
		m_myHero.UseDia(nTotalPriceDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		SaveToDB();
		SaveToLogDB(nOldPaidWarehouseSlotCount);
		WarehouseSlotExtendResponseBody resBody = new WarehouseSlotExtendResponseBody();
		resBody.paidWarehouseSlotCount = m_myHero.paidWarehouseSlotCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidWarehouseSlotCount(m_myHero.id, m_myHero.paidWarehouseSlotCount));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldPaidWarehouseSlotCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddWarehouseSlotExtendLog(Guid.NewGuid(), m_myHero.id, nOldPaidWarehouseSlotCount, m_myHero.paidInventorySlotCount, m_nUsedOwnDia, m_nUsedUnOwnDia, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
