using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class StoryDungeonPlay
{
	public const int kStatus_Enter = 0;

	public const int kStatus_Clear = 1;

	public const int kStatus_Fail = 2;

	public const int kStatus_Sweep = 3;

	private int m_nDungeonNo;

	private DateTime m_date = DateTime.MinValue.Date;

	private int m_nCount;

	public int dungeonNo => m_nDungeonNo;

	public DateTime date
	{
		get
		{
			return m_date;
		}
		set
		{
			m_date = value;
		}
	}

	public int enterCount
	{
		get
		{
			return m_nCount;
		}
		set
		{
			m_nCount = value;
		}
	}

	public StoryDungeonPlay(int nDungeonNo, DateTime date, int nCount)
	{
		m_nDungeonNo = nDungeonNo;
		m_date = date;
		m_nCount = nCount;
	}

	public PDStoryDungeonPlay ToPDStoryDungeonPlay()
	{
		PDStoryDungeonPlay inst = new PDStoryDungeonPlay();
		inst.dungeonNo = m_nDungeonNo;
		inst.count = m_nCount;
		return inst;
	}

	public static List<PDStoryDungeonPlay> ToPDStoryDungeonPlays(IEnumerable<StoryDungeonPlay> plays)
	{
		List<PDStoryDungeonPlay> results = new List<PDStoryDungeonPlay>();
		foreach (StoryDungeonPlay play in plays)
		{
			results.Add(play.ToPDStoryDungeonPlay());
		}
		return results;
	}
}
