using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RankPassiveSkillAttrLevel
{
	private RankPassiveSkillAttr m_attr;

	private RankPassiveSkillLevel m_level;

	private AttrValue m_attrValue;

	public RankPassiveSkillAttr attr => m_attr;

	public RankPassiveSkillLevel level => m_level;

	public AttrValue attrValue => m_attrValue;

	public int value
	{
		get
		{
			if (m_attrValue == null)
			{
				return 0;
			}
			return m_attrValue.value;
		}
	}

	public RankPassiveSkillAttrLevel(RankPassiveSkillAttr attr, RankPassiveSkillLevel level)
	{
		m_attr = attr;
		m_level = level;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		if (lnAttrValueId > 0)
		{
			m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
			if (attrValue == null)
			{
				SFLogUtil.Warn(GetType(), "속성값ID가 존재하지 않습니다. skillId = " + m_attr.id + ", attrId = " + m_attr.id + ", level = " + m_level.level + ", lnAttrValueId = " + lnAttrValueId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "속성값ID가 유효하지 않습니다. skillId = " + m_attr.id + ", attrId = " + m_attr.id + ", level = " + m_level.level + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
