using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class LevelUpRewardEntry
{
	private int m_nId;

	private int m_nLevel;

	private List<LevelUpRewardItem> m_rewardItems = new List<LevelUpRewardItem>();

	public int id => m_nId;

	public int level => m_nLevel;

	public List<LevelUpRewardItem> rewardItems => m_rewardItems;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["entryId"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
	}

	public void AddRewardItem(LevelUpRewardItem item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (item.entry != null)
		{
			throw new Exception("이미 레벨업보상항목에 추가된 레벨업보상아이템입니다.");
		}
		m_rewardItems.Add(item);
		item.entry = this;
	}
}
