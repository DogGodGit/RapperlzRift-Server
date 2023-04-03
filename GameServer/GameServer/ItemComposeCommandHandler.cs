using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ItemComposeCommandHandler : InGameCommandHandler<ItemComposeCommandBody, ItemComposeResponseBody>
{
	public const short kResult_NotEnoughMaterialItem = 101;

	public const short kResult_NotEnoughGold = 102;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMaterialItemId = m_body.materialItemId;
		bool bOwned = m_body.owned;
		if (nMaterialItemId <= 0)
		{
			throw new CommandHandleException(1, "재료아이템 ID가 유효하지 않습니다. nMaterialItemId = " + nMaterialItemId);
		}
		ItemCompositionRecipe recipe = Resource.instance.GetItemCompositionRecipe(nMaterialItemId);
		if (recipe == null)
		{
			throw new CommandHandleException(1, "합성이 불가능한 아이템입니다. nMaterialItemId = " + nMaterialItemId);
		}
		Item compositeItem = recipe.item;
		int nMaterialItemCount = recipe.materialItemCount;
		long lnPriceGold = recipe.gold;
		int nResultCount = 1;
		bool bResultOwned = bOwned;
		int nTargetItemCount = m_myHero.GetItemCount(nMaterialItemId, bOwned);
		if (nTargetItemCount <= 0)
		{
			throw new CommandHandleException(1, "재료아이템이 존재하지 않습니다. nMaterialItemId = " + nMaterialItemId + ", bOwned = " + bOwned);
		}
		if (nTargetItemCount < nMaterialItemCount)
		{
			if (m_myHero.GetItemCount(nMaterialItemId, !bOwned) < nMaterialItemCount - nTargetItemCount)
			{
				throw new CommandHandleException(101, "재료 아이템이 부족합니다. nMaterialItemId = " + nMaterialItemId + ", bOwned = " + bOwned);
			}
			bResultOwned = true;
		}
		if (m_myHero.gold < lnPriceGold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		m_myHero.UseItem(nMaterialItemId, bOwned, nMaterialItemCount, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
		m_myHero.UseGold(lnPriceGold);
		int nRemainingCount = m_myHero.AddItem(compositeItem, bResultOwned, nResultCount, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_REWARD_N_101", "MAIL_REWARD_D_101", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(compositeItem, nRemainingCount, bResultOwned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToDB_AddItemCompositionLog(compositeItem.id, nResultCount, bResultOwned, nMaterialItemId, nUsedOwnCount, nUsedUnOwnCount, lnPriceGold);
		ItemComposeResponseBody resBody = new ItemComposeResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.gold = m_myHero.gold;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_AddItemCompositionLog(int nResultItemId, int nResultItemCount, bool bResultItemOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, long lnUsedGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemCompositionLog(Guid.NewGuid(), m_myHero.id, nResultItemId, nResultItemCount, bResultItemOwned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, lnUsedGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
