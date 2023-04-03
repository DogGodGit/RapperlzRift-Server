using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SubQuestReward
{
	private SubQuest m_quest;

	private int m_nNo;

	private ItemReward m_itemReward;

	public SubQuest quest => m_quest;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public SubQuestReward(SubQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo < 0)
		{
			SFLogUtil.Warn(GetType(), "퀘스트번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
