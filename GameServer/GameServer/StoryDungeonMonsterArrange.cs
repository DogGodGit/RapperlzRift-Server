using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StoryDungeonMonsterArrange
{
	public const int kMonsterType_NormalMonster = 1;

	public const int kMonsterType_BossMonster = 2;

	public const int kMonsterType_TamingMonster = 3;

	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private StoryDungeonStep m_step;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterCount;

	private int m_nMonsterType;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	public StoryDungeon storyDungeon => difficulty.storyDungeon;

	public StoryDungeonDifficulty difficulty => m_step.difficulty;

	public StoryDungeonStep step => m_step;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterCount => m_nMonsterCount;

	public int monsterType => m_nMonsterType;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public StoryDungeonMonsterArrange(StoryDungeonStep step)
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
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + difficulty.difficulty + ", stepNo = " + m_step.no + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else if (lnMonsterArrangeId < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + difficulty.difficulty + ", stepNo = " + m_step.no + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nMonsterCount = " + m_nMonsterCount);
		}
		m_nMonsterType = Convert.ToInt32(dr["monsterType"]);
		if (!IsDefinedMonsterType(m_nMonsterType))
		{
			SFLogUtil.Warn(GetType(), "몬스터타입이 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + difficulty.difficulty + ", stepNo = " + m_step.no + ", m_nMonsterType = " + m_nMonsterType);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
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

	public static bool IsDefinedMonsterType(int nType)
	{
		if (nType != 1 && nType != 2)
		{
			return nType == 3;
		}
		return true;
	}
}
