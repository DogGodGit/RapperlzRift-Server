using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TradeShipMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private TradeShipDifficultyStep m_step;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private Vector3 m_position = Vector3.zero;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nMonsterCount;

	private int m_nPoint;

	public TradeShipDifficultyStep step => m_step;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public Vector3 position => m_position;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int monsterCount => m_nMonsterCount;

	public int point => m_nPoint;

	public TradeShipMonsterArrange(TradeShipDifficultyStep step)
	{
		m_step = step;
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
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. difficulty = " + m_step.difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. difficulty = " + m_step.difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. difficulty = " + m_step.difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. difficulty = " + m_step.difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nMonsterCount = " + m_nMonsterCount);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "점수가 유효하지 않습니다. difficulty = " + m_step.difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public static bool IsDefinedYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
