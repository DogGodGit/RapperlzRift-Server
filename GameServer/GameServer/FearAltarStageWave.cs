using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltarStageWave
{
	public const int kType_AllMonster = 1;

	public const int kType_TargetMonster = 2;

	public const int kHalidomMonsterYRotationType_Fixed = 1;

	public const int kHalidomMonsterYRotationType_Random = 2;

	private FearAltarStage m_stage;

	private int m_nNo;

	private int m_nType;

	private int m_nTargetArrangeKey;

	private int m_nHalidomMonsterSpawnRate;

	private Vector3 m_halidomMonsterPosition = Vector3.zero;

	private int m_nHalidomMonsterYRotationType;

	private float m_fHalidomMonsterYRotation;

	private List<FearAltarStageWaveMonsterArrange> m_monsterArranges = new List<FearAltarStageWaveMonsterArrange>();

	public FearAltarStage stage => m_stage;

	public int no => m_nNo;

	public int type => m_nType;

	public int targetArrangeKey => m_nTargetArrangeKey;

	public int halidomMonsterSpawnRate => m_nHalidomMonsterSpawnRate;

	public Vector3 halidomMonsterPosition => m_halidomMonsterPosition;

	public int halidomMonsterYRotationType => m_nHalidomMonsterYRotationType;

	public float halidomMonsterYRotation => m_fHalidomMonsterYRotation;

	public List<FearAltarStageWaveMonsterArrange> monsterArranges => m_monsterArranges;

	public FearAltarStageWave(FearAltarStage stage)
	{
		m_stage = stage;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		m_nTargetArrangeKey = Convert.ToInt32(dr["targetArrangeKey"]);
		if (m_nTargetArrangeKey < 0)
		{
			SFLogUtil.Warn(GetType(), "대상배치키가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetArrangeKey = " + m_nTargetArrangeKey);
		}
		m_nHalidomMonsterSpawnRate = Convert.ToInt32(dr["halidomMonsterSpawnRate"]);
		m_halidomMonsterPosition.x = Convert.ToSingle(dr["halidomMonsterXPosition"]);
		m_halidomMonsterPosition.y = Convert.ToSingle(dr["halidomMonsterYPosition"]);
		m_halidomMonsterPosition.z = Convert.ToSingle(dr["halidomMonsterZPosition"]);
		m_nHalidomMonsterYRotationType = Convert.ToInt32(dr["halidomMonsterYRotationType"]);
		if (!IsDefinedHalidomMonsterYRotationType(m_nHalidomMonsterYRotationType))
		{
			SFLogUtil.Warn(GetType(), "성물몬스터방향타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nHalidomMonsterYRotationType = " + m_nHalidomMonsterYRotationType);
		}
		m_fHalidomMonsterYRotation = Convert.ToSingle(dr["halidomMonsterYRotation"]);
	}

	public float SelectHalidomMonsterRotationY()
	{
		if (m_nHalidomMonsterYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fHalidomMonsterYRotation);
		}
		return m_fHalidomMonsterYRotation;
	}

	public void AddMonsterArrange(FearAltarStageWaveMonsterArrange monsterArrange)
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

	public static bool IsDefinedHalidomMonsterYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
