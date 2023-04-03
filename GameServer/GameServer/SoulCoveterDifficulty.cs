using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SoulCoveterDifficulty
{
	private SoulCoveter m_soulCoveter;

	private int m_nDifficulty;

	private List<SoulCoveterDifficultyReward> m_rewards = new List<SoulCoveterDifficultyReward>();

	private List<SoulCoveterDifficultyWave> m_waves = new List<SoulCoveterDifficultyWave>();

	public SoulCoveter soulCoveter => m_soulCoveter;

	public List<SoulCoveterDifficultyReward> rewards => m_rewards;

	public int difficulty => m_nDifficulty;

	public int waveCount => m_waves.Count;

	public SoulCoveterDifficulty(SoulCoveter soulCoveter)
	{
		m_soulCoveter = soulCoveter;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		if (m_nDifficulty <= 0)
		{
			SFLogUtil.Warn(GetType(), "난이도가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty);
		}
	}

	public void AddReward(SoulCoveterDifficultyReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public void AddWave(SoulCoveterDifficultyWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public SoulCoveterDifficultyWave GetWave(int nWaveNo)
	{
		int nIndex = nWaveNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}
}
