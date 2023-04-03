using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AnkouTombMonsterArrange
{
	public const int kMonsterType_NormalMonster = 1;

	public const int kMonsterType_BossMonster = 2;

	private AnkouTombWave m_wave;

	private int m_nNo;

	private int m_nMonsterType;

	private MonsterArrange m_monsterArrange;

	private int m_nMonsterCount;

	public AnkouTombWave wave => m_wave;

	public int no => m_nNo;

	public int monsterType => m_nMonsterType;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public int monsterCount => m_nMonsterCount;

	public AnkouTombMonsterArrange(AnkouTombWave wave)
	{
		m_wave = wave;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["arrangeNo"]);
		m_nMonsterType = Convert.ToInt32(dr["monsterType"]);
		if (!IsDefinedMonsterType(m_nMonsterType))
		{
			SFLogUtil.Warn(GetType(), "몬스터타입이 유효하지 않습니다. difficulty = " + m_wave.difficulty.difficulty + ", waveNo = " + m_wave.no + ", m_nNo = " + m_nNo + ", m_nMonsterType = " + m_nMonsterType);
		}
		long lnMonsterArrangeId = Convert.ToInt64(dr["monsterArrangeId"]);
		if (lnMonsterArrangeId > 0)
		{
			m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrangeId);
			if (m_monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. difficulty = " + m_wave.difficulty.difficulty + ", waveNo = " + m_wave.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터배치ID가 유효하지 않습니다. difficulty = " + m_wave.difficulty.difficulty + ", waveNo = " + m_wave.no + ", m_nNo = " + m_nNo + ", lnMonsterArrangeId = " + lnMonsterArrangeId);
		}
		m_nMonsterCount = Convert.ToInt32(dr["monsterCount"]);
		if (m_nMonsterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터수가 유효하지 않습니다. difficulty = " + m_wave.difficulty.difficulty + ", waveNo = " + m_wave.no + ", m_nNo = " + m_nNo + ", m_nMonsterCount = " + m_nMonsterCount);
		}
	}

	public static bool IsDefinedMonsterType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
