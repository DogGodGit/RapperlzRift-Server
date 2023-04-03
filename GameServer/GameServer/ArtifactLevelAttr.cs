using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ArtifactLevelAttr
{
	private ArtifactLevel m_level;

	private int m_nId;

	private AttrValue m_attrValue;

	public ArtifactLevel level => m_level;

	public int id => m_nId;

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

	public ArtifactLevelAttr(ArtifactLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_level = level;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		if (!m_level.artifact.IsAttr(m_nId))
		{
			SFLogUtil.Warn(GetType(), "속성ID가 아티팩트의 속성이 아닙니다. artifactNo = " + m_level.artifactNo + ", level = " + m_level.level + ", m_nId = " + m_nId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			throw new Exception("속성값이 존재하지 않습니다. artifactNo = " + m_level.artifactNo + ", level = " + m_level.level + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
