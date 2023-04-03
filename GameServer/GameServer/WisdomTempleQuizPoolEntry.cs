using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class WisdomTempleQuizPoolEntry : IPickEntry
{
	private WisdomTempleStep m_step;

	private int m_nNo;

	private List<WisdomTempleQuizRightAnswerPoolEntry> m_rightAnswerPoolEntires = new List<WisdomTempleQuizRightAnswerPoolEntry>();

	private int m_nRightAnswerTotalPoint;

	private List<WisdomTempleQuizWrongAnswerPoolEntry> m_wrongAnswerPoolEntries = new List<WisdomTempleQuizWrongAnswerPoolEntry>();

	private int m_nWrongAnswerTotalPoint;

	public WisdomTempleStep step => m_step;

	public int no => m_nNo;

	public int point => 1;

	int IPickEntry.point => point;

	public WisdomTempleQuizPoolEntry(WisdomTempleStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["quizNo"]);
	}

	public void AddRightAnswerPoolEntry(WisdomTempleQuizRightAnswerPoolEntry rightAnswerPoolEntry)
	{
		if (rightAnswerPoolEntry == null)
		{
			throw new ArgumentNullException("rightAnswerPoolEntry");
		}
		m_rightAnswerPoolEntires.Add(rightAnswerPoolEntry);
		m_nRightAnswerTotalPoint += rightAnswerPoolEntry.point;
	}

	public WisdomTempleQuizRightAnswerPoolEntry SelectRightAnswerPoolEntry()
	{
		return Util.SelectPickEntry(m_rightAnswerPoolEntires, m_nRightAnswerTotalPoint);
	}

	public void AddWrongAnswerPoolEntry(WisdomTempleQuizWrongAnswerPoolEntry wrongAnswerPoolEntry)
	{
		if (wrongAnswerPoolEntry == null)
		{
			throw new ArgumentNullException("wrongAnswerPoolEntry");
		}
		m_wrongAnswerPoolEntries.Add(wrongAnswerPoolEntry);
		m_nWrongAnswerTotalPoint += wrongAnswerPoolEntry.point;
	}

	public List<WisdomTempleQuizWrongAnswerPoolEntry> SelectWrongAnswerPoolEntries(int nCount)
	{
		return Util.SelectPickEntries(m_wrongAnswerPoolEntries, nCount, bDuplicated: false);
	}
}
