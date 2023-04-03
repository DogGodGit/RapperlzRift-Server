using System;
using System.Data;

namespace GameServer;

public class WisdomTempleArrangePosition
{
	private WisdomTemple m_wisdomTemple;

	private int m_nRow;

	private int m_nCol;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int row => m_nRow;

	public int col => m_nCol;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public WisdomTempleArrangePosition(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRow = Convert.ToInt32(dr["row"]);
		m_nCol = Convert.ToInt32(dr["col"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
	}
}
