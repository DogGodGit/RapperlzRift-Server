using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimObjectArrange : IPickEntry
{
	public const float kObjectInteractionMaxRangeFactor = 1.1f;

	private RuinsReclaimStep m_step;

	private int m_nNo;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nObjectInteractionDuration;

	private int m_nObjectInteractionMaxRange;

	private GoldReward m_goldReward;

	private ItemReward m_itemReward;

	public RuinsReclaimStep step => m_step;

	public int no => m_nNo;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int objectInteractionDuration => m_nObjectInteractionDuration;

	public int objectInteractionMaxRange => m_nObjectInteractionMaxRange;

	public GoldReward goldReward => m_goldReward;

	public ItemReward itemReward => m_itemReward;

	public int point => 1;

	int IPickEntry.point => point;

	public RuinsReclaimObjectArrange(RuinsReclaimStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_fRadius = " + m_fRadius);
		}
		m_nObjectInteractionDuration = Convert.ToInt32(dr["objectInteractionDuration"]);
		if (m_nObjectInteractionDuration < 0)
		{
			SFLogUtil.Warn(GetType(), "오브젝트상호작용시간이 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nObjectInteractionDuration = " + m_nObjectInteractionDuration);
		}
		if (m_nObjectInteractionMaxRange < 0)
		{
			SFLogUtil.Warn(GetType(), "오브젝트상호작용최대범위가 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nObjectInteractionMaxRange = " + m_nObjectInteractionMaxRange);
		}
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = res.GetGoldReward(lnGoldRewardId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
			}
		}
		else if (lnGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = res.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상ID가 존재하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
