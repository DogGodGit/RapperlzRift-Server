using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DragonNestTrap
{
	private DragonNest m_dragonNest;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private float m_fRadius;

	private int m_nDamage;

	private int m_nActivationStepNo;

	private int m_nDeactivationStepNo;

	public DragonNest dragonNest => m_dragonNest;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public float radius => m_fRadius;

	public int damage => m_nDamage;

	public int activationStepNo => m_nActivationStepNo;

	public int deactivationStepNo => m_nDeactivationStepNo;

	public DragonNestTrap(DragonNest dragonNest)
	{
		m_dragonNest = dragonNest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["trapId"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_nDamage = Convert.ToInt32(dr["damage"]);
		m_nActivationStepNo = Convert.ToInt32(dr["activationStepNo"]);
		if (m_nActivationStepNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "활성화단계번호가 유효하지 않습니다. m_nId = " + m_nId + ", m_nActivationStepNo = " + m_nActivationStepNo);
		}
		m_nDeactivationStepNo = Convert.ToInt32(dr["deactivationStepNo"]);
		if (m_nDeactivationStepNo < 0)
		{
			SFLogUtil.Warn(GetType(), "비활성화단계번호가 유효하지 않습니다. m_nId = " + m_nId + ", m_nDeactivationStepNo = " + m_nDeactivationStepNo);
		}
	}

	public bool ContainsPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_position, m_fRadius, position);
	}
}
