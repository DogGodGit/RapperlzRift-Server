using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ExpDungeonDifficulty
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	private ExpDungeon m_expDungeon;

	private int m_nDifficulty;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private ExpReward m_expReward;

	private List<ExpDungeonDifficultyWave> m_waves = new List<ExpDungeonDifficultyWave>();

	public ExpDungeon expDungeon => m_expDungeon;

	public int difficulty => m_nDifficulty;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public ExpReward expReward => m_expReward;

	public int waveCount => m_waves.Count;

	public ExpDungeonDifficulty(ExpDungeon expDungeon)
	{
		m_expDungeon = expDungeon;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDifficulty = Convert.ToInt32(dr["difficulty"]);
		m_nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!IsDefinedRequiredConditionType(m_nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", m_nRequiredConditionType = " + m_nRequiredConditionType);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
		if (m_nRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요메인퀘스트번호가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "경험치보상ID가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", lnExpRewardId = " + lnExpRewardId);
		}
	}

	public void AddWave(ExpDungeonDifficultyWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public ExpDungeonDifficultyWave GetWave(int nWaveNo)
	{
		int nIndex = nWaveNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public static bool IsDefinedRequiredConditionType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
