using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class OrdealQuest
{
	private int m_nNo;

	private int m_nRequiredHeroLevel;

	private ExpReward m_expReward;

	private OrdealQuestSlot[] m_slots = new OrdealQuestSlot[Resource.instance.ordealQuestSlotCount];

	public int no => m_nNo;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public ExpReward expReward => m_expReward;

	public long expRewardValue
	{
		get
		{
			if (m_expReward == null)
			{
				return 0L;
			}
			return m_expReward.value;
		}
	}

	public OrdealQuestSlot[] slots => m_slots;

	public OrdealQuest()
	{
		for (int i = 0; i < m_slots.Length; i++)
		{
			m_slots[i] = new OrdealQuestSlot(this, i);
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["questNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "퀘스트번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
	}

	public OrdealQuestSlot GetSlot(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_slots.Length)
		{
			return null;
		}
		return m_slots[nIndex];
	}
}
