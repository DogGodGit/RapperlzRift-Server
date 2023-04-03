using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AncientRelicTrap
{
	private AncientRelic m_ancientRelic;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fWidth;

	private float m_fHeight;

	private int m_nStartDelayTime;

	private int m_nRegenInterval;

	private int m_nDuration;

	public AncientRelic ancientRelic => m_ancientRelic;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float width => m_fWidth;

	public float height => m_fHeight;

	public int startDelayTime => m_nStartDelayTime;

	public int regenInterval => m_nRegenInterval;

	public int duration => m_nDuration;

	public AncientRelicTrap(AncientRelic ancientRelic)
	{
		m_ancientRelic = ancientRelic;
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
		m_fWidth = Convert.ToSingle(dr["width"]);
		m_fHeight = Convert.ToSingle(dr["height"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nRegenInterval = Convert.ToInt32(dr["regenInterval"]);
		m_nDuration = Convert.ToInt32(dr["duration"]);
		if (m_nDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "지속시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nDuration = " + m_nDuration);
		}
	}
}
