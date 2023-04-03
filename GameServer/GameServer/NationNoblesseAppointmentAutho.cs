using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationNoblesseAppointmentAuthority
{
	private NationNoblesse m_nationNoblesse;

	private int m_nTargetNoblesseId;

	public NationNoblesse nationNoblesse => m_nationNoblesse;

	public int targetNoblesseId => m_nTargetNoblesseId;

	public NationNoblesseAppointmentAuthority(NationNoblesse nationNoblesse)
	{
		m_nationNoblesse = nationNoblesse;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nNoblesseId = Convert.ToInt32(dr["noblesseId"]);
		if (nNoblesseId > 0)
		{
			m_nationNoblesse = Resource.instance.GetNationNoblesse(nNoblesseId);
			if (m_nationNoblesse == null)
			{
				SFLogUtil.Warn(GetType(), "관직이 존재하지 않습니다. nNoblesseId = " + nNoblesseId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "관직ID가 유효하지 않습니다. nNoblesseId = " + nNoblesseId);
		}
		m_nTargetNoblesseId = Convert.ToInt32(dr["targetNoblesseId"]);
		if (m_nTargetNoblesseId > 0)
		{
			if (Resource.instance.GetNationNoblesse(m_nTargetNoblesseId) == null)
			{
				SFLogUtil.Warn(GetType(), "대상관직이 존재하지 않습니다. m_nTargetNoblesseId = " + m_nTargetNoblesseId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대상관직ID가 유효하지 않습니다. m_nTargetNoblesseId = " + m_nTargetNoblesseId);
		}
	}
}
