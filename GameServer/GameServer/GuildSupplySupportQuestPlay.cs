using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildSupplySupportQuestPlay
{
	public const int kStatus_Accept = 0;

	public const int kStatus_Completed = 1;

	public const int kStatus_Fail = 2;

	private GuildSupplySupportQuest m_quest;

	private Guid m_id = Guid.Empty;

	private Guild m_guild;

	private Guid m_heroId = Guid.Empty;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private Cart m_cart;

	private int m_nCartHp;

	private bool m_bIsCartRiding;

	private int m_nCartContinentId;

	private Vector3 m_cartPosition = Vector3.zero;

	private float m_fCartYRotation;

	private GuildSupplySupportQuestCartInstance m_cartInst;

	public GuildSupplySupportQuest quest => m_quest;

	public Guid id => m_id;

	public Guild guild => m_guild;

	public Guid heroId => m_heroId;

	public DateTimeOffset startTime => m_startTime;

	public Cart cart => m_cart;

	public int cartHp => m_nCartHp;

	public bool isCartRiding => m_bIsCartRiding;

	public int cartContinentId => m_nCartContinentId;

	public Vector3 cartPosition => m_cartPosition;

	public float cartYRotation => m_fCartYRotation;

	public GuildSupplySupportQuestCartInstance cartInst
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

	public GuildSupplySupportQuestPlay(Guild guild)
	{
		m_quest = Resource.instance.guildSupplySupportQuest;
		m_guild = guild;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = SFDBUtil.ToGuid(dr["instanceId"]);
		m_heroId = SFDBUtil.ToGuid(dr["heroId"], Guid.Empty);
		m_startTime = SFDBUtil.ToDateTimeOffset(dr["regTime"], DateTimeOffset.MinValue);
		int nCartId = Convert.ToInt32(dr["cartId"]);
		if (nCartId > 0)
		{
			m_cart = Resource.instance.GetCart(nCartId);
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
	}

	public void Init(Guid heroId, DateTimeOffset time)
	{
		m_id = Guid.NewGuid();
		m_heroId = heroId;
		m_cart = m_quest.cart;
		m_startTime = time;
	}

	public void OnUpdate(DateTimeOffset time)
	{
		CheckLimitTime(time);
	}

	private void CheckLimitTime(DateTimeOffset time)
	{
		if (!(GetRemainingTime(time) > 0f))
		{
			CartSynchronizer.Exec(m_cartInst, new SFAction<DateTimeOffset, bool>(ProcessLifetimeEnd, time, arg2: true));
		}
	}

	public void ProcessLifetimeEnd(DateTimeOffset time, bool bSendFailEvent)
	{
		Hero hero = Cache.instance.GetLoggedInHero(m_heroId);
		if (hero != null)
		{
			if (m_cartInst.isRiding)
			{
				m_cartInst.GetOff(time, bSendEvent: false);
				if (hero.currentPlace != null)
				{
					ServerEvent.SendHeroEnter(hero.currentPlace.GetDynamicClientPeers(hero.sector, hero.id), hero.ToPDHero(time), bIsRevivalEnter: false);
				}
			}
			((ContinentInstance)m_cartInst.currentPlace)?.ExitCart(m_cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
			Cache.instance.RemoveCartInstance(m_cartInst);
		}
		Fail(time, bSendFailEvent);
	}

	public void Fail(DateTimeOffset time, bool bSendFailEvent)
	{
		m_guild.RemoveGuildSupplySupportQuest();
		m_cartInst = null;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuildSupplySupportQuest_Status(m_id, 2, time));
		dbWork.Schedule();
		m_guild.RefreshDailyGuildSupplySupportQuestStartCount(time.Date);
		_ = m_guild.dailyGuildSupplySupportQuestStartCount;
		if (bSendFailEvent)
		{
			Hero hero = Cache.instance.GetLoggedInHero(m_heroId);
			if (hero != null)
			{
				ServerEvent.SendGuildSupplySupportQuestFail(hero.account.peer);
			}
		}
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
			m_cart = quest.cart;
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
		return Math.Max(0f, (float)Resource.instance.guildSupplySupportQuest.limitTime - GetElapsedTime(time));
	}

	public PDGuildSupplySupportQuestPlay ToPDGuildSupplySupportQuestPlay(DateTimeOffset time)
	{
		PDGuildSupplySupportQuestPlay inst = new PDGuildSupplySupportQuestPlay();
		inst.cartInstanceId = m_cartInst.instanceId;
		inst.cartContinentId = m_nCartContinentId;
		inst.cartPosition = m_cartPosition;
		inst.cartRotationY = m_fCartYRotation;
		inst.remainingTime = GetRemainingTime(time);
		return inst;
	}
}
