using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTemplePuzzle : IPickEntry
{
	public const int kPuzzleId_ColorMatching = 1;

	public const int kPuzzleId_FindTreasureBox = 2;

	private WisdomTemple m_wisdomTemple;

	private int m_nId;

	private int m_nPoint;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int id => m_nId;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public WisdomTemplePuzzle(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["puzzleId"]);
		if (!IsDefinedPuzzleId(m_nId))
		{
			SFLogUtil.Warn(GetType(), "퍼즐ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
	}

	public static bool IsDefinedPuzzleId(int nId)
	{
		if (nId != 1)
		{
			return nId == 2;
		}
		return true;
	}
}
