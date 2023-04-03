using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StoryDungeonSweepReward
{
	private StoryDungeonDifficulty m_difficulty;

	private int m_nNo;

	private ItemReward m_itemReward;

	public StoryDungeon storyDungeon => m_difficulty.storyDungeon;

	public StoryDungeonDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public StoryDungeonSweepReward(StoryDungeonDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상아이템이 존재하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "보상아이템ID가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
