using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NpcShop
{
	private int m_nId;

	private Npc m_npc;

	private Dictionary<int, NpcShopCategory> m_categories = new Dictionary<int, NpcShopCategory>();

	public int id => m_nId;

	public Npc npc => m_npc;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["shopId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "상점ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nNpcId = Convert.ToInt32(dr["npcId"]);
		m_npc = Resource.instance.GetNpc(nNpcId);
		if (m_npc == null)
		{
			SFLogUtil.Warn(GetType(), "NPC가 존재하지 않습니다. m_nId = " + m_nId + ", nNpcId = " + nNpcId);
		}
	}

	public void AddCategory(NpcShopCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("category");
		}
		m_categories.Add(category.id, category);
	}

	public NpcShopCategory GetCategory(int nId)
	{
		if (!m_categories.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
