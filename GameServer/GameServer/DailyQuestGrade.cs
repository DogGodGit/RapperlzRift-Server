using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DailyQuestGrade
{
	public const int kGrade_C = 1;

	public const int kGrade_B = 2;

	public const int kGrade_A = 3;

	public const int kGrade_S = 4;

	public const int kGrade_SS = 5;

	public const int kCount = 5;

	private int m_nGrade;

	private int m_nPoint;

	private int m_nImmediateCompletionRequiredGold;

	private int m_nAutoCompletionRequiredTime;

	private int m_nRewardVipPoint;

	private ItemReward m_itemReward;

	public int grade => m_nGrade;

	public int point => m_nPoint;

	public int immediateCompletionRequiredGold => m_nImmediateCompletionRequiredGold;

	public int autoCompletionRequiredTime => m_nAutoCompletionRequiredTime;

	public int rewardVipPoint => m_nRewardVipPoint;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (m_nGrade <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nGrade = " + m_nGrade);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nGrade = " + m_nGrade + ", m_nPoint = " + m_nPoint);
		}
		m_nImmediateCompletionRequiredGold = Convert.ToInt32(dr["immediateCompletionRequiredGold"]);
		if (m_nImmediateCompletionRequiredGold <= 0)
		{
			SFLogUtil.Warn(GetType(), "즉시완료필요골드가 유효하지 않습니다. m_nGrade = " + m_nGrade + ", m_nImmediateCompletionRequiredGold = " + m_nImmediateCompletionRequiredGold);
		}
		m_nAutoCompletionRequiredTime = Convert.ToInt32(dr["autoCompletionRequiredTime"]);
		if (m_nAutoCompletionRequiredTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "자동완료필요시간이 유효하지 않습니다. m_nGrade = " + m_nGrade + ", m_nAutoCompletionRequiredTime = " + m_nAutoCompletionRequiredTime);
		}
		m_nRewardVipPoint = Convert.ToInt32(dr["rewardVipPoint"]);
		if (m_nRewardVipPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "보상VIP포인트가 유효하지 않습니다. m_nGrade = " + m_nGrade + ", m_nRewardVipPoint = " + m_nRewardVipPoint);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상아이템이 유효하지 않습니다. m_nGrade = " + m_nGrade + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
	}
}
