using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AccessRewardEntry
{
	private int m_nId;

	private int m_nAccessTime;

	private List<AccessRewardItem> m_rewardItems = new List<AccessRewardItem>();

	public int id => m_nId;

	public int accessTime => m_nAccessTime;

	public List<AccessRewardItem> rewardItems => m_rewardItems;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["entryId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "항목ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nAccessTime = Convert.ToInt32(dr["accessTime"]);
		if (m_nAccessTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "접속시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nAccessTime = " + m_nAccessTime);
		}
	}

	public void AddRewardItem(AccessRewardItem item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (item.entry != null)
		{
			throw new Exception("이미 접속보상항목에 추가된 접속보상아이템입니다.");
		}
		m_rewardItems.Add(item);
		item.entry = this;
	}
}
