using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestReward
{
	private int m_nLevel;

	private ExpReward m_missionCompletionExpReward;

	private ItemReward m_missionCompletionItemReward;

	private ItemReward m_questCompletionItemReward;

	public int level => m_nLevel;

	public ExpReward missionCompletionExpReward => m_missionCompletionExpReward;

	public long missionCompletionExpRewardValue
	{
		get
		{
			if (missionCompletionExpReward == null)
			{
				return 0L;
			}
			return m_missionCompletionExpReward.value;
		}
	}

	public ItemReward missionCompletionItemReward => m_missionCompletionItemReward;

	public ItemReward questCompletionItemReward => m_questCompletionItemReward;

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
		long lnMissionCompletionExpRewardId = Convert.ToInt64(dr["missionCompletionExpRewardId"]);
		m_missionCompletionExpReward = Resource.instance.GetExpReward(lnMissionCompletionExpRewardId);
		if (m_missionCompletionExpReward == null)
		{
			SFLogUtil.Warn(GetType(), "미션완료경험치보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnMissionCompletionExpRewardId = " + lnMissionCompletionExpRewardId);
		}
		long lnMissionCompletionItemRewardId = Convert.ToInt64(dr["missionCompletionItemRewardId"]);
		m_missionCompletionItemReward = Resource.instance.GetItemReward(lnMissionCompletionItemRewardId);
		if (m_missionCompletionItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "미션완료아이템보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnMissionCompletionItemRewardId = " + lnMissionCompletionItemRewardId);
		}
		long lnQuestCompletionItemRewardId = Convert.ToInt64(dr["questCompletionItemRewardId"]);
		m_questCompletionItemReward = Resource.instance.GetItemReward(lnQuestCompletionItemRewardId);
		if (m_questCompletionItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트완료아이템보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnQuestCompletionItemRewardId = " + lnQuestCompletionItemRewardId);
		}
	}
}
