using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CashProductPurchaseCompleteCommandHandler : InGameCommandHandler<CashProductPurchaseCompleteCommandBody, CashProductPurchaseCompleteResponseBody>
{
	public const short kResult_PurchaseNotExist = 101;

	public const short kResult_InvalidStatus = 102;

	private Guid m_purchaseId = Guid.Empty;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nProductId;

	private CashProduct m_product;

	private CashProductPurchaseCount m_purchaseCount;

	private int m_nProvidedUnOwnDia;

	private Item m_providedItem;

	private bool m_bProvidedItemOwned;

	private int m_nProvidedItemCount;

	private int m_nProvidedFirstPurchaseBonusUnOwnDia;

	private int m_nProvidedTotalUnOwnDia;

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_purchaseId = (Guid)m_body.purchaseId;
		if (m_purchaseId == Guid.Empty)
		{
			throw new CommandHandleException(1, "구매ID가 유효하지 않습니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
		work.runnable = new SFAction(Process);
		RunWork(work);
	}

	private void Process()
	{
		SqlConnection conn = null;
		SqlTransaction trans = null;
		try
		{
			conn = DBUtil.OpenUserDBConnection();
			trans = conn.BeginTransaction();
			DataRow drPurchase = UserDac.Purchase_x(conn, trans, m_purchaseId);
			if (drPurchase == null)
			{
				throw new CommandHandleException(101, "구매내역이 존재하지 않습니다. m_purchaseId = " + m_purchaseId);
			}
			Guid heroId = (Guid)drPurchase["heroId"];
			PurchaseStatus status = (PurchaseStatus)Convert.ToInt32(drPurchase["status"]);
			m_nProductId = Convert.ToInt32(drPurchase["productId"]);
			if (heroId != m_myHero.id)
			{
				throw new CommandHandleException(1, "구매를 시작한 영웅이 아닙니다. m_purchaseId = " + m_purchaseId);
			}
			if (status != PurchaseStatus.PaymentCompleted)
			{
				throw new CommandHandleException(102, "구매상태가 유효하지 않습니다. m_purchaseId = " + m_purchaseId);
			}
			if (UserDac.UpdatePurchase(conn, trans, m_purchaseId, 2, m_currentTime, null) != 0)
			{
				throw new CommandHandleException(1, "구매 수정 실패. m_purchaseId = " + m_purchaseId);
			}
			SFDBUtil.Commit(ref trans);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Rollback(ref trans);
			SFDBUtil.Close(ref conn);
		}
	}

	protected override void OnWork_Success(SFWork work)
	{
		base.OnWork_Success(work);
		m_product = Resource.instance.GetCashProduct(m_nProductId);
		if (m_product == null)
		{
			throw new CommandHandleException(1, string.Concat("상품이 존재하지 않습니다. m_purchaseId = ", m_purchaseId, ", m_nProductId = ", m_nProductId));
		}
		switch (m_product.type)
		{
		case CashProductType.Dia:
			m_nProvidedUnOwnDia = m_product.unOwnDia;
			m_myAccount.AddBaseUnOwnDia(m_nProvidedUnOwnDia);
			m_nProvidedTotalUnOwnDia += m_nProvidedUnOwnDia;
			break;
		case CashProductType.Item:
			m_providedItem = m_product.item;
			m_bProvidedItemOwned = m_product.itemOwned;
			m_nProvidedItemCount = m_product.itemCount;
			m_mail = Mail.Create("MAIL_REWARD_N_32", "MAIL_REWARD_D_32", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(m_providedItem, m_nProvidedItemCount, m_bProvidedItemOwned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
			break;
		default:
			throw new CommandHandleException(1, string.Concat("상품타입이 유효하지 않습니다. m_purchaseId = ", m_purchaseId, ", m_product.type = ", m_product.type));
		}
		m_purchaseCount = m_myAccount.GetOrCreateCashProductPurchaseCount(m_nProductId);
		if (m_product.firstPurchaseBonusUnOwnDia > 0 && m_purchaseCount.count == 0)
		{
			m_nProvidedFirstPurchaseBonusUnOwnDia = m_product.firstPurchaseBonusUnOwnDia;
			m_myAccount.AddBonusUnOwnDia(m_nProvidedFirstPurchaseBonusUnOwnDia);
			m_nProvidedTotalUnOwnDia += m_nProvidedFirstPurchaseBonusUnOwnDia;
		}
		m_myHero.AddVipPoint(m_product.vipPoint, 0);
		m_purchaseCount.count++;
		SaveToDB();
		SaveToGameLogDB();
		CashProductPurchaseCompleteResponseBody resBody = new CashProductPurchaseCompleteResponseBody();
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.vipPoint = m_myHero.vipPoint;
		SendResponseOK(resBody);
		if (m_product.type == CashProductType.Dia)
		{
			m_myAccount.ProcessFirstChargeEventAndRechargeEvent(m_nProvidedTotalUnOwnDia);
			m_myAccount.ProcessChargeEvent(m_nProvidedTotalUnOwnDia, m_currentTime);
			m_myAccount.ProcessDailyChargeEvent(m_nProvidedTotalUnOwnDia, m_currentTime);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_myAccount.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateAccount_VipPoint(m_myAccount.id, m_myAccount.vipPoint));
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateCashProductPurchaseCount(m_myAccount.id, m_purchaseCount.productId, m_purchaseCount.count));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddPurchaseProvideCompletionLog(Guid.NewGuid(), m_purchaseId, m_nProductId, m_myHero.id, m_nProvidedUnOwnDia, (m_providedItem != null) ? m_providedItem.id : 0, m_bProvidedItemOwned, m_nProvidedItemCount, m_nProvidedFirstPurchaseBonusUnOwnDia, m_product.vipPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
