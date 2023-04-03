using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarRevivalPointActivationCondition
{
	private NationWarRevivalPoint m_revivalPoint;

	private NationWarMonsterArrange m_monsterArrange;

	public NationWar nationWar => m_revivalPoint.nationWar;

	public NationWarRevivalPoint revivalPoint => m_revivalPoint;

	public NationWarMonsterArrange monsterArrange => m_monsterArrange;

	public NationWarRevivalPointActivationCondition(NationWarRevivalPoint revivalPoint)
	{
		m_revivalPoint = revivalPoint;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nArrangeId = Convert.ToInt32(dr["arrangeId"]);
		if (nArrangeId > 0)
		{
			m_monsterArrange = nationWar.GetMonsterArrange(nArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "배치ID가 존재하지 않습니다. revivalPointId = " + m_revivalPoint.id + ", nArrangeId = " + nArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "배치ID가 유효하지 않습니다. revivalPointId = " + m_revivalPoint.id + ", nArrangeId = " + nArrangeId);
		}
	}
}
