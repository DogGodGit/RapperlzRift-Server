using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ContinentMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private Continent m_continent;

	private int m_nNo;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private MonsterArrange m_arrange;

	private int m_nMonsterCount;

	private int m_nRegenTime;

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

	private Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public MonsterArrange arrange => m_arrange;

	public Monster monster => m_arrange.monster;

	public int monsterCount => m_nMonsterCount;

	public int regenTime => m_nRegenTime;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentException("dr");
		}
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_arrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_arrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. m_nMonsterCount = " + m_nMonsterCount);
		}
		m_nRegenTime = Convert.ToInt32(dr["regenTime"]);
		if (m_nRegenTime < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터리젠시간이 유효하지 않습니다. m_nRegenTime = " + m_nRegenTime);
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
