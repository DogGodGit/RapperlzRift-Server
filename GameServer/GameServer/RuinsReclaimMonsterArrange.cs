using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	public const int kType_NormalMonster = 1;

	public const int kType_BossMonster = 2;

	private RuinsReclaim m_ruinsReclaim;

	private int m_nKey;

	private RuinsReclaimStepWave m_wave;

	private int m_nType;

	private MonsterArrange m_monsterArrange;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nMonsterCount;

	private float m_fSummonMinHpFactor;

	private List<RuinsReclaimSummonMonsterArrange> m_summonMonsterArranges = new List<RuinsReclaimSummonMonsterArrange>();

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int key => m_nKey;

	public RuinsReclaimStepWave wave => m_wave;

	public int type => m_nType;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int monsterCount => m_nMonsterCount;

	public float summonMinHpFactor => m_fSummonMinHpFactor;

	public List<RuinsReclaimSummonMonsterArrange> summonMonsterArranges => m_summonMonsterArranges;

	public RuinsReclaimMonsterArrange(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nKey = Convert.ToInt32(dr["arrangeKey"]);
		int nStepNo = Convert.ToInt32(dr["stepNo"]);
		RuinsReclaimStep step = null;
		if (nStepNo > 0)
		{
			step = m_ruinsReclaim.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "단계가 존재하지 않습니다. m_nKey = " + m_nKey + ", nStepNo = " + nStepNo);
			}
		}
		else if (nStepNo < 0)
		{
			SFLogUtil.Warn(GetType(), "단계번호가 유효하지 않습니다. m_nKey = " + m_nKey + ", nStepNo = " + nStepNo);
		}
		int nWaveNo = Convert.ToInt32(dr["waveNo"]);
		if (nWaveNo > 0)
		{
			if (step != null)
			{
				m_wave = step.GetWave(nWaveNo);
				if (m_wave == null)
				{
					SFLogUtil.Warn(GetType(), "웨이브가 존재하지 않습니다. m_nKey = " + m_nKey + ", nStepNo = " + nStepNo + ", nWaveNo = " + nWaveNo);
				}
			}
		}
		else if (nWaveNo < 0)
		{
			SFLogUtil.Warn(GetType(), "웨이브번호가 유효하지 않습니다. m_nKey = " + m_nKey + ", nStepNo = " + nStepNo + ", nWaveNo = " + nWaveNo);
		}
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nKey = " + m_nKey + ", m_nType = " + m_nType);
		}
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. m_nKey = " + m_nKey + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. m_nKey = " + m_nKey + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_position.x = Convert.ToInt32(dr["xPosition"]);
		m_position.y = Convert.ToInt32(dr["yPosition"]);
		m_position.z = Convert.ToInt32(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nKey = " + m_nKey + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. m_nKey = " + m_nKey + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. m_nMonsterCount = " + m_nMonsterCount);
		}
		m_fSummonMinHpFactor = Convert.ToSingle(dr["summonMinHpFactor"]);
		if (m_fSummonMinHpFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "소환최소HP계수가 유효하지 않습니다. m_nKey = " + m_nKey + ", m_fSummonMinHpFactor = " + m_fSummonMinHpFactor);
		}
	}

	public void AddSummonMonsterArrange(RuinsReclaimSummonMonsterArrange summonMonsterArrange)
	{
		if (summonMonsterArrange == null)
		{
			throw new ArgumentNullException("summonMonsterArrange");
		}
		m_summonMonsterArranges.Add(summonMonsterArrange);
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextInt(0.0, m_fYRotation);
		}
		return m_fYRotation;
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}

	public static bool IsDefinedYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
