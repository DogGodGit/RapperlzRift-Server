using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CostumeEffectApplyCommandHandler : InGameCommandHandler<CostumeEffectApplyCommandBody, CostumeEffectApplyResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private HeroCostume m_heroCostume;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nCostumeId = m_body.costumeId;
		int nCostumeEffectId = m_body.costumeEffectId;
		if (nCostumeId < 0)
		{
			throw new CommandHandleException(1, "코스튬ID가 유효하지 않습니다. nCostumeId = " + nCostumeId);
		}
		if (nCostumeEffectId < 0)
		{
			throw new CommandHandleException(1, "코스튬효과ID가 유효하지 않습니다. nCostumeEffectId = " + nCostumeEffectId);
		}
		m_heroCostume = m_myHero.GetCostume(nCostumeId);
		if (m_heroCostume == null)
		{
			throw new CommandHandleException(1, "영웅코스튬ID가 유효하지 않습니다. nCostumeId = " + nCostumeId);
		}
		CostumeEffect costumeEffect = Resource.instance.GetCostumeEffect(nCostumeEffectId);
		if (costumeEffect == null)
		{
			throw new CommandHandleException(1, "코스튬효과ID가 유효하지 않습니다. nCostumeEffectId = " + nCostumeEffectId);
		}
		int nRequiredItemId = costumeEffect.requiredItemId;
		if (m_myHero.GetItemCount(nRequiredItemId) <= 0)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(nRequiredItemId, bFirstUseOwn: true, 1, m_changedInventorySlots);
		int nOldCostumeEffectId = m_heroCostume.costumeEffectId;
		m_heroCostume.costumeEffectId = nCostumeEffectId;
		SaveToDB();
		SaveToDB_Log(nOldCostumeEffectId);
		if (m_heroCostume.isEquipped)
		{
			Place currentPlace = m_myHero.currentPlace;
			if (currentPlace != null)
			{
				ServerEvent.SendHeroCostumeEffectApplied(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, nCostumeEffectId);
			}
		}
		CostumeEffectApplyResponseBody resBody = new CostumeEffectApplyResponseBody();
		resBody.changedInventorySlot = m_changedInventorySlots[0].ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCostume_CostumeEffect(m_heroCostume.hero.id, m_heroCostume.costumeId, m_heroCostume.costumeEffectId));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(int nOldCostumeEffectId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCostumeEffectApplicationLog(Guid.NewGuid(), m_myHero.id, m_heroCostume.costumeId, nOldCostumeEffectId, m_heroCostume.costumeEffectId, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
