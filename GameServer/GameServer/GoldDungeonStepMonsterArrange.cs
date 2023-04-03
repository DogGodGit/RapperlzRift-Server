using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GoldDungeonStepMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private GoldDungeonStep m_step;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterCount;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private bool m_bIsFugitive;

	private int m_nActivationWaveNo;

	public GoldDungeon goldDungeon => m_step.goldDungeon;

	public GoldDungeonDifficulty difficulty => m_step.difficulty;

	public GoldDungeonStep step => m_step;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterCount => m_nMonsterCount;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public bool isFugitive => m_bIsFugitive;

	public int activationWaveNo => m_nActivationWaveNo;

	public GoldDungeonStepMonsterArrange(GoldDungeonStep step)
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
		m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
		if (m_monsterArrange == null)
		{
			SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. difficulty = " + difficulty.difficulty + ", step = " + step.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. difficulty = " + difficulty.difficulty + ", step = " + step.no + ", m_nNo = " + m_nNo + ", m_nMonsterCount = " + m_nMonsterCount);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToInt32(dr["radius"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_bIsFugitive = Convert.ToBoolean(dr["isFugitive"]);
		m_nActivationWaveNo = Convert.ToInt32(dr["activationWaveNo"]);
		if (m_step.GetWave(m_nActivationWaveNo) == null)
		{
			SFLogUtil.Warn(GetType(), "활성웨이브번호가 유효하지 않습니다. difficulty = " + difficulty.difficulty + ", step = " + step.no + ", m_nNo = " + m_nNo + ", m_nActivationWaveNo = " + m_nActivationWaveNo);
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
