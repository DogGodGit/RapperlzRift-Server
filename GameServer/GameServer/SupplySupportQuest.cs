using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuest
{
	private int m_nRequiredHeroLevel;

	private Npc m_startNpc;

	private Npc m_completionNpc;

	private int m_nLimitCount;

	private int m_nGuaranteeGold;

	private int m_nLimitTime;

	private Dictionary<int, SupplySupportQuestOrder> m_orders = new Dictionary<int, SupplySupportQuestOrder>();

	private List<SupplySupportQuestWayPoint> m_wayPoints = new List<SupplySupportQuestWayPoint>();

	private Dictionary<int, SupplySupportQuestCart> m_carts = new Dictionary<int, SupplySupportQuestCart>();

	private List<SupplySupportQuestReward> m_rewards = new List<SupplySupportQuestReward>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Npc startNpc => m_startNpc;

	public Npc completionNpc => m_completionNpc;

	public int limitCount => m_nLimitCount;

	public int GuaranteeGold => m_nGuaranteeGold;

	public int limitTime => m_nLimitTime;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		if (nStartNpcId > 0)
		{
			m_startNpc = Resource.instance.GetNpc(nStartNpcId);
			if (m_startNpc == null)
			{
				SFLogUtil.Warn(GetType(), "시작NPC가 존재하지 않습니다. nStartNpcId = " + nStartNpcId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "시작NPC ID가 유효하지 않습니다. nStartNpcId = " + nStartNpcId);
		}
		int nCompletionNpcId = Convert.ToInt32(dr["completionNpcId"]);
		if (nCompletionNpcId > 0)
		{
			m_completionNpc = Resource.instance.GetNpc(nCompletionNpcId);
			if (m_completionNpc == null)
			{
				SFLogUtil.Warn(GetType(), "완료NPC가 존재하지 않습니다. nCompletionNpcId = " + nCompletionNpcId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "완료NPC ID가 유효하지 않습니다. nCompletionNpcId = " + nCompletionNpcId);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "일일제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		m_nGuaranteeGold = Convert.ToInt32(dr["guaranteeGold"]);
		if (m_nGuaranteeGold < 0)
		{
			SFLogUtil.Warn(GetType(), "보증골드가 유효하지 않습니다. m_nGuaranteeGold = " + m_nGuaranteeGold);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
	}

	public void AddOrder(SupplySupportQuestOrder order)
	{
		if (order == null)
		{
			throw new ArgumentNullException("order");
		}
		m_orders.Add(order.id, order);
	}

	public SupplySupportQuestOrder GetOrder(int nOrderId)
	{
		if (!m_orders.TryGetValue(nOrderId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddWayPoint(SupplySupportQuestWayPoint wayPoint)
	{
		if (wayPoint == null)
		{
			throw new ArgumentNullException("wayPoint");
		}
		m_wayPoints.Add(wayPoint);
	}

	public SupplySupportQuestWayPoint GetWayPoint(int nWayPoint)
	{
		int nIndex = nWayPoint - 1;
		if (nIndex < 0 || nIndex >= m_wayPoints.Count)
		{
			return null;
		}
		return m_wayPoints[nIndex];
	}

	public void AddCart(SupplySupportQuestCart cart)
	{
		if (cart == null)
		{
			throw new ArgumentNullException("cart");
		}
		m_carts.Add(cart.id, cart);
	}

	public SupplySupportQuestCart GetCart(int nCartId)
	{
		if (!m_carts.TryGetValue(nCartId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddReward(SupplySupportQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public SupplySupportQuestReward GetReward(int nCartId, int nLevel)
	{
		foreach (SupplySupportQuestReward reward in m_rewards)
		{
			if (reward.cartId == nCartId && reward.level == nLevel)
			{
				return reward;
			}
		}
		return null;
	}
}
