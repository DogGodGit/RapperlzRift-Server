using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CashProductPurchaseCancelCommandHandler : InGameCommandHandler<CashProductPurchaseCancelCommandBody, CashProductPurchaseCancelResponseBody>
{
	public const short kResult_PurchaseNotExist = 101;

	public const short kResult_InvalidStatus = 102;

	private Guid m_purchaseId = Guid.Empty;

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
			if (heroId != m_myHero.id)
			{
				throw new CommandHandleException(1, "구매를 시작한 영웅이 아닙니다. m_purchaseId = " + m_purchaseId);
			}
			if (status != 0)
			{
				throw new CommandHandleException(102, "구매상태가 유효하지 않습니다. m_purchaseId = " + m_purchaseId);
			}
			if (UserDac.UpdatePurchase(conn, trans, m_purchaseId, 3, DateTimeUtil.currentTime, null) != 0)
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
		SendResponseOK(null);
	}
}
