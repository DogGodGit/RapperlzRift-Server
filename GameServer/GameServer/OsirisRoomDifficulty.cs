using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class OsirisRoomDifficulty
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	private OsirisRoom m_osirisRoom;

	private int m_nDifficulty;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	public List<OsirisRoomDifficultyWave> m_waves = new List<OsirisRoomDifficultyWave>();

	public OsirisRoom osirisRoom => m_osirisRoom;

	public int difficulty => m_nDifficulty;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int waveCount => m_waves.Count;

	public OsirisRoomDifficulty(OsirisRoom osirisRoom)
	{
		m_osirisRoom = osirisRoom;
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
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
		if (m_nRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요메인퀘스트번호가 유효하지 않습니다. m_nDifficulty = " + m_nDifficulty + ", m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
		}
	}

	public void AddWave(OsirisRoomDifficultyWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public OsirisRoomDifficultyWave GetWave(int nWaveNo)
	{
		int nIndex = nWaveNo - 1;
		if (nIndex < 0 || nIndex >= waveCount)
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
