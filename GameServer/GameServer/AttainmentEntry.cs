using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AttainmentEntry
{
	private int m_nNo;

	private string m_sNameKey;

	private AttainmentEntryType m_type;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private List<AttainmentEntryReward> m_rewards = new List<AttainmentEntryReward>();

	public int no => m_nNo;

	public string nameKey => m_sNameKey;

	public AttainmentEntryType type => m_type;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public List<AttainmentEntryReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(AttainmentEntryType), nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (AttainmentEntryType)nType;
		if (m_type == AttainmentEntryType.HeroLevel)
		{
			m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
			if (m_nRequiredHeroLevel <= 0)
			{
				SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
			}
		}
		else
		{
			m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
			if (m_nRequiredMainQuestNo <= 0)
			{
				SFLogUtil.Warn(GetType(), "요구메인퀘스트번호가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
			}
		}
	}

	public void AddReward(AttainmentEntryReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}
}
