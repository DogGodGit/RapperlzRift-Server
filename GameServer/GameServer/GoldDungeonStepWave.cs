using System;
using System.Data;

namespace GameServer;

public class GoldDungeonStepWave
{
	private GoldDungeonStep m_step;

	private int m_nNo;

	private int m_nLimitTime;

	private int m_nNextWaveIntervalTime;

	public GoldDungeon goldDungeon => m_step.goldDungeon;

	public GoldDungeonDifficulty difficulty => m_step.difficulty;

	public GoldDungeonStep step => m_step;

	public int no => m_nNo;

	public int limitTime => m_nLimitTime;

	public int nextWaveIntervalTime => m_nNextWaveIntervalTime;

	public GoldDungeonStepWave(GoldDungeonStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		m_nNextWaveIntervalTime = Convert.ToInt32(dr["nextWaveIntervalTime"]);
	}
}
