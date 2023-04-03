using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GoldDungeonStep
{
	private GoldDungeonDifficulty m_difficulty;

	private int m_nNo;

	private GoldReward m_goldReward;

	private List<GoldDungeonStepWave> m_waves = new List<GoldDungeonStepWave>();

	private List<GoldDungeonStepMonsterArrange> m_monsterArranges = new List<GoldDungeonStepMonsterArrange>();

	public GoldDungeon goldDungeon => m_difficulty.goldDungeon;

	public GoldDungeonDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public GoldReward goldReward => m_goldReward;

	public List<GoldDungeonStepMonsterArrange> monsterArranges => m_monsterArranges;

	public int waveCount => m_waves.Count;

	public GoldDungeonStep(GoldDungeonDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["step"]);
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. difficulty = " + difficulty.difficulty + ", m_nNo = " + m_nNo);
			}
		}
		else if (lnGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. difficulty = " + difficulty.difficulty + ", m_nNO = " + m_nNo);
		}
	}

	public void AddWave(GoldDungeonStepWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public GoldDungeonStepWave GetWave(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public void AddMonsterArrange(GoldDungeonStepMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}
}
