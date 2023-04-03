using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobChangeQuestDifficulty
{
	private JobChangeQuest m_quest;

	private int m_nDifficulty;

	private bool m_bIsTargetPlaceGuildTerritory;

	public JobChangeQuest quest => m_quest;

	public int difficulty => m_nDifficulty;

	public bool isTargetPlaceGuildTerrtory => m_bIsTargetPlaceGuildTerritory;

	public JobChangeQuestDifficulty(JobChangeQuest quest)
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
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		if (m_nDifficulty <= 0)
		{
			SFLogUtil.Warn(GetType(), "난이도가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty);
		}
		m_bIsTargetPlaceGuildTerritory = Convert.ToBoolean(dr["isTargetPlaceGuildTerritory"]);
	}
}
