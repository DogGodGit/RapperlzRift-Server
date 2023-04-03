using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountAwakeningLevelMaster
{
	private int m_nLevel;

	private float m_fUnequippedAttrFacter;

	public int level => m_nLevel;

	public float unequippedAttrFacter => m_fUnequippedAttrFacter;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["awakeningLevel"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_fUnequippedAttrFacter = Convert.ToSingle(dr["unequippedAttrFactor"]);
		if (m_fUnequippedAttrFacter < 0f)
		{
			SFLogUtil.Warn(GetType(), "속성계수가 유효하지 않습니다. m_fUnequippedAttrFacter = " + m_fUnequippedAttrFacter);
		}
	}
}
