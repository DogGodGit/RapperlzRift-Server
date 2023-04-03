using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SubGearLevelQuality
{
	private SubGearLevel m_level;

	private int m_nQuality;

	private int m_nNextQualityUpItem1Id;

	private int m_nNextQualityUpItem1Count;

	private int m_nNextQualityUpItem2Id;

	private int m_nNextQualityUpItem2Count;

	private Dictionary<int, SubGearAttrValue> m_attrValues = new Dictionary<int, SubGearAttrValue>();

	public SubGearLevel level
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public int quality => m_nQuality;

	public int nextQualityUpItem1Id => m_nNextQualityUpItem1Id;

	public int nextQualityUpItem1Count => m_nNextQualityUpItem1Count;

	public int nextQualityUpItem2Id => m_nNextQualityUpItem2Id;

	public int nextQualityUpItem2Count => m_nNextQualityUpItem2Count;

	public IEnumerable<SubGearAttrValue> attrValues => m_attrValues.Values;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nQuality = Convert.ToInt32(dr["quality"]);
		m_nNextQualityUpItem1Id = Convert.ToInt32(dr["nextQualityUpItem1Id"]);
		m_nNextQualityUpItem1Count = Convert.ToInt32(dr["nextQualityUpItem1Count"]);
		if (m_nNextQualityUpItem1Id > 0 && m_nNextQualityUpItem1Count <= 0)
		{
			SFLogUtil.Warn(GetType(), "품질업 재료 아이템1 수량이 유효하지 않습니다. m_nNextQualityUpItem1Count = " + m_nNextQualityUpItem1Count);
		}
		m_nNextQualityUpItem2Id = Convert.ToInt32(dr["nextQualityUpItem2Id"]);
		m_nNextQualityUpItem2Count = Convert.ToInt32(dr["nextQualityUpItem2Count"]);
		if (m_nNextQualityUpItem2Id > 0 && m_nNextQualityUpItem2Count <= 0)
		{
			SFLogUtil.Warn(GetType(), "품질업 재료 아이템2 수량이 유효하지 않습니다. m_nNextQualityUpItem1Count = " + m_nNextQualityUpItem1Count);
		}
	}

	public void AddAttrValue(SubGearAttrValue value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.quality != null)
		{
			throw new Exception("이미 레벨품질에 추가된 속성값입니다.");
		}
		m_attrValues.Add(value.id, value);
		value.quality = this;
	}
}
