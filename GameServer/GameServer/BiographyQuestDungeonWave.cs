using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BiographyQuestDungeonWave
{
	private BiographyQuestDungeon m_dungeon;

	private int m_nNo;

	private BiographyQuestDungeonWaveTargetType m_targetType = BiographyQuestDungeonWaveTargetType.AllMonster;

	private int m_nTargetArrangeKey;

	private Dictionary<int, BiographyQuestMonsterArrange> m_monsterArranges = new Dictionary<int, BiographyQuestMonsterArrange>();

	public BiographyQuestDungeon dungeon => m_dungeon;

	public int no => m_nNo;

	public BiographyQuestDungeonWaveTargetType targetType => m_targetType;

	public int targetArrangeKey => m_nTargetArrangeKey;

	public Dictionary<int, BiographyQuestMonsterArrange> monsterArranges => m_monsterArranges;

	public BiographyQuestDungeonWave(BiographyQuestDungeon dungeon)
	{
		if (dungeon == null)
		{
			throw new ArgumentNullException("dungeon");
		}
		m_dungeon = dungeon;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["waveNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "웨이브번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		int nTargetType = Convert.ToInt32(dr["targetType"]);
		if (!Enum.IsDefined(typeof(BiographyQuestDungeonWaveTargetType), nTargetType))
		{
			SFLogUtil.Warn(GetType(), "목표타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nTargetType = " + nTargetType);
		}
		m_targetType = (BiographyQuestDungeonWaveTargetType)nTargetType;
		if (m_targetType == BiographyQuestDungeonWaveTargetType.TargetMonster)
		{
			m_nTargetArrangeKey = Convert.ToInt32(dr["targetArrangeKey"]);
			if (m_nTargetArrangeKey <= 0)
			{
				SFLogUtil.Warn(GetType(), "목표배치키가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetArrangeKey = " + m_nTargetArrangeKey);
			}
		}
	}

	public void AddMonsterArrange(BiographyQuestMonsterArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_monsterArranges.Add(arrange.key, arrange);
	}
}
