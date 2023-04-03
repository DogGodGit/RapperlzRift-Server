using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AnkouTombDifficulty
{
	private AnkouTomb m_ankouTomb;

	private int m_nDifficulty;

	private int m_nMinHeroLevel;

	private int m_nMaxHeroLevel;

	private GoldReward m_goldReward;

	private ExpReward m_expReward;

	private GoldReward m_pointGoldReward;

	private ExpReward m_pointExpReward;

	private long m_lnMaxAdditionalExp;

	private List<AnkouTombRewardPoolEntry> m_rewardPoolEntries = new List<AnkouTombRewardPoolEntry>();

	private int m_nRewardPoolEntryTotalPoint;

	private AnkouTombWave[] m_waves;

	public AnkouTomb ankouTomb => m_ankouTomb;

	public int difficulty => m_nDifficulty;

	public int minHeroLevel => m_nMinHeroLevel;

	public int maxHeroLevel => m_nMaxHeroLevel;

	public GoldReward goldReward => m_goldReward;

	public ExpReward expReward => m_expReward;

	public GoldReward pointGoldReward => m_pointGoldReward;

	public ExpReward pointExpReward => m_pointExpReward;

	public long maxAdditionalExp => m_lnMaxAdditionalExp;

	public AnkouTombDifficulty(AnkouTomb ankouTomb)
	{
		m_ankouTomb = ankouTomb;
		int nWaveCount = ankouTomb.waveCount;
		m_waves = new AnkouTombWave[nWaveCount];
		for (int i = 0; i < nWaveCount; i++)
		{
			m_waves[i] = new AnkouTombWave(this, i + 1);
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		m_nMinHeroLevel = Convert.ToInt32(dr["minHeroLevel"]);
		if (m_nMinHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "최소영웅레벨이 유효하지 않습니다. m_nMinHeroLevel = " + m_nMinHeroLevel);
		}
		m_nMaxHeroLevel = Convert.ToInt32(dr["maxHeroLevel"]);
		if (m_nMaxHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "최대영웅레벨이 유효하지 않습니다. m_nMaxHeroLevel = " + m_nMaxHeroLevel);
		}
		if (m_nMinHeroLevel > m_nMaxHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "최소영웅레벨이 최대영웅레벨보다 큽니다. m_nDifficulty = " + m_nDifficulty + ", m_nMinHeroLevel = " + m_nMinHeroLevel + ", m_nMaxHeroLevel = " + m_nMaxHeroLevel);
		}
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = res.GetGoldReward(lnGoldRewardId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnGoldRewardId = " + lnGoldRewardId);
			}
		}
		else if (lnGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnGoldRewardId = " + lnGoldRewardId);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = res.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else if (lnExpRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치보상ID가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
		}
		long lnPointGoldRewardId = Convert.ToInt64(dr["pointgoldRewardId"]);
		if (lnPointGoldRewardId > 0)
		{
			m_pointGoldReward = res.GetGoldReward(lnPointGoldRewardId);
			if (pointGoldReward == null)
			{
				SFLogUtil.Warn(GetType(), "점수골드보상이 존재하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else if (lnPointGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "점수골드보상ID가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
		}
		long lnPointExpRewardId = Convert.ToInt64(dr["pointExpRewardId"]);
		if (lnPointExpRewardId > 0)
		{
			m_pointExpReward = res.GetExpReward(lnPointExpRewardId);
			if (m_pointExpReward == null)
			{
				SFLogUtil.Warn(GetType(), "점수경험치보상이 존재하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else if (lnPointGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "점수경험치보상ID가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
		}
		m_lnMaxAdditionalExp = Convert.ToInt64(dr["maxAdditionalExp"]);
		if (m_lnMaxAdditionalExp <= 0)
		{
			SFLogUtil.Warn(GetType(), "최대추가경험치가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
		}
	}

	public void AddRewardPoolEntry(AnkouTombRewardPoolEntry rewardPoolEntry)
	{
		if (rewardPoolEntry == null)
		{
			throw new ArgumentNullException("rewardPoolEntry");
		}
		m_rewardPoolEntries.Add(rewardPoolEntry);
		m_nRewardPoolEntryTotalPoint += rewardPoolEntry.point;
	}

	public AnkouTombRewardPoolEntry SelectRewardPoolEntry()
	{
		return Util.SelectPickEntry(m_rewardPoolEntries, m_nRewardPoolEntryTotalPoint);
	}

	public AnkouTombWave GetWave(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Length)
		{
			return null;
		}
		return m_waves[nIndex];
	}
}
