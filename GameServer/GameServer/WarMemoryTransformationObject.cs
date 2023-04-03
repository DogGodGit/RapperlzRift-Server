using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WarMemoryTransformationObject
{
	public const float kObjectInteractionMaxRangeFactor = 1.1f;

	private WarMemoryWave m_wave;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private float m_fObjectInteractionDuration;

	private float m_fObjectInteractionMaxRange;

	private int m_nObjectLifetime;

	private Monster m_transformationMonster;

	private int m_nTransformationLifetime;

	private int m_nTransformationPoint;

	public WarMemoryWave wave => m_wave;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public float objectInteractionDuration => m_fObjectInteractionDuration;

	public float objectInteractionMaxRange => m_fObjectInteractionMaxRange;

	public int objectLifetime => m_nObjectLifetime;

	public Monster transformationMonster => m_transformationMonster;

	public int transformationLifetime => m_nTransformationLifetime;

	public int transformationPoint => m_nTransformationPoint;

	public WarMemoryTransformationObject(WarMemoryWave wave)
	{
		m_wave = wave;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["transformationObjectId"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_fObjectInteractionDuration = Convert.ToSingle(dr["objectInteractionDuration"]);
		if (m_fObjectInteractionDuration < 0f)
		{
			SFLogUtil.Warn(GetType(), "오브젝트상호작용시간 유효하지 않습니다. m_nId = " + m_nId + ", m_fObjectInteractionDuration = " + m_fObjectInteractionDuration);
		}
		m_fObjectInteractionMaxRange = Convert.ToSingle(dr["objectInteractionMaxRange"]);
		if (m_fObjectInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "오브젝트상호작용최대범위가 유효하지 않습니다. m_nId = " + m_nId + ", m_fObjectInteractionMaxRange = " + m_fObjectInteractionMaxRange);
		}
		m_nObjectLifetime = Convert.ToInt32(dr["objectLifetime"]);
		if (m_nObjectLifetime <= 0)
		{
			SFLogUtil.Warn(GetType(), "오브젝트유지기간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nObjectLifetime = " + m_nObjectLifetime);
		}
		int nTransformtaionMonsterId = Convert.ToInt32(dr["transformationMonsterId"]);
		if (nTransformtaionMonsterId > 0)
		{
			m_transformationMonster = Resource.instance.GetMonster(nTransformtaionMonsterId);
			if (m_transformationMonster == null)
			{
				SFLogUtil.Warn(GetType(), "변신몬스터가 존재하지 않습니다. m_nId = " + m_nId + ", nTransformtaionMonsterId = " + nTransformtaionMonsterId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "변신몬스터ID가 유효하지 않습니다. m_nId = " + m_nId + ", nTransformtaionMonsterId = " + nTransformtaionMonsterId);
		}
		m_nTransformationLifetime = Convert.ToInt32(dr["transformationLifetime"]);
		if (m_nTransformationLifetime <= 0)
		{
			SFLogUtil.Warn(GetType(), "변신유지기간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTransformationLifetime = " + m_nTransformationLifetime);
		}
		m_nTransformationPoint = Convert.ToInt32(dr["transformationPoint"]);
		if (m_nTransformationPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "변신점수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTransformationPoint = " + m_nTransformationPoint);
		}
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}
}
