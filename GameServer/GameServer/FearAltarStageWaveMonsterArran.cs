using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltarStageWaveMonsterArrange
{
	public const int kMonsterType_Normal = 1;

	public const int kMonsterType_Boss = 2;

	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private FearAltar m_fearAltar;

	private int m_nKey;

	private FearAltarStageWave m_wave;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterType;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nMonsterCount;

	public FearAltar fearAltar => m_fearAltar;

	public int key => m_nKey;

	public FearAltarStageWave wave => m_wave;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterType => m_nMonsterType;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int monsterCount => m_nMonsterCount;

	public FearAltarStageWaveMonsterArrange(FearAltar fearAltar)
	{
		m_fearAltar = fearAltar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nKey = Convert.ToInt32(dr["arrangeKey"]);
		int nStageId = Convert.ToInt32(dr["stageId"]);
		FearAltarStage stage = null;
		if (nStageId > 0)
		{
			stage = m_fearAltar.GetStage(nStageId);
			if (stage == null)
			{
				SFLogUtil.Warn(GetType(), "스테이지가 존재하지 않습니다. m_nKey = " + m_nKey + ", nStageId = " + nStageId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "스테이지ID가 유효하지 않습니다. m_nKey = " + m_nKey + ", nStageId = " + nStageId);
		}
		int nWaveNo = Convert.ToInt32(dr["waveNo"]);
		if (nWaveNo > 0)
		{
			if (stage != null)
			{
				m_wave = stage.GetWave(nWaveNo);
				if (m_wave == null)
				{
					SFLogUtil.Warn(GetType(), "웨이브가 존재하지 않습니다. m_nKey = " + m_nKey + ", nWaveNo = " + nWaveNo);
				}
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "웨이브번호가 유효하지 않습니다. m_nKey = " + m_nKey + ", nWaveNo = " + nWaveNo);
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
		m_nMonsterType = Convert.ToInt32(dr["monsterType"]);
		if (!IsDefinedMonsterType(m_nMonsterType))
		{
			SFLogUtil.Warn(GetType(), "몬스터타입이 유효하지 않습니다. m_nKey = " + m_nKey + ", m_nMonsterType = " + m_nMonsterType);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nKey = " + m_nKey + ", m_fRadius = " + m_fRadius);
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
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. m_nKey = " + m_nKey + ", m_nMonsterCount = " + m_nMonsterCount);
		}
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public static bool IsDefinedMonsterType(int nType)
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
