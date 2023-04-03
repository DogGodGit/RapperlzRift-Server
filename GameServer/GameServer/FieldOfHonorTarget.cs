using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorTarget
{
	public const int kMaxSlotIndex = 5;

	private FieldOfHonor m_fieldOfHonor;

	private int m_nHighRanking;

	private int m_nLowRanking;

	private int m_nSlotIndex;

	private int m_nTargetHighRankingGap;

	private int m_nTargetLowRankingGap;

	public FieldOfHonor fieldOfHonor => m_fieldOfHonor;

	public int highRanking => m_nHighRanking;

	public int lowRanking => m_nLowRanking;

	public int slotIndex => m_nSlotIndex;

	public int targetHighRankingGap => m_nTargetHighRankingGap;

	public int targetLowRankingGap => m_nTargetLowRankingGap;

	public FieldOfHonorTarget(FieldOfHonor fieldOfHonor)
	{
		m_fieldOfHonor = fieldOfHonor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nHighRanking = Convert.ToInt32(dr["highRanking"]);
		m_nLowRanking = Convert.ToInt32(dr["lowRanking"]);
		if (m_nHighRanking > m_nLowRanking)
		{
			SFLogUtil.Warn(GetType(), "상위랭킹이 하위랭킹보다 순위가 낮습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking);
		}
		m_nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
		if (m_nSlotIndex <= 0 || m_nSlotIndex > 5)
		{
			SFLogUtil.Warn(GetType(), "상대슬롯인덱스가 유효하지 않습니다. m_nSlotIndex = " + m_nSlotIndex);
		}
		m_nTargetHighRankingGap = Convert.ToInt32(dr["targetHighRankingGap"]);
		m_nTargetLowRankingGap = Convert.ToInt32(dr["targetLowRankingGap"]);
		if (m_nTargetHighRankingGap > m_nTargetLowRankingGap)
		{
			SFLogUtil.Warn(GetType(), "대상상위랭킹차이가 대상하위랭킹차이보다 높습니다. m_nTargetHighRankingGap = " + m_nTargetHighRankingGap + ", m_nTargetLowRankingGap = " + m_nTargetLowRankingGap);
		}
	}
}
