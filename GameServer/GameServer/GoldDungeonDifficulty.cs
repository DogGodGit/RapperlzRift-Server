using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GoldDungeonDifficulty
{
	private GoldDungeon m_goldDungeon;

	private int m_nDifficulty;

	private int m_nRequiredHeroLevel;

	private GoldReward m_goldReward;

	private List<GoldDungeonStep> m_steps = new List<GoldDungeonStep>();

	public GoldDungeon goldDungeon => m_goldDungeon;

	public int difficulty => m_nDifficulty;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public GoldReward goldReward => m_goldReward;

	public int stepCount => m_steps.Count;

	public GoldDungeonDifficulty(GoldDungeon goldDungeon)
	{
		m_goldDungeon = goldDungeon;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
		if (m_goldReward == null)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnGoldRewardId = " + lnGoldRewardId);
		}
	}

	public void AddStep(GoldDungeonStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public GoldDungeonStep GetStep(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}
}
