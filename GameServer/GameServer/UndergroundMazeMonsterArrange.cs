using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private UndergroundMazeFloor m_floor;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterCount;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nRegenTime;

	public UndergroundMaze undergroundMaze => m_floor.undergroundMaze;

	public UndergroundMazeFloor floor => m_floor;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterCount => m_nMonsterCount;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int regenTime => m_nRegenTime;

	public UndergroundMazeMonsterArrange(UndergroundMazeFloor floor)
	{
		m_floor = floor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. floor = " + m_floor.floor + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. floor = " + m_floor.floor + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. floor = " + m_floor.floor + ", m_nNo = " + m_nNo + ", m_nMonsterCount = " + m_nMonsterCount);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (m_nYRotationType < 1 || m_nYRotationType > 2)
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. floor = " + m_floor.floor + ", m_nNo = " + m_nNo + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nRegenTime = Convert.ToInt32(dr["regenTime"]);
		if (m_nRegenTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "리젠시간이 유효하지 않습니다. floor = " + m_floor.floor + ", m_nNo = " + m_nNo + ", m_nRegenTime = " + m_nRegenTime);
		}
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
