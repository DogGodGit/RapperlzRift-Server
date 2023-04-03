using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestWayPoint
{
	private SupplySupportQuest m_supplySupportQuest;

	private int m_nWayPoint;

	private Npc m_cartChangeNpc;

	public SupplySupportQuest supplySupportQuest => m_supplySupportQuest;

	public int wayPoint => m_nWayPoint;

	public Npc cartChangeNpc => m_cartChangeNpc;

	public SupplySupportQuestWayPoint(SupplySupportQuest supplySupportQuest)
	{
		m_supplySupportQuest = supplySupportQuest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nWayPoint = Convert.ToInt32(dr["wayPoint"]);
		int nCartChangeNpcId = Convert.ToInt32(dr["cartChangeNpcId"]);
		if (nCartChangeNpcId > 0)
		{
			m_cartChangeNpc = Resource.instance.GetNpc(nCartChangeNpcId);
			if (m_cartChangeNpc == null)
			{
				SFLogUtil.Warn(GetType(), "수레변경NPC가 존재하지 않습니다. m_nWayPoint = " + m_nWayPoint);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "수레변경NPC ID가 유효하지 않습니다. m_nWayPoint = " + m_nWayPoint);
		}
	}
}
