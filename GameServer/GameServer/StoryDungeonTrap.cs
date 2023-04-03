using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StoryDungeonTrap
{
	private StoryDungeonDifficulty m_difficulty;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private float m_fWidth;

	private float m_fHeight;

	private float m_fHitAreaOffset;

	private float m_fStartDelay;

	private float m_fCastingStartDelay;

	private int m_nCastingDuration;

	private int m_nHitCount;

	private float m_fCastingTerm;

	private float m_fDamage;

	public StoryDungeon storyDungeon => m_difficulty.storyDungeon;

	public StoryDungeonDifficulty difficulty => m_difficulty;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public float width => m_fWidth;

	public float height => m_fHeight;

	public float hitAreaOffset => m_fHitAreaOffset;

	public float startDelay => m_fStartDelay;

	public float castingStartDelay => m_fCastingStartDelay;

	public int castingDuration => m_nCastingDuration;

	public int hitCount => m_nHitCount;

	public float castingTerm => m_fCastingTerm;

	public float damage => m_fDamage;

	public StoryDungeonTrap(StoryDungeonDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["trapId"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fWidth = Convert.ToSingle(dr["width"]);
		if (m_fWidth <= 0f)
		{
			SFLogUtil.Warn(GetType(), "가로크기가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fWidth = " + m_fWidth);
		}
		m_fHeight = Convert.ToSingle(dr["height"]);
		if (m_fHeight <= 0f)
		{
			SFLogUtil.Warn(GetType(), "세로크기가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fHeight = " + m_fHeight);
		}
		m_fHitAreaOffset = Convert.ToSingle(dr["hitAreaOffset"]);
		m_fStartDelay = Convert.ToSingle(dr["startDelay"]);
		if (m_fStartDelay < 0f)
		{
			SFLogUtil.Warn(GetType(), "시작딜레이가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fStartDelay = " + m_fStartDelay);
		}
		m_fCastingStartDelay = Convert.ToSingle(dr["castingStartDelay"]);
		if (m_fCastingStartDelay < 0f)
		{
			SFLogUtil.Warn(GetType(), "시전시작딜레이가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fCastingStartDelay = " + m_fCastingStartDelay);
		}
		m_nCastingDuration = Convert.ToInt32(dr["castingDuration"]);
		if (m_nCastingDuration < 0)
		{
			SFLogUtil.Warn(GetType(), "시전유지시간이 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_nCastingDuration = " + m_nCastingDuration);
		}
		m_nHitCount = Convert.ToInt32(dr["hitCount"]);
		if (m_nHitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "히트수가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_nHitCount = " + m_nHitCount);
		}
		m_fCastingTerm = Convert.ToSingle(dr["castingTerm"]);
		if (m_fCastingTerm <= 0f)
		{
			SFLogUtil.Warn(GetType(), "시전간격이 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fCastingTerm = " + m_fCastingTerm);
		}
		if (m_fCastingTerm < m_fCastingStartDelay + (float)m_nCastingDuration)
		{
			SFLogUtil.Warn(GetType(), "시전간격이 시전시작딜레이와 시전유지시간의 합보다 작습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fCastingStartDelay = " + m_fCastingStartDelay + ", m_nCastingDuration = " + m_nCastingDuration + ", m_fCastingTerm = " + m_fCastingTerm);
		}
		m_fDamage = Convert.ToSingle(dr["damage"]);
		if (m_fDamage <= 0f)
		{
			SFLogUtil.Warn(GetType(), "데미지가 유효하지 않습니다. dungeonNo = " + storyDungeon.no + ", difficulty = " + m_difficulty.difficulty + ", m_nId = " + m_nId + ", m_fDamage = " + m_fDamage);
		}
	}

	public bool ValidationAreaContains(Vector3 targetPosition)
	{
		Vector3 revisionTargetPosition = MathUtil.PositionRotation(targetPosition - m_position, m_fYRotation);
		revisionTargetPosition.z -= m_fHitAreaOffset;
		if (MathUtil.SquareContains(Vector3.zero, m_fWidth, m_fHeight, revisionTargetPosition))
		{
			return true;
		}
		return false;
	}
}
