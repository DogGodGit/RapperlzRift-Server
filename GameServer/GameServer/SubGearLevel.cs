using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class SubGearLevel
{
	private SubGear m_subGear;

	private int m_nLevel;

	private int m_nGrade;

	private int m_nNextLevelUpRequiredGold;

	private int m_nNextGradeUpItem1Id;

	private int m_nNextGradeUpItem1Count;

	private int m_nNextGradeUpItem2Id;

	private int m_nNextGradeUpItem2Count;

	private List<SubGearLevelQuality> m_qualities = new List<SubGearLevelQuality>();

	public SubGear subGear
	{
		get
		{
			return m_subGear;
		}
		set
		{
			m_subGear = value;
		}
	}

	public int level => m_nLevel;

	public int grade => m_nGrade;

	public int nextLevelUpRequiredGold => m_nNextLevelUpRequiredGold;

	public int nextGradeUpItem1Id => m_nNextGradeUpItem1Id;

	public int nextGradeUpItem1Count => m_nNextGradeUpItem1Count;

	public int nextGradeUpItem2Id => m_nNextGradeUpItem2Id;

	public int nextGradeUpItem2Count => m_nNextGradeUpItem2Count;

	public SubGearLevelQuality firstQuality => m_qualities.FirstOrDefault();

	public SubGearLevelQuality lastQuality => m_qualities.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_nGrade = Convert.ToInt32(dr["grade"]);
		m_nNextLevelUpRequiredGold = Convert.ToInt32(dr["nextLevelUpRequiredGold"]);
		m_nNextGradeUpItem1Id = Convert.ToInt32(dr["nextGradeUpItem1Id"]);
		m_nNextGradeUpItem1Count = Convert.ToInt32(dr["nextGradeUpItem1Count"]);
		if (m_nNextGradeUpItem1Id > 0 && m_nNextGradeUpItem1Count <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급업 재료 아이템1 수량이 유효하지 않습니다. m_nNextGradeUpItem1Count = " + m_nNextGradeUpItem1Count);
		}
		m_nNextGradeUpItem2Id = Convert.ToInt32(dr["nextGradeUpItem2Id"]);
		m_nNextGradeUpItem2Count = Convert.ToInt32(dr["nextGradeUpItem2Count"]);
		if (m_nNextGradeUpItem2Id > 0 && m_nNextGradeUpItem2Count <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급업 재료 아이템2 수량이 유효하지 않습니다. m_nNextGradeUpItem1Count = " + m_nNextGradeUpItem1Count);
		}
	}

	public void AddQuality(SubGearLevelQuality quality)
	{
		if (quality == null)
		{
			throw new ArgumentNullException("quality");
		}
		if (quality.level != null)
		{
			throw new Exception("이미 레벨에 추가된 품질입니다.");
		}
		m_qualities.Add(quality);
		quality.level = this;
	}

	public SubGearLevelQuality GetQuality(int nQuality)
	{
		if (nQuality < 0 || nQuality >= m_qualities.Count)
		{
			return null;
		}
		return m_qualities[nQuality];
	}
}
