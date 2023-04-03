using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DragonNestStep
{
	public const int kType_Move = 1;

	public const int kType_AllMonsterKill = 2;

	private DragonNest m_dragonNest;

	private int m_nNo;

	private int m_nType;

	private int m_nStartDelayTime;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private List<DragonNestStepReward> m_rewards = new List<DragonNestStepReward>();

	private List<DragonNestMonsterArrange> m_monsterArranges = new List<DragonNestMonsterArrange>();

	public DragonNest dragonNest => m_dragonNest;

	public int no => m_nNo;

	public int type => m_nType;

	public int startDelayTime => m_nStartDelayTime;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public List<DragonNestStepReward> rewards => m_rewards;

	public List<DragonNestMonsterArrange> monsterArranges => m_monsterArranges;

	public DragonNestStep(DragonNest dragonNest)
	{
		m_dragonNest = dragonNest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["stepNo"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		if (m_nStartDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "목표반지름이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_fTargetRadius = " + m_fTargetRadius);
		}
	}

	public bool ContainsTargetPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius, position);
	}

	public void AddReward(DragonNestStepReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public void AddMonsterArrange(DragonNestMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
