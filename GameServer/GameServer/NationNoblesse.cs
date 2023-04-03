using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class NationNoblesse
{
	public const int kId_King = 1;

	public const int kId_Archduke = 2;

	public const int kId_Duke = 3;

	public const int kId_Marquis = 4;

	public const int kId_Count = 5;

	public const int kId_Viscount = 6;

	public const int kId_Baron = 7;

	public const int kId_Baronet = 8;

	private int m_nId;

	private bool m_bNationWarDeclarationEnabled;

	private bool m_bNationCallEnabled;

	private bool m_bNationWarCallEnabled;

	private bool m_bNationWarConvergingEnabled;

	private bool m_bNationAllianceEnabled;

	private List<NationNoblesseAttr> m_attrs = new List<NationNoblesseAttr>();

	private Dictionary<int, NationNoblesseAppointmentAuthority> m_appointmentAuthorities = new Dictionary<int, NationNoblesseAppointmentAuthority>();

	public int id => m_nId;

	public bool nationWarDeclarationEnabled => m_bNationWarDeclarationEnabled;

	public bool nationCallEnabled => m_bNationCallEnabled;

	public bool nationWarCallEnabled => m_bNationWarCallEnabled;

	public bool nationWarConvergingAttackEnabled => m_bNationWarConvergingEnabled;

	public bool nationAllianceEnabled => m_bNationAllianceEnabled;

	public List<NationNoblesseAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["noblesseId"]);
		m_bNationWarDeclarationEnabled = Convert.ToBoolean(dr["nationWarDeclarationEnabled"]);
		m_bNationCallEnabled = Convert.ToBoolean(dr["nationCallEnabled"]);
		m_bNationWarCallEnabled = Convert.ToBoolean(dr["nationWarCallEnabled"]);
		m_bNationWarConvergingEnabled = Convert.ToBoolean(dr["nationWarConvergingAttackEnabled"]);
		m_bNationAllianceEnabled = Convert.ToBoolean(dr["nationAllianceEnabled"]);
	}

	public void AddAttr(NationNoblesseAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
	}

	public void AddAppointmentAuthority(NationNoblesseAppointmentAuthority appointmentAuthority)
	{
		if (appointmentAuthority == null)
		{
			throw new ArgumentNullException("appointmentAuthority");
		}
		m_appointmentAuthorities.Add(appointmentAuthority.targetNoblesseId, appointmentAuthority);
	}

	public bool ContaninsAppointmentAuthority(int nTargetNoblesseId)
	{
		return m_appointmentAuthorities.ContainsKey(nTargetNoblesseId);
	}
}
