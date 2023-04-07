using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceInstallCommandHandler : InGameCommandHandler<WingMemoryPieceInstallCommandBody, WingMemoryPieceInstallResponseBody>
{
	private class HeroWingMemoryPieceInstallationDetailLog
	{
		public int slotIndex;

		public int oldAttrValue;

		public int attrValue;

		public HeroWingMemoryPieceInstallationDetailLog(int nSlotIndex, int nOldAttrValue, int nAttrValue)
		{
			slotIndex = nSlotIndex;
			oldAttrValue = nOldAttrValue;
			attrValue = nAttrValue;
		}
	}

	public const short kResult_AllInstalledWing = 101;

	public const short kResult_NotEnoughItem = 102;

	public const short kResult_NotEnoughHeroLevel = 103;

	private HeroWing m_heroWing;

	private WingMemoryPieceStep m_wingMemoryPieceStep;

	private WingMemoryPieceType m_wingMemoryPieceType;

	private List<HeroWingMemoryPieceSlot> m_heroWingMemoryPieceSlots;

	private int m_nUsedItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<HeroWingMemoryPieceInstallationDetailLog> m_detailLogs = new List<HeroWingMemoryPieceInstallationDetailLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nWingId = m_body.wingId;
		int nWingMemoryPieceType = m_body.wingMemoryPieceType;
		if (nWingId <= 0)
		{
			throw new CommandHandleException(1, "날개ID가 유효하지 않습니다. nWingId = " + nWingId);
		}
		if (nWingMemoryPieceType <= 0)
		{
			throw new CommandHandleException(1, "날개기억조각타입이 유효하지 않습니다. nWingMemoryPieceType = " + nWingMemoryPieceType);
		}
		if (m_myHero.level < Resource.instance.wingMemoryPieceInstallationRequiredHeroLevel)
		{
			throw new CommandHandleException(103, "레벨이 부족합니다.");
		}
		m_heroWing = m_myHero.GetWing(nWingId);
		if (m_heroWing == null)
		{
			throw new CommandHandleException(1, "날개가 존재하지 않습니다. nWingId = " + nWingId);
		}
		Wing wing = m_heroWing.wing;
		if (!wing.memoryPieceInstallationEnabled)
		{
			throw new CommandHandleException(1, "날개에 기억조각을 장착할 수 없습니다.");
		}
		if (m_heroWing.isMemoryPieceLastStep && m_heroWing.isMemoryPieceSlotAllInstalled)
		{
			throw new CommandHandleException(101, "기억조각이 전부 장착된 날개입니다.");
		}
		m_wingMemoryPieceStep = wing.GetMemoryPieceStep(m_heroWing.memoryPieceStep);
		m_wingMemoryPieceType = Resource.instance.GetWingMemoryPieceType(nWingMemoryPieceType);
		if (m_wingMemoryPieceType == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 날개기억조각타입 입니다.");
		}
		m_nUsedItemId = m_wingMemoryPieceType.requiredItemId;
		int nRequiredItemCount = m_wingMemoryPieceStep.requiredMemoryPieceCount;
		if (m_myHero.GetItemCount(m_nUsedItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		bool bSuccess = Util.DrawLots(m_wingMemoryPieceType.suceessRate);
		bool bCritical = false;
		if (bSuccess)
		{
			bCritical = Util.DrawLots(m_wingMemoryPieceType.criticalRate);
			if (bCritical)
			{
				CriticalSuccess();
			}
			else
			{
				Success();
			}
		}
		else
		{
			Fail();
		}
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_heroWing.RefreshAttrTotalValues();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		if (!m_heroWing.isMemoryPieceLastStep && m_heroWing.isMemoryPieceSlotAllInstalled)
		{
			m_heroWing.memoryPieceStep++;
		}
		SaveToDB();
		SaveToLogDB(nWingMemoryPieceType, bSuccess, bCritical);
		WingMemoryPieceInstallResponseBody resBody = new WingMemoryPieceInstallResponseBody();
		resBody.memoryPieceStep = m_heroWing.memoryPieceStep;
		resBody.changedWingMemoryPieceSlots = HeroWingMemoryPieceSlot.ToPDHeroWingMemoryPieceSlots(m_heroWingMemoryPieceSlots).ToArray();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void Success()
	{
		WingMemoryPieceSuccessFactorPool pool = m_wingMemoryPieceStep.GetSuccessFactorPool(m_wingMemoryPieceType.type);
		m_heroWingMemoryPieceSlots = m_heroWing.SelectMemoryPieceSlots(1);
		int nSuccessFactor = pool.SelectSuccessFactor();
		foreach (HeroWingMemoryPieceSlot heroSlot in m_heroWingMemoryPieceSlots)
		{
			int nOldAttrValue = heroSlot.accAttrValue;
			int nIncBaseValue = heroSlot.step.attrIncBaseValue;
			heroSlot.IncreaseAccAttrValue(nIncBaseValue * nSuccessFactor);
			m_detailLogs.Add(new HeroWingMemoryPieceInstallationDetailLog(heroSlot.index, nOldAttrValue, heroSlot.accAttrValue));
		}
	}

	private void CriticalSuccess()
	{
		WingMemoryPieceCriticalCountPool pool = m_wingMemoryPieceStep.GetCriticalCountPool(m_wingMemoryPieceType.type);
		int nCriticalCount = pool.SelectCriticalCount();
		m_heroWingMemoryPieceSlots = m_heroWing.SelectMemoryPieceSlots(nCriticalCount);
		int nCriticalFactor = m_wingMemoryPieceType.criticalFactor;
		foreach (HeroWingMemoryPieceSlot heroSlot in m_heroWingMemoryPieceSlots)
		{
			int nOldAttrValue = heroSlot.accAttrValue;
			int nIncBaseValue = heroSlot.step.attrIncBaseValue;
			heroSlot.IncreaseAccAttrValue(nIncBaseValue * nCriticalFactor);
			m_detailLogs.Add(new HeroWingMemoryPieceInstallationDetailLog(heroSlot.index, nOldAttrValue, heroSlot.accAttrValue));
		}
	}

	private void Fail()
	{
		m_heroWingMemoryPieceSlots = m_heroWing.SelectMemoryPieceSlots(1);
		foreach (HeroWingMemoryPieceSlot heroSlot in m_heroWingMemoryPieceSlots)
		{
			int nOldAttrValue = heroSlot.accAttrValue;
			int nDecValue = heroSlot.step.attrDecValue;
			heroSlot.DecreaseAccAttrValue(nDecValue);
			m_detailLogs.Add(new HeroWingMemoryPieceInstallationDetailLog(heroSlot.index, nOldAttrValue, heroSlot.accAttrValue));
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroWing_MemoryPieceStep(m_heroWing.hero.id, m_heroWing.wing.id, m_heroWing.memoryPieceStep));
		foreach (InventorySlot slot2 in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot2));
		}
		foreach (HeroWingMemoryPieceSlot slot in m_heroWingMemoryPieceSlots)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWingMemoryPieceSlot(slot.wing.hero.id, slot.wing.wing.id, slot.index, slot.accAttrValue));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nType, bool bSucceded, bool bCriticalSucceeded)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWingMemoryPieceInstallationLog(logId, m_myHero.id, m_heroWing.wing.id, m_heroWing.memoryPieceStep, nType, bSucceded, bCriticalSucceeded, m_nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			foreach (HeroWingMemoryPieceInstallationDetailLog detailLog in m_detailLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWingMemoryPieceInstallationDetailLog(Guid.NewGuid(), logId, detailLog.slotIndex, detailLog.oldAttrValue, detailLog.attrValue));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
