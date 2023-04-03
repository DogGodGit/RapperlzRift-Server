using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class StoryDungeonDifficulty
{
	private StoryDungeon m_storyDungeon;

	private int m_nDifficulty;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private long m_lnRecommendBattlePower;

	private List<StoryDungeonReward> m_rewards = new List<StoryDungeonReward>();

	private List<StoryDungeonSweepReward> m_sweepRewards = new List<StoryDungeonSweepReward>();

	private List<StoryDungeonStep> m_steps = new List<StoryDungeonStep>();

	private List<StoryDungeonTrap> m_traps = new List<StoryDungeonTrap>();

	public StoryDungeon storyDungeon => m_storyDungeon;

	public int difficulty => m_nDifficulty;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public long recommendBattlePower => m_lnRecommendBattlePower;

	public int stepCount => m_steps.Count;

	public List<StoryDungeonReward> rewards => m_rewards;

	public List<StoryDungeonSweepReward> sweepRewards => m_sweepRewards;

	public List<StoryDungeonTrap> traps => m_traps;

	public StoryDungeonDifficulty(StoryDungeon storyDungeon)
	{
		m_storyDungeon = storyDungeon;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
	}

	public void AddReward(StoryDungeonReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public StoryDungeonReward GetReward(int nRewardNo)
	{
		int nIndex = nRewardNo - 1;
		if (nRewardNo < 0 || nRewardNo >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}

	public void AddSweepReward(StoryDungeonSweepReward sweepReward)
	{
		if (sweepReward == null)
		{
			throw new ArgumentNullException("sweepReward");
		}
		m_sweepRewards.Add(sweepReward);
	}

	public StoryDungeonSweepReward GetSweepReward(int nSweepRewardNo)
	{
		int nIndex = nSweepRewardNo - 1;
		if (nIndex < 0 || nIndex >= m_sweepRewards.Count)
		{
			return null;
		}
		return m_sweepRewards[nIndex];
	}

	public void AddStep(StoryDungeonStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public StoryDungeonStep GetStep(int nStepNo)
	{
		int nIndex = nStepNo - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public void AddTrap(StoryDungeonTrap trap)
	{
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		m_traps.Add(trap);
	}
}
