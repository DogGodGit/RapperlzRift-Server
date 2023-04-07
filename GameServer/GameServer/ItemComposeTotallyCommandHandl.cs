using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ItemComposeTotallyCommandHandler : InGameCommandHandler<ItemComposeTotallyCommandBody, ItemComposeTotallyResponseBody>
{
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
			throw new CommandHandleException(1, "재료아이템ID가 유효하지 않습니다. nMaterialItemId = " + nMaterialItemId);
		}
		ItemCompositionRecipe recipe = Resource.instance.GetItemCompositionRecipe(nMaterialItemId);
		if (recipe == null)
		{
			throw new CommandHandleException(1, "합성이 불가능한 아이템입니다. nMaterialItemId = " + nMaterialItemId);
		}
		Item resultItem = recipe.item;
		int nMaterialItemCount = recipe.materialItemCount;
		long lnPriceGold = recipe.gold;
		int nResultCount = 1;
		bool bResultOwned = bOwned;
		int nTotalResultCount = 0;
		int nTotalUsedOwnCount = 0;
		int nTotalUsedUnOwnCount = 0;
		long lnTotalUsedGold = 0L;
		int nRemainingCount = 0;
		int nTargetItemCount = m_myHero.GetItemCount(nMaterialItemId, bOwned);
		if (nTargetItemCount <= 0)
		{
			throw new CommandHandleException(1, "재료아이템이 존재하지 않습니다. nMaterialItemId = " + nMaterialItemId + ", bOwned = " + bOwned);
		}
		while (nTargetItemCount >= nMaterialItemCount && m_myHero.gold >= lnPriceGold)
		{
			int nUsedOwnCount = 0;
			int nUsedUnOwnCount = 0;
			m_myHero.UseItem(nMaterialItemId, bOwned, nMaterialItemCount, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
			m_myHero.UseGold(lnPriceGold);
			nRemainingCount += m_myHero.AddItem(resultItem, bResultOwned, nResultCount, m_changedInventorySlots);
			nTargetItemCount -= nMaterialItemCount;
			nTotalUsedOwnCount += nUsedOwnCount;
			nTotalUsedUnOwnCount += nUsedUnOwnCount;
			nTotalResultCount += nResultCount;
			lnTotalUsedGold += lnPriceGold;
		}
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_REWARD_N_101", "MAIL_REWARD_D_101", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(resultItem, nRemainingCount, bResultOwned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		if (nTotalResultCount > 0)
		{
			SaveToDB();
			SaveToDB_AddItemCompositionLog(resultItem.id, nTotalResultCount, bResultOwned, nMaterialItemId, nTotalUsedOwnCount, nTotalUsedUnOwnCount, lnTotalUsedGold);
		}
		ItemComposeTotallyResponseBody resBody = new ItemComposeTotallyResponseBody();
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

	private void SaveToDB_AddItemCompositionLog(int nResultItemId, int nResultCount, bool bResultOwned, int nMaterialItemId, int nMaterialOwnCount, int nMaterialUnOwnCount, long lnUsedGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemCompositionLog(Guid.NewGuid(), m_myHero.id, nResultItemId, nResultCount, bResultOwned, nMaterialItemId, nMaterialOwnCount, nMaterialUnOwnCount, lnUsedGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
