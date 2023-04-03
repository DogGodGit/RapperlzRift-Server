using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainQuestDungeonStep
{
	public const int kType_Move = 1;

	public const int kType_MonsterHunt = 2;

	public const int kType_Direction = 3;

	private MainQuestDungeon m_dungeon;

	private int m_nNo;

	private int m_nType;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private int m_nTargetMonsterArrangeNo;

	private int m_nDirectingDuration;

	private float m_fDirectingStartYRotation;

	private ExpReward m_expReward;

	private GoldReward m_goldReward;

	private List<MainQuestDungeonMonsterArrange> m_monsterArranges = new List<MainQuestDungeonMonsterArrange>();

	public MainQuestDungeon dungeon => m_dungeon;

	public int no => m_nNo;

	public int type => m_nType;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public int targetMonsterArrangeNo => m_nTargetMonsterArrangeNo;

	public int directingDuration => m_nDirectingDuration;

	public float directingStartYRotation => m_fDirectingStartYRotation;

	public ExpReward expReward => m_expReward;

	public GoldReward goldReward => m_goldReward;

	public List<MainQuestDungeonMonsterArrange> monsterArranges => m_monsterArranges;

	public MainQuestDungeonStep(MainQuestDungeon dungeon)
	{
		m_dungeon = dungeon;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["step"]);
		m_nType = Convert.ToInt32(dr["type"]);
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		m_nTargetMonsterArrangeNo = Convert.ToInt32(dr["targetMonsterArrangeNo"]);
		if (m_nType == 3)
		{
			m_nDirectingDuration = Convert.ToInt32(dr["directingDuration"]);
			m_fDirectingStartYRotation = Convert.ToSingle(dr["directingStartYRotation"]);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. dungeonId = " + m_dungeon.id + ", m_nNo = " + m_nNo + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else if (lnExpRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치보상ID가 유효하지 않습니다. dungeonId = " + m_dungeon.id + ", m_nNo = " + m_nNo + ", lnExpRewardId = " + lnExpRewardId);
		}
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. dungeonId = " + m_dungeon.id + ", m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
			}
		}
		else if (lnGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. dungeonId = " + m_dungeon.id + ", m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
		}
	}

	public void AddMonsterArrange(MainQuestDungeonMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public MainQuestDungeonMonsterArrange GetMonsterArrange(int nMonsterArrangeNo)
	{
		int nIndex = nMonsterArrangeNo - 1;
		if (nIndex < 0 || nIndex >= m_monsterArranges.Count)
		{
			return null;
		}
		return m_monsterArranges[nIndex];
	}

	public bool IsInTargetPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius, position);
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_targetPosition, m_fTargetRadius);
	}
}
