using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TrueHeroQuestStep
{
	private TrueHeroQuest m_quest;

	private int m_nNo;

	private int m_nTargetContinentId;

	private Vector3 m_targetPosition = Vector3.zero;

	private int m_nObjectiveWaitingTime;

	private ItemReward m_itemReward;

	public TrueHeroQuest quest => m_quest;

	public int no => m_nNo;

	public int targetContinentId => m_nTargetContinentId;

	public Vector3 targetPosition => m_targetPosition;

	public int objectiveWaitingTime => m_nObjectiveWaitingTime;

	public ItemReward itemReward => m_itemReward;

	public TrueHeroQuestStep(TrueHeroQuest quest)
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
		m_nNo = Convert.ToInt32(dr["stepNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		if (Resource.instance.GetContinent(m_nTargetContinentId) == null)
		{
			SFLogUtil.Warn(GetType(), "목표대륙이 존재하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetContinentId = " + m_nTargetContinentId);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetObjectXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetObjectYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetObjectZPosition"]);
		m_nObjectiveWaitingTime = Convert.ToInt32(dr["objectiveWaitingTime"]);
		if (m_nObjectiveWaitingTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표대기시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nObjectiveWaitingTime = " + m_nObjectiveWaitingTime);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템 보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
	}

	public bool TargetAreaContains(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetPosition, m_quest.targetObjectInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}
}
