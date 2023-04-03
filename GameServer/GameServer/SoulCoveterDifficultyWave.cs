using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SoulCoveterDifficultyWave
{
	private SoulCoveterDifficulty m_difficulty;

	private int m_nNo;

	private int m_nTargetArrangeNo;

	private List<SoulCoveterMonsterArrange> m_monsterArranges = new List<SoulCoveterMonsterArrange>();

	public SoulCoveter soulCoveter => m_difficulty.soulCoveter;

	public SoulCoveterDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public int targetArrangeNo => m_nTargetArrangeNo;

	public List<SoulCoveterMonsterArrange> monsterArranges => m_monsterArranges;

	public SoulCoveterDifficultyWave(SoulCoveterDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		m_nTargetArrangeNo = Convert.ToInt32(dr["targetArrangeNo"]);
		if (m_nTargetArrangeNo < 0)
		{
			SFLogUtil.Warn(GetType(), "목표배치번호가 유효하지 않습니다. difficulty = " + difficulty.difficulty + ", m_nNo = " + m_nNo + ", m_nTargetArrangeNo = " + m_nTargetArrangeNo);
		}
	}

	public void AddMonsterArrange(SoulCoveterMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}
}
