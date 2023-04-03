using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class OsirisRoomDifficultyWave
{
	private OsirisRoomDifficulty m_difficulty;

	private int m_nNo;

	private List<OsirisRoomMonsterArrange> m_monsterArranges = new List<OsirisRoomMonsterArrange>();

	public OsirisRoomDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public int monsterArrangeCount => m_monsterArranges.Count;

	public OsirisRoomDifficultyWave(OsirisRoomDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
	}

	public void AddMonsterArrange(OsirisRoomMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public OsirisRoomMonsterArrange GetMonsterArrange(int nArrangeNo)
	{
		int nIndex = nArrangeNo - 1;
		if (nIndex < 0 || nIndex >= monsterArrangeCount)
		{
			return null;
		}
		return m_monsterArranges[nIndex];
	}
}
