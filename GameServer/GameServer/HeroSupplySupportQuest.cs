using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroSupplySupportQuest
{
	public const int kUpdateInterval = 500;

	public const int kStatus_Start = 0;

	public const int kStatus_Completed = 1;

	public const int kStatus_Fail = 2;

	public const int kLogStatus_Success = 1;

	public const int kLogStatus_Fail = 2;

	private SupplySupportQuest m_supplySupportQuest;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private int m_nOrderId;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private SupplySupportQuestCart m_cart;

	private int m_nCartHp;

	private bool m_bIsCartRiding;

	private int m_nCartContinentId;

	private Vector3 m_cartPosition = Vector3.zero;

	private float m_fCartYRotation;

	private SupplySupportQuestCartInstance m_cartInst;

	private List<int> m_visitedWayPoints = new List<int>();

	private Timer m_updateTimer;

	private bool m_bReleased;

	public SupplySupportQuest supplySupportQuest => m_supplySupportQuest;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public int orderId => m_nOrderId;

	public DateTimeOffset startTime => m_startTime;

	public SupplySupportQuestCart cart => m_cart;

	public int cartHp => m_nCartHp;

	public bool isCartRiding => m_bIsCartRiding;

	public int cartContinentId => m_nCartContinentId;

	public Vector3 cartPosition => m_cartPosition;

	public float cartYRotation => m_fCartYRotation;

	public SupplySupportQuestCartInstance cartInst
	{
		get
		{
			return m_cartInst;
		}
		set
		{
			m_cartInst = value;
		}
	}

	public HeroSupplySupportQuest(Hero hero)
	{
		m_supplySupportQuest = Resource.instance.supplySupportQuest;
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = SFDBUtil.ToGuid(dr["instanceId"]);
		m_nOrderId = Convert.ToInt32(dr["orderId"]);
		m_startTime = SFDBUtil.ToDateTimeOffset(dr["regTime"], DateTimeOffset.MinValue);
		int nCartId = Convert.ToInt32(dr["cartId"]);
		if (nCartId > 0)
		{
			m_cart = m_supplySupportQuest.GetCart(nCartId);
			if (m_cart == null)
			{
				SFLogUtil.Warn(GetType(), "수레가 존재하지 않습니다. nCartId = " + nCartId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "수레ID가 유효하지 않습니다. nCartId = " + nCartId);
		}
		m_nCartHp = Convert.ToInt32(dr["cartHp"]);
		m_bIsCartRiding = Convert.ToBoolean(dr["isCartRiding"]);
		m_nCartContinentId = Convert.ToInt32(dr["cartContinentId"]);
		m_cartPosition.x = Convert.ToSingle(dr["cartXPosition"]);
		m_cartPosition.y = Convert.ToSingle(dr["cartYPosition"]);
		m_cartPosition.z = Convert.ToSingle(dr["cartZPosition"]);
		m_fCartYRotation = Convert.ToSingle(dr["cartYRotation"]);
		StartUpdateTimer();
	}

	public void Init(SupplySupportQuestOrder order, DateTimeOffset time)
	{
		if (order == null)
		{
			throw new ArgumentNullException("order");
		}
		m_id = Guid.NewGuid();
		m_nOrderId = order.id;
		m_cart = order.SelectCart();
		m_startTime = time;
		StartUpdateTimer();
	}

	private void StartUpdateTimer()
	{
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
	}

	private void OnUpdateTimerTick(object state)
	{
		try
		{
			m_hero.AddWork(new SFAction(CheckLimitTime), bGlobalLockRequired: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void CheckLimitTime()
	{
		if (m_hero.supplySupportQuest != null)
		{
			DateTimeOffset time = DateTimeUtil.currentTime;
			if (!(GetRemainingTime(time) > 0f))
			{
				CartSynchronizer.Exec(m_cartInst, new SFAction<DateTimeOffset, bool>(ProcessLifetimeEnd, time, arg2: true));
			}
		}
	}

	public void ProcessLifetimeEnd(DateTimeOffset time, bool bSendFailEvent)
	{
		if (m_cartInst.isRiding)
		{
			m_cartInst.GetOff(time, bSendEvent: false);
			if (m_hero.currentPlace != null)
			{
				ServerEvent.SendHeroEnter(m_hero.currentPlace.GetDynamicClientPeers(m_hero.sector, m_hero.id), m_hero.ToPDHero(time), bIsRevivalEnter: false);
			}
		}
		((ContinentInstance)m_cartInst.currentPlace)?.ExitCart(m_cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(m_cartInst);
		Fail(time, bSendFailEvent);
	}

	public void Fail(DateTimeOffset time, bool bSendFailEvent)
	{
		m_hero.supplySupportQuest = null;
		m_cartInst = null;
		SupplySupportQuestOrder order = Resource.instance.supplySupportQuest.GetOrder(m_nOrderId);
		long lnFailRefundRewardGold = order.failRefundGoldRewardValue;
		if (lnFailRefundRewardGold > 0)
		{
			m_hero.AddGold(lnFailRefundRewardGold);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateSupplySupportQuest_Status(m_id, 2, time));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_hero));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSupplySupportQuestRewardLog(Guid.NewGuid(), m_hero.id, m_id, 2, m_cart.id, 0L, 0L, 0, 0, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		if (bSendFailEvent)
		{
			ServerEvent.SendSupplySupportQuestFail(m_hero.account.peer, m_hero.gold, m_hero.maxGold);
		}
		Release();
	}

	public void AddVisitedWayPoint(int nWayPoint)
	{
		m_visitedWayPoints.Add(nWayPoint);
	}

	public bool ContainsVisitedWayPoint(int nWayPoint)
	{
		return m_visitedWayPoints.Contains(nWayPoint);
	}

	public void RefreshCartInfo()
	{
		m_cart = null;
		m_nCartHp = 0;
		m_bIsCartRiding = false;
		m_nCartContinentId = 0;
		m_cartPosition = Vector3.zero;
		m_fCartYRotation = 0f;
		if (m_cartInst != null)
		{
			m_cart = m_supplySupportQuest.GetCart(m_cartInst.cart.id);
			m_nCartHp = m_cartInst.hp;
			m_bIsCartRiding = m_cartInst.isRiding;
			ContinentInstance currentPlace = (ContinentInstance)m_cartInst.currentPlace;
			if (currentPlace != null)
			{
				m_nCartContinentId = currentPlace.continent.id;
				m_cartPosition = m_cartInst.position;
				m_fCartYRotation = m_cartInst.rotationY;
			}
		}
	}

	private float GetElapsedTime(DateTimeOffset time)
	{
		return (float)(time - m_startTime).TotalSeconds;
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		return Math.Max(0f, (float)Resource.instance.supplySupportQuest.limitTime - GetElapsedTime(time));
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeUpdateTimer();
			m_bReleased = true;
		}
	}

	private void DisposeUpdateTimer()
	{
		if (m_updateTimer != null)
		{
			m_updateTimer.Dispose();
			m_updateTimer = null;
		}
	}

	public PDHeroSupplySupportQuest ToPDHeroSupplySupportQuest(DateTimeOffset time)
	{
		PDHeroSupplySupportQuest inst = new PDHeroSupplySupportQuest();
		inst.cartInstanceId = m_cartInst.instanceId;
		inst.cartContinentId = m_nCartContinentId;
		inst.cartPosition = m_cartPosition;
		inst.cartRotationY = m_fCartYRotation;
		inst.orderId = m_nOrderId;
		inst.cartId = m_cart.id;
		inst.remainingTime = GetRemainingTime(time);
		inst.visitedWayPoints = m_visitedWayPoints.ToArray();
		return inst;
	}
}
