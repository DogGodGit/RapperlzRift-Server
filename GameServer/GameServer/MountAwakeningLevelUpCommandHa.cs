using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class MountAwakeningLevelUpCommandHandler : InGameCommandHandler<MountAwakeningLevelUpCommandBody, MountAwakeningLevelUpResponseBody>
{
	public const short kResult_LastAwakeningLevel = 101;

	public const short kResult_NotEnoughHeroLevel = 102;

	public const short kResult_NotEnoughItem = 103;

	private HeroMount m_heroMount;

	private Item m_usedItem;

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
		if (m_heroMount.isMaxAwakeningLevel)
		{
			throw new CommandHandleException(101, "마지막 각성레벨입니다.");
		}
		if (m_myHero.level < Resource.instance.mountAwakeningRequiredHeroLevel)
		{
			throw new CommandHandleException(102, "영웅레벨이 부족합니다.");
		}
		m_usedItem = Resource.instance.mountAwakeningItem;
		if (m_myHero.GetItemCount(m_usedItem.id) < 1)
		{
			throw new CommandHandleException(103, "아이템이 부족합니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(m_usedItem.id, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUseditemUnOwnCount);
		m_changedInventorySlot = changedInventorySlots[0];
		int nOldAwakeningLevel = m_heroMount.awakeningLevel;
		int nOldAwakeningExp = m_heroMount.awakeningExp;
		m_heroMount.AddAwakeningExp(m_usedItem.value1);
		if (nOldAwakeningLevel != m_heroMount.awakeningLevel)
		{
			m_heroMount.RefreshAttrTotalValues();
			if (!m_heroMount.isEquipped)
			{
				m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			}
		}
		SaveToDB();
		SaveToLogDB(nOldAwakeningLevel, nOldAwakeningExp);
		MountAwakeningLevelUpResponseBody resBody = new MountAwakeningLevelUpResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.awakningLevel = m_heroMount.awakeningLevel;
		resBody.awakningExp = m_heroMount.awakeningExp;
		resBody.changedInvetorySlot = m_changedInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMount_AwakeningLevel(m_heroMount.hero.id, m_heroMount.mount.id, m_heroMount.awakeningLevel, m_heroMount.awakeningExp));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldAwakeningLevel, int nOldAwakeningExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMountAwakeningLog(Guid.NewGuid(), m_myHero.id, m_heroMount.mount.id, m_usedItem.id, m_nUsedItemOwnCount, m_nUseditemUnOwnCount, nOldAwakeningLevel, m_heroMount.awakeningLevel, nOldAwakeningExp, m_heroMount.awakeningExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
