using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ConstellationCycle
{
	private ConstellationStep m_step;

	private int m_nCycle;

	private List<ConstellationCycleBuff> m_buffs = new List<ConstellationCycleBuff>();

	private Dictionary<int, ConstellationEntry> m_entries = new Dictionary<int, ConstellationEntry>();

	public ConstellationStep step => m_step;

	public int cycle => m_nCycle;

	public bool isLastCycle => m_nCycle >= m_step.lastCycle;

	public List<ConstellationCycleBuff> buffs => m_buffs;

	public Dictionary<int, ConstellationEntry> entries => m_entries;

	public int lastEntry => m_entries.Count;

	public ConstellationCycle(ConstellationStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nCycle = Convert.ToInt32(dr["cycle"]);
		if (m_nCycle <= 0)
		{
			SFLogUtil.Warn(GetType(), "사이클이 유효하지 않습니다. m_nCycle = " + m_nCycle);
		}
	}

	public void AddBuff(ConstellationCycleBuff buff)
	{
		if (buff == null)
		{
			throw new ArgumentNullException("buff");
		}
		m_buffs.Add(buff);
	}

	public void AddEntry(ConstellationEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry.no, entry);
	}

	public ConstellationEntry GetEntry(int nNo)
	{
		if (!m_entries.TryGetValue(nNo, out var value))
		{
			return null;
		}
		return value;
	}
}
