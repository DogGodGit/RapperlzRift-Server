using System;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CashProductPurchaseStartCommandHandler : InGameCommandHandler<CashProductPurchaseStartCommandBody, CashProductPurchaseStartResponseBody>
{
	private int m_nProductId;

	private StoreType m_storeType;

	private CashProduct m_product;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private Guid m_purchaseId = Guid.Empty;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nProductId = m_body.productId;
		if (m_nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다. m_nProductId = " + m_nProductId);
		}
		int nStoreType = m_body.storeType;
		if (!Enum.IsDefined(typeof(StoreType), nStoreType))
		{
			throw new CommandHandleException(1, "스토어타입이 유효하지 않습니다. nStoreType = " + nStoreType);
		}
		m_storeType = (StoreType)nStoreType;
		if (m_storeType == StoreType.None)
		{
			throw new CommandHandleException(1, "스토어타입이 유효하지 않습니다. nStoreType = " + nStoreType);
		}
		m_product = Resource.instance.GetCashProduct(m_nProductId);
		if (m_product == null)
		{
			throw new CommandHandleException(1, "상품이 존재하지 않습니다. m_nProductId = " + m_nProductId);
		}
		m_purchaseId = Guid.NewGuid();
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
			if (UserDac.AddPurchase(conn, trans, m_purchaseId, m_myAccount.userId, m_myAccount.virtualGameserver.gameServer.id, m_myAccount.id, m_myHero.id, m_nProductId, (int)m_storeType, m_currentTime) != 0)
			{
				throw new CommandHandleException(1, "구매 등록 실패.");
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
		CashProductPurchaseStartResponseBody resBody = new CashProductPurchaseStartResponseBody();
		resBody.purchaseId = (Guid)m_purchaseId;
		SendResponseOK(resBody);
	}
}
