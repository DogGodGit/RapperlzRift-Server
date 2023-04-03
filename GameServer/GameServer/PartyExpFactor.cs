using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class PartyExpFactor
{
	private int m_nMemberCount;

	private float m_fExpFactor;

	public int memberCount => m_nMemberCount;

	public float expFactor => m_fExpFactor;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nMemberCount = Convert.ToInt32(dr["memberCount"]);
		m_fExpFactor = Convert.ToSingle(dr["expFactor"]);
		if (expFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "경험치계수가 유효하지 않습니다. m_nMemberCount = " + m_nMemberCount + ", m_fExpFactor = " + m_fExpFactor);
		}
	}
}
