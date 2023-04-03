using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SceneryQuest
{
	public const float kRadiusFactor = 1.3f;

	private int m_nId;

	private int m_nContinentId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nWaitingTime;

	private ItemReward m_itemReward;

	public int id => m_nId;

	public int continentId => m_nContinentId;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int waitingTime => m_nWaitingTime;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["questId"]);
		m_nContinentId = Convert.ToInt32(dr["continentId"]);
		if (m_nContinentId > 0)
		{
			if (Resource.instance.GetContinent(m_nContinentId) == null)
			{
				SFLogUtil.Warn(GetType(), "대륙이 존재하지 않습니다. m_nId = " + m_nId + ", m_nContinentId = " + m_nContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대륙ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nContinentId = " + m_nContinentId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nWaitingTime = Convert.ToInt32(dr["waitingTime"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnItemRewardId = " + lnItemRewardId);
		}
	}

	public bool IsSceneryQuestAreaPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fRadius * 1.3f, position);
	}
}
