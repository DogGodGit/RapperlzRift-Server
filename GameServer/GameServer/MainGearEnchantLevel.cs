using System;
using System.Data;

namespace GameServer;

public class MainGearEnchantLevel
{
	private int m_nEnchantLevel;

	private MainGearEnchantStep m_step;

	private int m_nNextSusscessRate;

	private bool m_bPenaltyPreventEnabled;

	public int enchantLevel => m_nEnchantLevel;

	public MainGearEnchantStep step
	{
		get
		{
			return m_step;
		}
		set
		{
			m_step = value;
		}
	}

	public int nextSusscessRate => m_nNextSusscessRate;

	public bool penaltyPreventEnabled => m_bPenaltyPreventEnabled;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nEnchantLevel = Convert.ToInt32(dr["enchantLevel"]);
		m_nNextSusscessRate = Convert.ToInt32(dr["nextSuccessRate"]);
		m_bPenaltyPreventEnabled = Convert.ToBoolean(dr["penaltyPreventEnabled"]);
	}
}
