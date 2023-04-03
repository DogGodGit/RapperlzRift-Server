using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Account
{
	private ClientPeer m_peer;

	private Guid m_id = Guid.Empty;

	private AccountStatus m_status;

	private Guid m_userId = Guid.Empty;

	private VirtualGameServer m_virtualGameServer;

	private Language m_language;

	private Guid m_lastHeroId = Guid.Empty;

	private Hero m_currentHero;

	private int m_nBaseUnOwnDia;

	private int m_nBonusUnOwnDia;

	private int m_nVipPoint;

	private HashSet<int> m_receivedVipLevelRewards = new HashSet<int>();

	private Dictionary<int, CashProductPurchaseCount> m_cashProductPurchaseCounts = new Dictionary<int, CashProductPurchaseCount>();

	private bool m_bFirstChargeEventObjectiveCompleted;

	private bool m_bFirstChargeEventRewarded;

	private int m_nRechargeEventAccUnOwnDia;

	private bool m_bRechargeEventRewarded;

	private Dictionary<int, AccountChargeEvent> m_chargeEvents = new Dictionary<int, AccountChargeEvent>();

	private DateTime m_dailyChargeEventDate = DateTime.MinValue.Date;

	private int m_nDailyChargeEventAccUnOwnDia;

	private HashSet<int> m_rewardedDailyChargeEventMissions = new HashSet<int>();

	private Dictionary<int, AccountConsumeEvent> m_consumeEvents = new Dictionary<int, AccountConsumeEvent>();

	private DateTime m_dailyConsumeEventDate = DateTime.MinValue.Date;

	private int m_nDailyConsumeEventAccDia;

	private HashSet<int> m_rewardedDailyConsumeEventMissions = new HashSet<int>();

	public ClientPeer peer => m_peer;

	public Guid id => m_id;

	public object syncObject
	{
		get
		{
			if (m_peer == null)
			{
				return this;
			}
			return m_peer.syncObject;
		}
	}

	public AccountStatus status => m_status;

	public bool isLoggingIn => m_status == AccountStatus.LoggingIn;

	public bool isLoggedIn => m_status == AccountStatus.LoggedIn;

	public bool isLoggedOut => m_status == AccountStatus.LoggedOut;

	public Guid userId => m_userId;

	public VirtualGameServer virtualGameserver => m_virtualGameServer;

	public int virtualGameServerId => m_virtualGameServer.id;

	public Language language => m_language;

	public Guid lastHeroId => m_lastHeroId;

	public Hero currentHero
	{
		get
		{
			return m_currentHero;
		}
		set
		{
			m_currentHero = value;
		}
	}

	public int baseUnOwnDia => m_nBaseUnOwnDia;

	public int bonusUnOwnDia => m_nBonusUnOwnDia;

	public int unOwnDia => m_nBaseUnOwnDia + m_nBonusUnOwnDia;

	public int vipPoint => m_nVipPoint;

	public HashSet<int> receivedVipLevelRewards => m_receivedVipLevelRewards;

	public bool firstChargeEventObjectiveCompleted => m_bFirstChargeEventObjectiveCompleted;

	public bool firstChargeEventRewarded
	{
		get
		{
			return m_bFirstChargeEventRewarded;
		}
		set
		{
			m_bFirstChargeEventRewarded = value;
		}
	}

	public int rechargeEventAccUnOwnDia => m_nRechargeEventAccUnOwnDia;

	public bool rechargeEventRewarded
	{
		get
		{
			return m_bRechargeEventRewarded;
		}
		set
		{
			m_bRechargeEventRewarded = value;
		}
	}

	public DateTime dailyChargeEventDate => m_dailyChargeEventDate;

	public int dailyChargeEventAccUnOwnDia => m_nDailyChargeEventAccUnOwnDia;

	public HashSet<int> rewardedDailyChargeEventMissions => m_rewardedDailyChargeEventMissions;

	public DateTime dailyConsumeEventDate => m_dailyConsumeEventDate;

	public int dailyConsumeEventAccDia => m_nDailyConsumeEventAccDia;

	public HashSet<int> rewardedDailyConsumeEventMissions => m_rewardedDailyConsumeEventMissions;

	public Account(Guid id)
	{
		m_id = id;
	}

	public Account(ClientPeer peer, Guid id, DataRow drUser, VirtualGameServer virtualGameServer)
	{
		m_peer = peer;
		m_id = id;
		m_userId = (Guid)drUser["userId"];
		m_virtualGameServer = virtualGameServer;
		int nLanguageId = Convert.ToInt32(drUser["languageId"]);
		m_language = Resource.instance.GetLanguage(nLanguageId);
		if (m_language == null)
		{
			throw new Exception(string.Concat("언어가 존재하지 않습니다. userId = ", m_userId, ", nLanguageId = ", nLanguageId));
		}
	}

	public void BeginLogIn()
	{
		if (m_status != 0)
		{
			throw new InvalidOperationException("계정 상태가 유효하지 않습니다..");
		}
		m_status = AccountStatus.LoggingIn;
	}

	public void CompleteLogIn(DataRow drAccount, DataRowCollection drcVipLevelRewards, DataRowCollection drcCashProductPurchaseCounts, DataRow drChargeEvent, DataRowCollection drcChargeEventMissionRewards, DataRow drDailyChargeEvent, DataRowCollection drcDailyChargeEventMissionRewards, DataRow drConsumeEvent, DataRowCollection drcConsumeEventMissionRewards, DataRow drDailyConsumeEvent, DataRowCollection drcDailyConsumeEventMissionRewards)
	{
		if (!isLoggingIn)
		{
			throw new InvalidOperationException("계정 상태가 로그인중이 아닙니다.");
		}
		CompleteLogIn_Base(drAccount);
		CompleteLogIn_VipLevelReward(drcVipLevelRewards);
		CompleteLogIn_CashProductPurchaseCount(drcCashProductPurchaseCounts);
		CompleteLogIn_FirstChargeEvent(drAccount);
		CompleteLogIn_RechargeEvent(drAccount);
		CompleteLogIn_ChargeEvent(drChargeEvent, drcChargeEventMissionRewards);
		CompleteLogIn_DailyChargeEvent(drDailyChargeEvent, drcDailyChargeEventMissionRewards);
		CompleteLogIn_ConsumeEvent(drConsumeEvent, drcConsumeEventMissionRewards);
		CompleteLogIn_DailyConsumeEvent(drDailyConsumeEvent, drcDailyConsumeEventMissionRewards);
		m_status = AccountStatus.LoggedIn;
	}

	private void CompleteLogIn_Base(DataRow drAccount)
	{
		m_lastHeroId = (Guid)drAccount["lastHeroId"];
		m_nBaseUnOwnDia = Convert.ToInt32(drAccount["baseUnOwnDia"]);
		m_nBonusUnOwnDia = Convert.ToInt32(drAccount["bonusUnOwnDia"]);
		m_nVipPoint = Convert.ToInt32(drAccount["vipPoint"]);
	}

	private void CompleteLogIn_VipLevelReward(DataRowCollection drcVipLevelRewards)
	{
		foreach (DataRow dr in drcVipLevelRewards)
		{
			int nReceivedLevel = Convert.ToInt32(dr["vipLevel"]);
			AddReceivedVipLevelReward(nReceivedLevel);
		}
	}

	private void CompleteLogIn_CashProductPurchaseCount(DataRowCollection drcCashProductPurchaseCounts)
	{
		foreach (DataRow dr in drcCashProductPurchaseCounts)
		{
			CashProductPurchaseCount count = new CashProductPurchaseCount(this);
			count.Init(dr);
			AddCashProductPurchaseCount(count);
		}
	}

	private void CompleteLogIn_FirstChargeEvent(DataRow drAccount)
	{
		m_bFirstChargeEventObjectiveCompleted = Convert.ToBoolean(drAccount["firstChargeEventObjectiveCompleted"]);
		m_bFirstChargeEventRewarded = Convert.ToBoolean(drAccount["firstChargeEventRewarded"]);
	}

	private void CompleteLogIn_RechargeEvent(DataRow drAccount)
	{
		m_nRechargeEventAccUnOwnDia = Convert.ToInt32(drAccount["rechargeEventAccUnOwnDia"]);
		m_bRechargeEventRewarded = Convert.ToBoolean(drAccount["rechargeEventRewarded"]);
	}

	private void CompleteLogIn_ChargeEvent(DataRow drChargeEvent, DataRowCollection drcChargeEventMissionRewards)
	{
		if (drChargeEvent == null)
		{
			return;
		}
		AccountChargeEvent evt = new AccountChargeEvent(this);
		evt.Init(drChargeEvent);
		AddChargeEvent(evt);
		foreach (DataRow dr in drcChargeEventMissionRewards)
		{
			evt.AddRewardedMission(Convert.ToInt32(dr["missionNo"]));
		}
	}

	private void CompleteLogIn_DailyChargeEvent(DataRow drDailyChargeEvent, DataRowCollection drcDailyChargeEventMissionRewards)
	{
		if (drDailyChargeEvent == null)
		{
			return;
		}
		m_dailyChargeEventDate = Convert.ToDateTime(drDailyChargeEvent["date"]);
		m_nDailyChargeEventAccUnOwnDia = Convert.ToInt32(drDailyChargeEvent["accUnOwnDia"]);
		foreach (DataRow dr in drcDailyChargeEventMissionRewards)
		{
			AddRewardedDailyChargeEventMission(Convert.ToInt32(dr["missionNo"]));
		}
	}

	private void CompleteLogIn_ConsumeEvent(DataRow drConsumeEvent, DataRowCollection drcConsumeEventMissionRewards)
	{
		if (drConsumeEvent == null)
		{
			return;
		}
		AccountConsumeEvent evt = new AccountConsumeEvent(this);
		evt.Init(drConsumeEvent);
		AddConsumeEvent(evt);
		foreach (DataRow dr in drcConsumeEventMissionRewards)
		{
			evt.AddRewardedMission(Convert.ToInt32(dr["missionNo"]));
		}
	}

	private void CompleteLogIn_DailyConsumeEvent(DataRow drDailyConsumeEvent, DataRowCollection drcDailyConsumeEventMissionRewards)
	{
		if (drDailyConsumeEvent == null)
		{
			return;
		}
		m_dailyConsumeEventDate = Convert.ToDateTime(drDailyConsumeEvent["date"]);
		m_nDailyConsumeEventAccDia = Convert.ToInt32(drDailyConsumeEvent["accDia"]);
		foreach (DataRow dr in drcDailyConsumeEventMissionRewards)
		{
			AddRewardedDailyConsumeEventMission(Convert.ToInt32(dr["missionNo"]));
		}
	}

	public void LogOut()
	{
		if (isLoggingIn || isLoggedIn)
		{
			if (m_currentHero != null)
			{
				m_currentHero.LogOut();
			}
			m_status = AccountStatus.LoggedOut;
			m_peer.account = null;
			Cache.instance.RemoveAccount(m_id);
		}
	}

	public void SetLastHeroId(Guid heroId)
	{
		if (!(m_lastHeroId == heroId))
		{
			m_lastHeroId = heroId;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateAccount_LastHero(m_id, m_lastHeroId));
			dbWork.Schedule();
		}
	}

	public void AddBaseUnOwnDia(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nBaseUnOwnDia += nAmount;
		}
	}

	public void AddBonusUnOwnDia(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nBonusUnOwnDia += nAmount;
		}
	}

	public void UseUnOwnDia(int nAmount, DateTimeOffset time)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (unOwnDia < nAmount)
			{
				throw new Exception("보유한 비귀속다이아가 부족합니다.");
			}
			int nWorkAmount = nAmount;
			if (m_nBonusUnOwnDia > 0)
			{
				int nUsedBonusUnOwnDia = Math.Min(m_nBonusUnOwnDia, nWorkAmount);
				m_nBonusUnOwnDia -= nUsedBonusUnOwnDia;
				nWorkAmount -= nUsedBonusUnOwnDia;
			}
			m_nBaseUnOwnDia -= nWorkAmount;
			ProcessConsumeEvent(nAmount, time);
			ProcessDailyConsumeEvent(nAmount, time);
		}
	}

	public void AddVipPoint(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nVipPoint += nAmount;
		}
	}

	public void AddReceivedVipLevelReward(int nLevel)
	{
		if (nLevel <= 0)
		{
			throw new ArgumentOutOfRangeException("nLevel");
		}
		m_receivedVipLevelRewards.Add(nLevel);
	}

	public bool IsVipLevelRewardRecieved(int nLevel)
	{
		return m_receivedVipLevelRewards.Contains(nLevel);
	}

	public void Release()
	{
		if (m_currentHero != null)
		{
			m_currentHero.Release();
		}
	}

	private void AddCashProductPurchaseCount(CashProductPurchaseCount count)
	{
		m_cashProductPurchaseCounts.Add(count.productId, count);
	}

	public CashProductPurchaseCount GetCashProductPurchaseCount(int nProductId)
	{
		if (!m_cashProductPurchaseCounts.TryGetValue(nProductId, out var value))
		{
			return null;
		}
		return value;
	}

	public CashProductPurchaseCount GetOrCreateCashProductPurchaseCount(int nProductId)
	{
		CashProductPurchaseCount inst = GetCashProductPurchaseCount(nProductId);
		if (inst == null)
		{
			inst = new CashProductPurchaseCount(this, nProductId, 0);
			AddCashProductPurchaseCount(inst);
		}
		return inst;
	}

	public List<PDCashProductPurchaseCount> GetPDCashProductPurchaseCounts()
	{
		return CashProductPurchaseCount.ToPDCashProductPurchaseCounts(m_cashProductPurchaseCounts.Values);
	}

	public void ProcessFirstChargeEventAndRechargeEvent(int nAmount)
	{
		ProcessRechargeEvent(nAmount);
		ProcessFirstChargeEvent();
	}

	private void ProcessFirstChargeEvent()
	{
		FirstChargeEvent evt = Resource.instance.firstChargeEvent;
		if (evt != null && !m_bFirstChargeEventObjectiveCompleted)
		{
			m_bFirstChargeEventObjectiveCompleted = true;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateAccount_FirstChargeEventObjectiveCompletion(m_id));
			dbWork.Schedule();
			ServerEvent.SendFirstChargeEventObjectiveCompleted(m_peer);
		}
	}

	private void ProcessRechargeEvent(int nAmount)
	{
		if (nAmount > 0)
		{
			RechargeEvent evt = Resource.instance.rechargeEvent;
			if (evt != null && m_bFirstChargeEventObjectiveCompleted && !m_bRechargeEventRewarded)
			{
				m_nRechargeEventAccUnOwnDia += nAmount;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateAccount_RechargeEventAccUnOwnDia(m_id, m_nRechargeEventAccUnOwnDia));
				dbWork.Schedule();
				ServerEvent.SendRechargeEventProgress(m_peer, m_nRechargeEventAccUnOwnDia);
			}
		}
	}

	private void AddChargeEvent(AccountChargeEvent evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException("evt");
		}
		m_chargeEvents.Add(evt.eventId, evt);
	}

	public AccountChargeEvent GetChargeEvent(int nEventId)
	{
		if (!m_chargeEvents.TryGetValue(nEventId, out var value))
		{
			return null;
		}
		return value;
	}

	public AccountChargeEvent GetOrCreateChargeEvent(int nEventId)
	{
		AccountChargeEvent inst = GetChargeEvent(nEventId);
		if (inst == null)
		{
			inst = new AccountChargeEvent(this, nEventId);
			AddChargeEvent(inst);
		}
		return inst;
	}

	public void ProcessChargeEvent(int nAmount, DateTimeOffset time)
	{
		if (nAmount > 0)
		{
			ChargeEvent evt = Resource.instance.GetChargeEventByTime(time.DateTime);
			if (evt != null)
			{
				AccountChargeEvent accountChargeEvent = GetOrCreateChargeEvent(evt.id);
				accountChargeEvent.AddUnOwnDia(nAmount);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateAccountChargeEvent(m_id, accountChargeEvent.eventId, accountChargeEvent.accUnOwnDia));
				dbWork.Schedule();
				ServerEvent.SendChargeEventProgress(m_peer, accountChargeEvent.eventId, accountChargeEvent.accUnOwnDia);
			}
		}
	}

	public void AddRewardedDailyChargeEventMission(int nMissionNo)
	{
		m_rewardedDailyChargeEventMissions.Add(nMissionNo);
	}

	public bool IsRewardedDailyChargeEventMission(int nMissionNo)
	{
		return m_rewardedDailyChargeEventMissions.Contains(nMissionNo);
	}

	public void RefreshDailyChargeEvent(DateTime date)
	{
		if (!(m_dailyChargeEventDate == date))
		{
			m_dailyChargeEventDate = date;
			m_nDailyChargeEventAccUnOwnDia = 0;
			m_rewardedDailyChargeEventMissions.Clear();
		}
	}

	public void ProcessDailyChargeEvent(int nAmount, DateTimeOffset time)
	{
		if (nAmount > 0)
		{
			DailyChargeEvent evt = Resource.instance.dailyChargeEvent;
			if (evt != null)
			{
				RefreshDailyChargeEvent(time.Date);
				m_nDailyChargeEventAccUnOwnDia += nAmount;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateAccountDailyChargeEvent(m_id, m_dailyChargeEventDate, m_nDailyChargeEventAccUnOwnDia));
				dbWork.Schedule();
				ServerEvent.SendDailyChargeEventProgress(m_peer, m_dailyChargeEventDate, m_nDailyChargeEventAccUnOwnDia);
			}
		}
	}

	private void AddConsumeEvent(AccountConsumeEvent evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException("evt");
		}
		m_consumeEvents.Add(evt.eventId, evt);
	}

	public AccountConsumeEvent GetConsumeEvent(int nEventId)
	{
		if (!m_consumeEvents.TryGetValue(nEventId, out var value))
		{
			return null;
		}
		return value;
	}

	public AccountConsumeEvent GetOrCreateConsumeEvent(int nEventId)
	{
		AccountConsumeEvent inst = GetConsumeEvent(nEventId);
		if (inst == null)
		{
			inst = new AccountConsumeEvent(this, nEventId);
			AddConsumeEvent(inst);
		}
		return inst;
	}

	public void ProcessConsumeEvent(int nAmount, DateTimeOffset time)
	{
		if (nAmount > 0)
		{
			ConsumeEvent evt = Resource.instance.GetConsumeEventByTime(time.DateTime);
			if (evt != null)
			{
				AccountConsumeEvent accountConsumeEvent = GetOrCreateConsumeEvent(evt.id);
				accountConsumeEvent.AddDia(nAmount);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateAccountConsumeEvent(m_id, accountConsumeEvent.eventId, accountConsumeEvent.accDia));
				dbWork.Schedule();
				ServerEvent.SendConsumeEventProgress(m_peer, accountConsumeEvent.eventId, accountConsumeEvent.accDia);
			}
		}
	}

	public void AddRewardedDailyConsumeEventMission(int nMissionNo)
	{
		m_rewardedDailyConsumeEventMissions.Add(nMissionNo);
	}

	public bool IsRewardedDailyConsumeEventMission(int nMissionNo)
	{
		return m_rewardedDailyConsumeEventMissions.Contains(nMissionNo);
	}

	public void RefreshDailyConsumeEvent(DateTime date)
	{
		if (!(m_dailyConsumeEventDate == date))
		{
			m_dailyConsumeEventDate = date;
			m_nDailyConsumeEventAccDia = 0;
			m_rewardedDailyConsumeEventMissions.Clear();
		}
	}

	public void ProcessDailyConsumeEvent(int nAmount, DateTimeOffset time)
	{
		if (nAmount > 0)
		{
			DailyConsumeEvent evt = Resource.instance.dailyConsumeEvent;
			if (evt != null)
			{
				RefreshDailyConsumeEvent(time.Date);
				m_nDailyConsumeEventAccDia += nAmount;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateAccountWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateAccountDailyConsumeEvent(m_id, m_dailyConsumeEventDate, m_nDailyConsumeEventAccDia));
				dbWork.Schedule();
				ServerEvent.SendDailyConsumeEventProgress(m_peer, m_dailyConsumeEventDate, m_nDailyConsumeEventAccDia);
			}
		}
	}
}
