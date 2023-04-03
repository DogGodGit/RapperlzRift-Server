using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ContinentObjectArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	public const float kInteractionMaxRangeFactor = 1.1f;

	private Continent m_continent;

	private int m_nNo;

	private ContinentObject m_object;

	private Vector3 m_position = Vector3.zero;

	private int m_nYRotationType;

	private float m_fYRotation;

	public Continent continent
	{
		get
		{
			return m_continent;
		}
		set
		{
			m_continent = value;
		}
	}

	public int no => m_nNo;

	public ContinentObject obj => m_object;

	public Vector3 position => m_position;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		int nObjectId = Convert.ToInt32(dr["objectId"]);
		m_object = Resource.instance.GetContinentObject(nObjectId);
		if (m_object == null)
		{
			SFLogUtil.Warn(GetType(), "오브젝트가 존재하지 않습니다. m_nNo = " + m_nNo + ", nObjectId = " + nObjectId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
	}
}
