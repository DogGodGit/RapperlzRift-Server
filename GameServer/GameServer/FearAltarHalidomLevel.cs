using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltarHalidomLevel
{
	private FearAltar m_fearAltar;

	private int m_nLevel;

	public FearAltar fearAltar => m_fearAltar;

	public int level => m_nLevel;

	public FearAltarHalidomLevel(FearAltar fearAltar)
	{
		m_fearAltar = fearAltar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["halidomLevel"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "성물레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
	}
}
