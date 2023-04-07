using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class MountAttrPotionUseCommandHandler : InGameCommandHandler<MountAttrPotionUseCommandBody, MountAttrPotionUseResponseBody>
{
	public const short kResult_OverflowedUseCount = 101;

	public const short kResult_NotEnoughItem = 102;

	private HeroMount m_heroMount;

	private int m_nUsedItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUseditemUnOwnCount;

	private InventorySlot m_changedInventorySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMountId = m_body.mountId;
		if (nMountId <= 0)
		{
			throw new CommandHandleException(1, "탈것ID가 유효하지 않습니다. nMountId = " + nMountId);
		}
		m_heroMount = m_myHero.GetMount(nMountId);
		if (m_heroMount == null)
		{
			throw new CommandHandleException(1, "영웅탈것이 존재하지 않습니다. nMountId = " + nMountId);
		}
		Mount mount = m_heroMount.mount;
		MountLevelMaster mountLevelMaster = mount.GetLevel(m_heroMount.level).levelMaster;
		MountQuality mountQuality = mount.GetQuality(mountLevelMaster.qualityMaster.quality);
		if (m_heroMount.potionAttrCount >= mountQuality.potionAttrMaxCount)
		{
			throw new CommandHandleException(101, "사용횟수가 최대횟수를 넘어갑니다.");
		}
		m_nUsedItemId = Resource.instance.mountPotionAttrItemId;
		if (m_myHero.GetItemCount(m_nUsedItemId) < 1)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUseditemUnOwnCount);
		m_changedInventorySlot = changedInventorySlots[0];
		m_heroMount.potionAttrCount++;
		m_heroMount.RefreshAttrTotalValues();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB();
		MountAttrPotionUseResponseBody resBody = new MountAttrPotionUseResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.potionAttrCount = m_heroMount.potionAttrCount;
		resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMount_PotionAttr(m_heroMount.hero.id, m_heroMount.mount.id, m_heroMount.potionAttrCount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMountPotionAttrLog(Guid.NewGuid(), m_myHero.id, m_heroMount.mount.id, m_heroMount.potionAttrCount, m_nUsedItemId, m_nUsedItemOwnCount, m_nUseditemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
