using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimTrap
{
	private RuinsReclaim m_ruinsReclaim;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nDamage;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int damage => m_nDamage;

	public RuinsReclaimTrap(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
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
		if (m_nDamage <= 0)
		{
			SFLogUtil.Warn(GetType(), "데미지가 유효하지 않습니다. m_nId = " + m_nId + ", m_nDamage = " + m_nDamage);
		}
	}
}
