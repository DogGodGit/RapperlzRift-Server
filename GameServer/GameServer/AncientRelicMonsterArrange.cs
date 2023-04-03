using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AncientRelicMonsterArrange
{
	public const int kType_NormalMonster = 1;

	public const int kType_BossMonster = 2;

	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private AncientRelicStepWave m_wave;

	private int m_nNo;

	private int m_nRouteId;

	private int m_nType;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterCount;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nPoint;

	public AncientRelic ancientRelic => m_wave.ancientRelic;

	public AncientRelicStep step => m_wave.step;

	public AncientRelicStepWave wave => m_wave;

	public int no => m_nNo;

	public int routeId => m_nRouteId;

	public int type => m_nType;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterCount => m_nMonsterCount;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int point => m_nPoint;

	public AncientRelicMonsterArrange(AncientRelicStepWave wave)
	{
		m_wave = wave;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		m_nRouteId = Convert.ToInt32(dr["routeId"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. stepNo = " + step.no + ", waveNo = " + wave.no + ", m_nType = " + m_nType);
		}
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. stepNo = " + step.no + ", waveNo = " + wave.no + ", m_nNo = " + m_nNo);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. stepNo = " + step.no + ", waveNo = " + wave.no + ", m_nNo = " + m_nNo);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. stepNo = " + step.no + ", waveNo = " + wave.no + ", m_nMonsterCount = " + m_nMonsterCount);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. stepNo = " + step.no + ", waveNo = " + wave.no + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
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
