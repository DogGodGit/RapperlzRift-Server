using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DimensionRaidQuestStep
{
	private int m_nStep;

	private Npc m_targetNpc;

	public int step => m_nStep;

	public Npc targetNpc => m_targetNpc;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nStep = " + m_nStep);
		}
		int nTargetNpcId = Convert.ToInt32(dr["targetNpcId"]);
		m_targetNpc = Resource.instance.GetNpc(nTargetNpcId);
		if (m_targetNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다. m_nStep = " + m_nStep + ", nTargetNpcId = " + nTargetNpcId);
		}
	}
}
