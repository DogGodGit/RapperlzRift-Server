using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ConstellationEntry
{
	private ConstellationCycle m_cycle;

	private int m_nNo;

	private int m_nRequiredStarEssense;

	private long m_lnRequiredGold;

	private int m_nSuccessRate;

	private int m_nFailPoint;

	private List<ConstellationEntryBuff> m_buffs = new List<ConstellationEntryBuff>();

	public ConstellationCycle cycle => m_cycle;

	public int no => m_nNo;

	public int requiredStarEssense => m_nRequiredStarEssense;

	public long requiredGold => m_lnRequiredGold;

	public int successRate => m_nSuccessRate;

	public int failPoint => m_nFailPoint;

	public bool isLastEntry => m_nNo >= m_cycle.lastEntry;

	public List<ConstellationEntryBuff> buffs => m_buffs;

	public ConstellationEntry(ConstellationCycle cycle)
	{
		if (cycle == null)
		{
			throw new ArgumentNullException("cycle");
		}
		m_cycle = cycle;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nRequiredStarEssense = Convert.ToInt32(dr["requiredStarEssense"]);
		if (m_nRequiredStarEssense < 0)
		{
			SFLogUtil.Warn(GetType(), "필요별의정수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredStarEssense = " + m_nRequiredStarEssense);
		}
		m_lnRequiredGold = Convert.ToInt32(dr["requiredGold"]);
		if (m_lnRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "필요골드가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_lnRequiredGold = " + m_lnRequiredGold);
		}
		m_nSuccessRate = Convert.ToInt32(dr["successRate"]);
		if (m_nSuccessRate < 0)
		{
			SFLogUtil.Warn(GetType(), "성공확률이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nSuccessRate = " + m_nSuccessRate);
		}
		m_nFailPoint = Convert.ToInt32(dr["failPoint"]);
		if (m_nFailPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "실패포인트가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nFailPoint = " + m_nFailPoint);
		}
	}

	public void AddBuff(ConstellationEntryBuff buff)
	{
		if (buff == null)
		{
			throw new ArgumentNullException("buff");
		}
		m_buffs.Add(buff);
	}
}
