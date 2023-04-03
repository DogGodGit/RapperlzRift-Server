using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SystemNationWarDeclaration
{
	private NationWar m_nationWar;

	private int m_nServerOpenDayCount;

	private Nation m_offenseNation;

	private Nation m_defenseNation;

	public NationWar nationWar => m_nationWar;

	public int serverOpenDayCount => m_nServerOpenDayCount;

	public Nation offenseNation => m_offenseNation;

	public Nation defenseNation => m_defenseNation;

	public SystemNationWarDeclaration(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nServerOpenDayCount = Convert.ToInt32(dr["serverOpenDayCount"]);
		if (m_nServerOpenDayCount < 0)
		{
			SFLogUtil.Warn(GetType(), "서버오픈일수가 유효하지 않습니다. m_nServerOpenDayCount = " + m_nServerOpenDayCount);
		}
		int nOffenseNationId = Convert.ToInt32(dr["offenseNationId"]);
		if (nOffenseNationId > 0)
		{
			m_offenseNation = res.GetNation(nOffenseNationId);
			if (m_offenseNation == null)
			{
				SFLogUtil.Warn(GetType(), "공격국가가 존재하지 않습니다. m_nServerOpenDayCount = " + m_nServerOpenDayCount + ", nOffenseNationId = " + nOffenseNationId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "공격국가ID가 유효하지 않습니다. m_nServerOpenDayCount = " + m_nServerOpenDayCount + ", nOffenseNationId = " + nOffenseNationId);
		}
		int nDefenseNationId = Convert.ToInt32(dr["defenseNationId"]);
		if (nDefenseNationId > 0)
		{
			m_defenseNation = res.GetNation(nDefenseNationId);
			if (m_defenseNation == null)
			{
				SFLogUtil.Warn(GetType(), "수비국가가 존재하지 않습니다. m_nServerOpenDayCount = " + m_nServerOpenDayCount + ", nDefenseNationId = " + nDefenseNationId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "수비국가ID가 유효하지 않습니다. m_nServerOpenDayCount = " + m_nServerOpenDayCount + ", nDefenseNationId = " + nDefenseNationId);
		}
		if (nOffenseNationId == nDefenseNationId)
		{
			SFLogUtil.Warn(GetType(), "공격국가ID와 수비국가ID가 같습니다. m_nServerOpenDayCount = " + m_nServerOpenDayCount + ", nOffenseNationId = " + nOffenseNationId + ", nDefenseNationId = " + nDefenseNationId);
		}
	}
}
