using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainQuestDungeonMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private MainQuestDungeonStep m_step;

	private int m_nNo;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterCount;

	private Vector3 m_position = Vector3.zero;

	private int m_nYRotationType;

	private float m_fYRotation;

	private float m_fSummonMinHpFactor;

	private List<MainQuestDungeonSummon> m_summons = new List<MainQuestDungeonSummon>();

	public MainQuestDungeon dungeon => m_step.dungeon;

	public MainQuestDungeonStep step => m_step;

	public int no => m_nNo;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterCount => m_nMonsterCount;

	public Vector3 position => m_position;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public float summonMinHpFactor => m_fSummonMinHpFactor;

	public List<MainQuestDungeonSummon> summons => m_summons;

	public MainQuestDungeonMonsterArrange(MainQuestDungeonStep step)
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
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. dungeonId = " + dungeon.id + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. dungeonId = " + dungeon.id + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. dungeonId = " + dungeon.id + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_nMonsterCount = " + m_nMonsterCount);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fSummonMinHpFactor = Convert.ToSingle(dr["summonMinHpFactor"]);
		if (m_fSummonMinHpFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "소환최소HP계수가 유효하지 않습니다. dungeonId = " + dungeon.id + ", stepNo = " + m_step.no + ", m_nNo = " + m_nNo + ", m_fSummonMinHpFactor = " + m_fSummonMinHpFactor);
		}
	}

	public void AddSummon(MainQuestDungeonSummon summon)
	{
		if (summon == null)
		{
			throw new ArgumentNullException("summon");
		}
		m_summons.Add(summon);
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
