using System;
using System.Collections.Generic;

namespace GameServer;

public class AnkouTombWave
{
	private AnkouTombDifficulty m_difficulty;

	private int m_nNo;

	private List<AnkouTombMonsterArrange> m_monsterArranges = new List<AnkouTombMonsterArrange>();

	public AnkouTombDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public List<AnkouTombMonsterArrange> monsterArranges => m_monsterArranges;

	public AnkouTombWave(AnkouTombDifficulty difficulty, int nNo)
	{
		m_difficulty = difficulty;
		m_nNo = nNo;
	}

	public void AddMonsterArrange(AnkouTombMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}
}
