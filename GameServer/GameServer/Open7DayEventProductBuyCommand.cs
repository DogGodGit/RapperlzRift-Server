using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class Open7DayEventProductBuyCommandHandler : InGameCommandHandler<Open7DayEventProductBuyCommandBody, Open7DayEventProductBuyResponseBody>
{
	public const short kResult_AlreadyPurchasedProduct = 101;

	public const short kResult_NotOpendEvent = 102;

	public const short kResult_CompletedEvent = 103;

	public const short kResult_NotOpendEventDay = 104;

	public const short kResult_NotEnoughUnOwnDia = 105;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private Item m_productItem;

	private bool m_bProductItemOwned;

	private int m_nProductItemCount;

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nProductId = m_body.productId;
		if (nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다. nProductId = " + nProductId);
		}
		if (m_myHero.IsPurchasedOpen7DayEventProduct(nProductId))
		{
			throw new CommandHandleException(101, "이미 구매한 상품입니다.");
		}
		if (!m_myHero.IsMainQuestCompleted(Resource.instance.open7DayEventRequiredMainQuestNo))
		{
			throw new CommandHandleException(102, "개방되지 않은 이벤트입니다.");
		}
		if (m_myHero.isOpen7DayEventCompleted)
		{
			throw new CommandHandleException(103, "미션을 전부 완료했습니다.");
		}
		Open7DayEventProduct product = Resource.instance.GetOpen7DayEventDayProduct(nProductId);
		if (product == null)
		{
			throw new CommandHandleException(1, "상품이 존재하지 않습니다. nProductId = " + nProductId);
		}
		if (product.day.day > m_myHero.GetElapsedDaysFromCreation(m_currentTime))
		{
			throw new CommandHandleException(104, "아직 개방되지 않은 이벤트일차입니다.");
		}
		int nRequiredDia = product.requiredDia;
		if (m_myHero.unOwnDia < nRequiredDia)
		{
			throw new CommandHandleException(105, "비귀속다이아가 부족합니다.");
		}
		m_productItem = product.item;
		m_bProductItemOwned = product.itemOwned;
		m_nProductItemCount = product.itemCount;
		int nRemainingItemCount = m_myHero.AddItem(m_productItem, m_bProductItemOwned, m_nProductItemCount, m_changedInventorySlots);
		if (nRemainingItemCount > 0)
		{
			m_mail = Mail.Create("MAIL_REWARD_N_19", "MAIL_REWARD_D_19", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(m_productItem, m_nProductItemCount, m_bProductItemOwned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myAccount.UseUnOwnDia(nRequiredDia, m_currentTime);
		m_nUsedUnOwnDia = nRequiredDia;
		m_myHero.AddPurchasedOpen7DayEventProducts(nProductId);
		SaveToDB(nProductId);
		SaveToLogDB(nProductId);
		Open7DayEventProductBuyResponseBody resBody = new Open7DayEventProductBuyResponseBody();
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nProductId)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroOpen7DayEventProduct(m_myHero.id, nProductId));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nProductId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOpen7DayEventProductBuyLog(Guid.NewGuid(), m_myHero.id, nProductId, m_nUsedOwnDia, m_nUsedUnOwnDia, m_productItem.id, m_bProductItemOwned, m_nProductItemCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
