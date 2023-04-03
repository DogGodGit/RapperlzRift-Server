using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class OrdealQuestMission
{
	private OrdealQuestSlot m_slot;

	private int m_nNo;

	private OrdealQuestMissionType m_type = OrdealQuestMissionType.BountyHunterQuest;

	private int m_nTargetCount;

	private int m_nAutoCompletionRequiredTime;

	private ExpReward m_expReward;

	public OrdealQuestSlot slot => m_slot;

	public int no => m_nNo;

	public OrdealQuestMissionType type => m_type;

	public int targetCount => m_nTargetCount;

	public int autoCompletionRequiredTime => m_nAutoCompletionRequiredTime;

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

	public OrdealQuestMission(OrdealQuestSlot slot)
	{
		if (slot == null)
		{
			throw new ArgumentNullException("slot");
		}
		m_slot = slot;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["missionNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(OrdealQuestMissionType), nType))
		{
			SFLogUtil.Warn(GetType(), "시련퀘스트미션타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (OrdealQuestMissionType)nType;
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetCount = " + m_nTargetCount);
		}
		m_nAutoCompletionRequiredTime = Convert.ToInt32(dr["autoCompletionRequiredTime"]);
		if (m_nAutoCompletionRequiredTime < 0)
		{
			SFLogUtil.Warn(GetType(), "자동완성필요시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nAutoCompletionRequiredTime = " + m_nAutoCompletionRequiredTime);
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
}
