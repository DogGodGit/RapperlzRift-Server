using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureSkillCountPoolEntry : IPickEntry
{
	private int m_nNo;

	private int m_nPoint;

	private int m_nSkillCount;

	public int no => m_nNo;

	public int point => m_nPoint;

	public int skillCount => m_nSkillCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		m_nSkillCount = Convert.ToInt32(dr["skillCount"]);
		if (m_nSkillCount < 0)
		{
			SFLogUtil.Warn(GetType(), "스킬개수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nSkillCount = " + m_nSkillCount);
		}
	}
}
