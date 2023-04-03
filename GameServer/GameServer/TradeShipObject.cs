using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TradeShipObject
{
	private TradeShipDifficulty m_difficulty;

	private int m_nId;

	private MonsterArrange m_monsterArrange;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private int m_nActivationStepNo;

	private int m_nPoint;

	private List<TradeShipObjectDestroyerReward> m_destroyerRewards = new List<TradeShipObjectDestroyerReward>();

	private int m_nDestroyerRewardTotalPoint;

	public TradeShipDifficulty difficulty => m_difficulty;

	public int id => m_nId;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public int activationStepNo => m_nActivationStepNo;

	public int point => m_nPoint;

	public TradeShipObject(TradeShipDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["objectId"]);
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nActivationStepNo = Convert.ToInt32(dr["activationStepNo"]);
		if (m_difficulty.tradeShip.GetStep(m_nActivationStepNo) == null)
		{
			SFLogUtil.Warn(GetType(), "활성단계번호가 유효하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nActivationStepNo = " + m_nActivationStepNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "점수가 유효하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nPoint = " + m_nPoint);
		}
	}

	public void AddDestroyerReward(TradeShipObjectDestroyerReward destroyerReward)
	{
		if (destroyerReward == null)
		{
			throw new ArgumentNullException("destroyerReward");
		}
		m_destroyerRewards.Add(destroyerReward);
		m_nDestroyerRewardTotalPoint += destroyerReward.point;
	}

	public TradeShipObjectDestroyerReward SelectDestroyerReward()
	{
		return Util.SelectPickEntry(m_destroyerRewards, m_nDestroyerRewardTotalPoint);
	}
}
