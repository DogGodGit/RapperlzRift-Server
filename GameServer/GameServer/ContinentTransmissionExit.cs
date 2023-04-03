using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ContinentTransmissionExit
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private Npc m_npc;

	private int m_nNo;

	private string m_sNameKey;

	private Continent m_continent;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	public Npc npc => m_npc;

	public int no => m_nNo;

	public string nameKey => m_sNameKey;

	public Continent continent => m_continent;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public ContinentTransmissionExit(Npc npc)
	{
		m_npc = npc;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["exitNo"]);
		m_sNameKey = Convert.ToString(dr["continentId"]);
		int nContinentId = Convert.ToInt32(dr["continentId"]);
		if (nContinentId > 0)
		{
			m_continent = Resource.instance.GetContinent(nContinentId);
			if (m_continent == null)
			{
				SFLogUtil.Warn(GetType(), "대륙이 존재하지 않습니다. npcId = " + m_npc.id + ", m_nNo = " + m_nNo + ", nContinentId = " + nContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대륙ID가 유효하지 않습니다. npcId = " + m_npc.id + ", m_nNo = " + m_nNo + ", nContinentId = " + nContinentId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (m_nYRotationType < 1 || m_nYRotationType > 2)
		{
			SFLogUtil.Warn(GetType(), string.Concat("방향타입이 유효하지 않습니다. m_npc = ", m_npc, ", m_nNo = ", m_nNo, ", m_nYRotationType = ", m_nYRotationType));
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
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
}
