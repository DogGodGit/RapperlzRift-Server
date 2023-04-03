using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FishingQuestCastingReward
{
	private FishingQuestBait m_bait;

	private int m_nLevel;

	private ExpReward m_expReward;

	public FishingQuestBait bait => m_bait;

	public int level => m_nLevel;

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

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		long lnExpReward = Convert.ToInt64(dr["expRewardId"]);
		m_expReward = Resource.instance.GetExpReward(lnExpReward);
		if (m_expReward == null)
		{
			SFLogUtil.Warn(GetType(), "낚시퀘스트캐스팅경험치보상이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", lnExpReward = " + lnExpReward);
		}
	}
}
