using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimStepWaveSkill
{
	public const float kObjectInteractionMaxRangeFactor = 1.1f;

	private RuinsReclaimStepWave m_wave;

	private int m_nCastingInterval;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private Monster m_transformationMonster;

	private int m_nTransformationLifetime;

	private float m_fObjectInteractionDuration;

	private float m_fObjectInteractionMaxRange;

	private int m_nObjectLifetime;

	public RuinsReclaimStepWave wave => m_wave;

	public int CastingInteraval => m_nCastingInterval;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public Monster transformationMonster => m_transformationMonster;

	public int transformationLifetime => m_nTransformationLifetime;

	public float objectInteractionDuration => m_fObjectInteractionDuration;

	public float objectInteractionMaxRange => m_fObjectInteractionMaxRange;

	public int objectLifetime => m_nObjectLifetime;

	public RuinsReclaimStepWaveSkill(RuinsReclaimStepWave wave)
	{
		m_wave = wave;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nCastingInterval = Convert.ToInt32(dr["castingInterval"]);
		if (m_nCastingInterval <= 0)
		{
			SFLogUtil.Warn(GetType(), "시전간격이 유효하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", m_nCastingInterval = " + m_nCastingInterval);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		int nTransformationMonsterId = Convert.ToInt32(dr["transformationMonsterId"]);
		if (nTransformationMonsterId > 0)
		{
			m_transformationMonster = Resource.instance.GetMonster(nTransformationMonsterId);
			if (m_transformationMonster == null)
			{
				SFLogUtil.Warn(GetType(), "변신몬스터가 존재하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", nTransformationMonsterId = " + nTransformationMonsterId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "변신몬스터ID가 유효하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", nTransformationMonsterId = " + nTransformationMonsterId);
		}
		m_nTransformationLifetime = Convert.ToInt32(dr["transformationLifetime"]);
		if (m_nTransformationLifetime <= 0)
		{
			SFLogUtil.Warn(GetType(), "변신유지기간이 유효하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", m_nTransformationLifetime = " + m_nTransformationLifetime);
		}
		m_fObjectInteractionDuration = Convert.ToSingle(dr["objectInteractionDuration"]);
		if (m_fObjectInteractionDuration < 0f)
		{
			SFLogUtil.Warn(GetType(), "오브젝트상호작용시간이 유효하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", m_fObjectInteractionDuration = " + m_fObjectInteractionDuration);
		}
		m_fObjectInteractionMaxRange = Convert.ToSingle(dr["objectInteractionMaxRange"]);
		if (m_fObjectInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "오브젝트상호작용최대범위가 유효하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", m_fObjectInteractionMaxRange = " + m_fObjectInteractionMaxRange);
		}
		m_nObjectLifetime = Convert.ToInt32(dr["objectLifetime"]);
		if (m_nObjectLifetime <= 0)
		{
			SFLogUtil.Warn(GetType(), "오브젝트유지기간이 유효하지 않습니다. stepNo = " + m_wave.step.no + ", waveNo = " + m_wave.no + ", m_nObjectLifetime = " + m_nObjectLifetime);
		}
	}
}
