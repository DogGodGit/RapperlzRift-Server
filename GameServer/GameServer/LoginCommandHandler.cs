using System;
using System.Data;
using System.Data.SqlClient;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class LoginCommandHandler : CommandHandler<LoginCommandBody, LoginResponseBody>
{
	public const short kResult_AlreadyLoggingInOrLoggedIn = 101;

	public const short kResult_VirtualGameServerNotExist = 102;

	public const short kResult_GameServerNotEqual = 103;

	public const short kResult_UserNotExist = 104;

	public const short kResult_UserAccessSecretNotEqual = 105;

	public const short kResult_TooManyUser = 106;

	private const int kWorkStep_ProcessUserLogin = 1;

	private const int kWorkStep_GetAccountInfo = 2;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private int m_nWorkStep;

	private int m_nVirtualGameServerId;

	private VirtualGameServer m_virtualGameServer;

	private UserAccessToken m_accessToken;

	private Guid m_userId = Guid.Empty;

	private Guid m_accountId = Guid.Empty;

	private Account m_account;

	private DataRow m_drUser;

	private DataRow m_drAccount;

	private DataRowCollection m_drcVipLevelRewards;

	private DataRowCollection m_drcCashProductPurchaseCounts;

	private DataRow m_drChargeEvent;

	private DataRowCollection m_drcChargeEventMissionRewards;

	private DataRow m_drDailyChargeEvent;

	private DataRowCollection m_drcDailyChargeEventMissionRewards;

	private DataRow m_drConsumeEvent;

	private DataRowCollection m_drcConsumeEventMissionRewards;

	private DataRow m_drDailyConsumeEvent;

	private DataRowCollection m_drcDailyConsumeEventMissionRewards;

	protected override bool globalLockRequired => true;

	protected override void HandleCommandInternal()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		m_nVirtualGameServerId = m_body.virtualGameServerId;
		string sAccessToken = m_body.accessToken;
		m_virtualGameServer = SystemResource.instance.GetVirtualGameServer(m_nVirtualGameServerId);
		if (m_virtualGameServer == null)
		{
			throw new CommandHandleException(102, "가상게임서버가 존재하지 않습니다. m_nVirtualGameServerId = " + m_nVirtualGameServerId);
		}
		if (m_virtualGameServer.gameServer.id != GameServerApp.inst.serverId)
		{
			throw new CommandHandleException(103, "게임서버가 다릅니다. currentGameServerId = " + GameServerApp.inst.serverId + ", m_nVirtualGameServerId = " + m_nVirtualGameServerId);
		}
		if (Cache.instance.isUserConnectionFull)
		{
			throw new CommandHandleException(106, "현재 사용자가 너무 많습니다.", null, bLoggingEnabled: false);
		}
		if (!UserAccessToken.TryParse(sAccessToken, out m_accessToken))
		{
			throw new CommandHandleException(1, "엑세스토큰 파싱 에러입니다.");
		}
		if (!m_accessToken.isValid)
		{
			throw new CommandHandleException(1, "엑세스토큰이 유효하지 않습니다. userId = " + m_accessToken.userId);
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_userId = m_accessToken.userId;
		m_nWorkStep = 1;
		SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
		work.runnable = new SFAction(ProcessUserLogin);
		RunWork(work);
	}

	protected override void OnWork_Success(SFWork work)
	{
		base.OnWork_Success(work);
		switch (m_nWorkStep)
		{
		case 1:
			OnWork_Success_ProcessUserLogin();
			break;
		case 2:
			OnWork_Success_GetAccountInfo();
			break;
		default:
			throw new CommandHandleException(1, "작업스텝이 유효하지 않습니다.");
		}
	}

	protected override void OnWork_Error(SFWork work, Exception error)
	{
		base.OnWork_Error(work, error);
		int nWorkStep = m_nWorkStep;
		if (nWorkStep == 2)
		{
			OnWork_Error_GetAccountInfo();
		}
	}

	private void ProcessUserLogin()
	{
		Verify();
		CreateAccount();
	}

	private void Verify()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenUserDBConnection();
			m_drUser = UserDac.User(conn, null, m_userId);
			if (m_drUser == null)
			{
				throw new CommandHandleException(104, "사용자가 존재하지 않습니다. userId = " + m_userId);
			}
			string sAccessSecret = Convert.ToString(m_drUser["accessSecret"]);
			if (m_accessToken.accessSecret != sAccessSecret)
			{
				throw new CommandHandleException(105, string.Concat("엑세스시크릿이 다릅니다. userId = ", m_userId, ", m_accessToken.accessSecret = ", m_accessToken.accessSecret));
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void CreateAccount()
	{
		SqlConnection conn = null;
		SqlTransaction trans = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			trans = conn.BeginTransaction();
			DataRow drAccount = GameDac.AccountByUserIdAndVirtualGameServerId_x(conn, trans, m_userId, m_nVirtualGameServerId);
			if (drAccount != null)
			{
				m_accountId = (Guid)drAccount["accountId"];
			}
			else
			{
				m_accountId = Guid.NewGuid();
				if (GameDac.AddAccount(conn, trans, m_accountId, m_userId, m_nVirtualGameServerId, m_currentTime) != 0)
				{
					throw new CommandHandleException(1, "계정 등록 실패.");
				}
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

	private void OnWork_Success_ProcessUserLogin()
	{
		ClientPeer peer = base.clientPeer;
		if (peer.account != null)
		{
			throw new CommandHandleException(101, "이미 로그인중이거나 로그인되어 있습니다. userId = " + m_userId);
		}
		Account occupiedAccount = Cache.instance.GetAccount(m_accountId);
		if (occupiedAccount != null)
		{
			AccountSynchronizer.Exec(occupiedAccount, new SFAction<Account>(LogoutDuplicatedAccount, occupiedAccount));
		}
		m_account = new Account(peer, m_accountId, m_drUser, m_virtualGameServer);
		m_account.BeginLogIn();
		peer.account = m_account;
		Cache.instance.AddAccount(m_account);
		try
		{
			m_nWorkStep = 2;
			SFRunnableQueuingWork work = RunnableQueuingWorkUtil.CreateAccountWork(m_accountId);
			work.runnable = new SFAction(GetAccountInfo);
			RunWork(work);
		}
		catch (Exception ex)
		{
			m_account.LogOut();
			throw new CommandHandleException(1, ex.Message, ex);
		}
	}

	private void LogoutDuplicatedAccount(Account account)
	{
		account.LogOut();
		ServerEvent.SendAccountLoginDuplicated(account.peer);
	}

	private void GetAccountInfo()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			m_drAccount = GameDac.Account(conn, null, m_accountId);
			if (m_drAccount == null)
			{
				throw new CommandHandleException(1, "계정이 존재하지 않습니다.");
			}
			m_drcVipLevelRewards = GameDac.VipLevelRewards(conn, null, m_accountId);
			m_drcCashProductPurchaseCounts = GameDac.CashProductPurchaseCounts(conn, null, m_accountId);
			ChargeEvent chargeEvent = Resource.instance.GetChargeEventByTime(m_currentTime.DateTime);
			if (chargeEvent != null)
			{
				m_drChargeEvent = GameDac.AccountChargeEvent(conn, null, m_accountId, chargeEvent.id);
				m_drcChargeEventMissionRewards = GameDac.AccountChargeEventMissionRewards(conn, null, m_accountId, chargeEvent.id);
			}
			m_drDailyChargeEvent = GameDac.AccountDailyChargeEvent(conn, null, m_accountId, m_currentDate);
			m_drcDailyChargeEventMissionRewards = GameDac.AccountDailyChargeEventMissionRewards(conn, null, m_accountId, m_currentDate);
			ConsumeEvent consumeEvent = Resource.instance.GetConsumeEventByTime(m_currentTime.DateTime);
			if (consumeEvent != null)
			{
				m_drConsumeEvent = GameDac.AccountConsumeEvent(conn, null, m_accountId, consumeEvent.id);
				m_drcConsumeEventMissionRewards = GameDac.AccountConsumeEventMissionRewards(conn, null, m_accountId, consumeEvent.id);
			}
			m_drDailyConsumeEvent = GameDac.AccountDailyConsumeEvent(conn, null, m_accountId, m_currentDate);
			m_drcDailyConsumeEventMissionRewards = GameDac.AccountDailyConsumeEventMissionRewards(conn, null, m_accountId, m_currentDate);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void OnWork_Success_GetAccountInfo()
	{
		try
		{
			string sIp = m_sender.RemoteIP;
			m_account.CompleteLogIn(m_drAccount, m_drcVipLevelRewards, m_drcCashProductPurchaseCounts, m_drChargeEvent, m_drcChargeEventMissionRewards, m_drDailyChargeEvent, m_drcDailyChargeEventMissionRewards, m_drConsumeEvent, m_drcConsumeEventMissionRewards, m_drDailyConsumeEvent, m_drcDailyConsumeEventMissionRewards);
			SFSqlQueuingWork dbWork2 = SqlQueuingWorkUtil.CreateAccountWork(m_accountId);
			dbWork2.AddSqlCommand(GameDac.CSC_UpdateAccount_Login(m_accountId, m_currentTime, sIp));
			dbWork2.Schedule();
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddAccountLoginLog(Guid.NewGuid(), m_accountId, sIp, m_currentTime));
			logWork.Schedule();
			int nLastVirtualGameServerId1 = Convert.ToInt32(m_drUser["lastVirtualGameServerId1"]);
			int nLastVirtualGameServerId2 = Convert.ToInt32(m_drUser["lastVirtualGameServerId2"]);
			if (nLastVirtualGameServerId1 != m_nVirtualGameServerId)
			{
				nLastVirtualGameServerId2 = nLastVirtualGameServerId1;
				nLastVirtualGameServerId1 = m_nVirtualGameServerId;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateUserWork(m_userId);
				dbWork.AddSqlCommand(UserDac.CSC_UpdateUser_LastVirtualGameServer(m_userId, nLastVirtualGameServerId1, nLastVirtualGameServerId2));
				dbWork.Schedule();
			}
			SendResponseOK(null);
		}
		catch (Exception ex)
		{
			m_account.LogOut();
			throw new CommandHandleException(1, ex.Message, ex);
		}
	}

	private void OnWork_Error_GetAccountInfo()
	{
		m_account.LogOut();
	}
}
