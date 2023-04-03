using System;
using ClientCommon;

namespace GameServer;

public class NationNoblesseInstance
{
	private NationInstance m_nationInst;

	private int m_nId;

	private Guid m_heroId = Guid.Empty;

	private string m_sHeroName;

	private int m_nHeroJobId;

	private DateTime m_appointmentDate = DateTime.MinValue;

	public NationInstance nationInst => m_nationInst;

	public int id => m_nId;

	public Guid heroId
	{
		get
		{
			return m_heroId;
		}
		set
		{
			m_heroId = value;
		}
	}

	public string heroName
	{
		get
		{
			return m_sHeroName;
		}
		set
		{
			m_sHeroName = value;
		}
	}

	public int heroJobId
	{
		get
		{
			return m_nHeroJobId;
		}
		set
		{
			m_nHeroJobId = value;
		}
	}

	public DateTime appointmentDate
	{
		get
		{
			return m_appointmentDate;
		}
		set
		{
			m_appointmentDate = value;
		}
	}

	public NationNoblesseInstance(NationInstance nationInst, int nNoblesseId)
	{
		m_nationInst = nationInst;
		m_nId = nNoblesseId;
	}

	public PDNationNoblesseInstance ToPDNationNoblesseInstance()
	{
		PDNationNoblesseInstance inst = new PDNationNoblesseInstance();
		inst.noblesseId = m_nId;
		inst.heroId = (Guid)m_heroId;
		inst.heroName = m_sHeroName;
		inst.heroJobId = m_nHeroJobId;
		inst.appointmentDate = (DateTime)m_appointmentDate;
		return inst;
	}
}
