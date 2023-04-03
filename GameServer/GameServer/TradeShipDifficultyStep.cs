using System;
using System.Collections.Generic;

namespace GameServer;

public class TradeShipDifficultyStep
{
	private TradeShipDifficulty m_difficulty;

	private int m_nNo;

	private List<TradeShipMonsterArrange> m_monsterArranges = new List<TradeShipMonsterArrange>();

	private List<TradeShipAdditionalMonsterArrangePoolEntry> m_additionalMonsterArrangePoolEntries = new List<TradeShipAdditionalMonsterArrangePoolEntry>();

	private int m_nAdditionalMonsterArrangePoolEntryTotalPoint;

	public TradeShipDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public List<TradeShipMonsterArrange> monsterArranges => m_monsterArranges;

	public TradeShipDifficultyStep(TradeShipDifficulty difficulty, int nStep)
	{
		m_difficulty = difficulty;
		m_nNo = nStep;
	}

	public void AddMonsterArrange(TradeShipMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public void AddAdditionalMonsterArrangePoolEntry(TradeShipAdditionalMonsterArrangePoolEntry additionalMonsterArrangePoolEntry)
	{
		if (additionalMonsterArrangePoolEntry == null)
		{
			throw new ArgumentNullException("additionalMonsterArrangePoolEntry");
		}
		m_additionalMonsterArrangePoolEntries.Add(additionalMonsterArrangePoolEntry);
		m_nAdditionalMonsterArrangePoolEntryTotalPoint = additionalMonsterArrangePoolEntry.point;
	}

	public TradeShipAdditionalMonsterArrangePoolEntry GetAdditionalMonsterArrangePoolEntry(int nAdditionalMonsterArrangePoolEntryNo)
	{
		int nIndex = nAdditionalMonsterArrangePoolEntryNo - 1;
		if (nIndex < 0 || nIndex >= m_additionalMonsterArrangePoolEntries.Count)
		{
			return null;
		}
		return m_additionalMonsterArrangePoolEntries[nIndex];
	}

	public TradeShipAdditionalMonsterArrangePoolEntry SelectAdditionalMonsterArrangePoolEntry()
	{
		return Util.SelectPickEntry(m_additionalMonsterArrangePoolEntries, m_nAdditionalMonsterArrangePoolEntryTotalPoint);
	}
}
