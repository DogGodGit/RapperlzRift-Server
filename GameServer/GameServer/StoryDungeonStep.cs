using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StoryDungeonStep
{
	public const int kType_Move = 1;

	public const int kType_AllMonsterKill = 2;

	public const int kType_BossMonsterKill = 3;

	private StoryDungeonDifficulty m_difficulty;

	private int m_nNo;

	private int m_nType;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private bool m_bIsCompletionRemoveTaming;

	private List<StoryDungeonMonsterArrange> m_monsterArranges = new List<StoryDungeonMonsterArrange>();

	public StoryDungeon storyDungeon => m_difficulty.storyDungeon;

	public StoryDungeonDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public int type => m_nType;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public List<StoryDungeonMonsterArrange> monsterArranges => m_monsterArranges;

	public bool isCompletionRemoveTaming => m_bIsCompletionRemoveTaming;

	public StoryDungeonStep(StoryDungeonDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["step"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nType = " + m_nType);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		m_bIsCompletionRemoveTaming = Convert.ToBoolean(dr["isCompletionRemoveTaming"]);
	}

	public void AddMonsterArrange(StoryDungeonMonsterArrange monsterArrnage)
	{
		if (monsterArrnage == null)
		{
			throw new ArgumentNullException("monsterArrnage");
		}
		m_monsterArranges.Add(monsterArrnage);
	}

	public bool IsInTargetPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius, position);
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1 && nType != 2)
		{
			return nType == 3;
		}
		return true;
	}
}
