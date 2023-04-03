using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountQualityMaster
{
	private int m_nQuality;

	private string m_sNameKey;

	public int quality => m_nQuality;

	public string nameKey => m_sNameKey;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nQuality = Convert.ToInt32(dr["quality"]);
		if (m_nQuality <= 0)
		{
			SFLogUtil.Warn(GetType(), "품질이 유효하지 않습니다. m_nQuality = " + m_nQuality);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}
}
