using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AchievementReward
{
	private int m_nNo;

	private int m_nRequiredAchievementPoint;

	private List<AchievementRewardEntry> m_entries = new List<AchievementRewardEntry>();

	public int no => m_nNo;

	public int requiredAchievementPoint => m_nRequiredAchievementPoint;

	public List<AchievementRewardEntry> entries => m_entries;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다.");
		}
		m_nRequiredAchievementPoint = Convert.ToInt32(dr["requiredAchievementPoint"]);
		if (m_nRequiredAchievementPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구달성포인트가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredAchievementPoint = " + m_nRequiredAchievementPoint);
		}
	}

	public void AddEntry(AchievementRewardEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
	}
}
