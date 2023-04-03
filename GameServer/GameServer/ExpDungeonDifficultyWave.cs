using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ExpDungeonDifficultyWave
{
	public const int kLakChargeMonsterYRotationType_Fixed = 1;

	public const int kLakChargeMonsterYRotationType_Random = 2;

	private ExpDungeonDifficulty m_difficulty;

	private int m_nNo;

	private int m_nWaveLimitTime;

	private int m_nLakChargeMonsterRate;

	private int m_nLakChargeAmount;

	private MonsterArrange m_lakChargeMonsterArrange;

	private Vector3 m_lakChargeMonsterPosition = Vector3.zero;

	private int m_nLakChargeMonsterYRotationType;

	private float m_fLakChargeMonsterYRotation;

	private List<ExpDungeonMonsterArrange> m_monsterArranges = new List<ExpDungeonMonsterArrange>();

	public ExpDungeon expDungeon => m_difficulty.expDungeon;

	public ExpDungeonDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public int waveLimitTime => m_nWaveLimitTime;

	public int lakChargeMonsterRate => m_nLakChargeMonsterRate;

	public int lakChargeAmount => m_nLakChargeAmount;

	public MonsterArrange lakChargeMonsterArrange => m_lakChargeMonsterArrange;

	public Vector3 lakChargeMonsterPosition => m_lakChargeMonsterPosition;

	public int lakChargeMonsterYRotationType => m_nLakChargeMonsterYRotationType;

	public float lakChargeMonsterYRotation => m_fLakChargeMonsterYRotation;

	public List<ExpDungeonMonsterArrange> monsterArranges => m_monsterArranges;

	public ExpDungeonDifficultyWave(ExpDungeonDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		m_nWaveLimitTime = Convert.ToInt32(dr["waveLimitTime"]);
		if (m_nWaveLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "웨이브제한시간이 유효하지 않습니다. difficuty = " + difficulty.difficulty + ", m_nNo = " + m_nNo + ", m_nWaveLimitTime = " + m_nWaveLimitTime);
		}
		m_nLakChargeMonsterRate = Convert.ToInt32(dr["lakChargeMonsterRate"]);
		m_nLakChargeAmount = Convert.ToInt32(dr["lakChargeAmount"]);
		long lnLakChargeMonsterArrangeId = Convert.ToInt64(dr["lakChargeMonsterArrangeId"]);
		if (lnLakChargeMonsterArrangeId > 0)
		{
			m_lakChargeMonsterArrange = Resource.instance.GetMonsterArrange(lnLakChargeMonsterArrangeId);
			if (m_lakChargeMonsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "라크충전몬스터배치가 존재하지 않습니다. difficuty = " + difficulty.difficulty + ", m_nNo = " + m_nNo + ", lnLakChargeMonsterArrangeId = " + lnLakChargeMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "라크충전몬스터배치ID가 유효하지 않습니다. difficuty = " + difficulty.difficulty + ", m_nNo = " + m_nNo + ", lnLakChargeMonsterArrangeId = " + lnLakChargeMonsterArrangeId);
		}
		m_lakChargeMonsterPosition.x = Convert.ToSingle(dr["lakChargeMonsterXPosition"]);
		m_lakChargeMonsterPosition.y = Convert.ToSingle(dr["lakChargeMonsterYPosition"]);
		m_lakChargeMonsterPosition.z = Convert.ToSingle(dr["lakchargeMonsterZPosition"]);
		m_nLakChargeMonsterYRotationType = Convert.ToInt32(dr["lakChargeMonsterYRotationType"]);
		if (m_nLakChargeMonsterYRotationType < 1 || m_nLakChargeMonsterYRotationType > 2)
		{
			SFLogUtil.Warn(GetType(), "라크충전몬스터방향타입이 유효하지 않습니다. difficuty = " + difficulty.difficulty + ", m_nNo = " + m_nNo + ", m_nLakChargeMonsterYRotationType = " + m_nLakChargeMonsterYRotationType);
		}
		m_fLakChargeMonsterYRotation = Convert.ToSingle(dr["lakChargeMonsterYRotation"]);
	}

	public void AddMonsterArrange(ExpDungeonMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public float SelectRotationY()
	{
		if (m_nLakChargeMonsterYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fLakChargeMonsterYRotation);
		}
		return m_fLakChargeMonsterYRotation;
	}
}
