using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestMonsterArrange
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private TreatOfFarmQuestMission m_mission;

	private int m_nHeroMinLevel;

	private MonsterArrange m_monsterArrange;

	public TreatOfFarmQuestMission mission
	{
		get
		{
			return m_mission;
		}
		set
		{
			m_mission = value;
		}
	}

	public int heroMinLevel => m_nHeroMinLevel;

	public MonsterArrange monsterArrange => m_monsterArrange;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nHeroMinLevel = Convert.ToInt32(dr["heroMinLevel"]);
		if (m_nHeroMinLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "영웅최소레벨이 유효하지 않습니다. m_nHeroMinLevel = " + m_nHeroMinLevel);
		}
		long lnMonsterArrange = Convert.ToInt64(dr["monsterArrangeId"]);
		m_monsterArrange = Resource.instance.GetMonsterArrange(lnMonsterArrange);
		if (m_monsterArrange == null)
		{
			SFLogUtil.Warn(GetType(), "몬스터배치가 존재하지 않습니다. m_nHeroMinLevel = " + m_nHeroMinLevel + ", lnMonsterArrange = " + lnMonsterArrange);
		}
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_mission.monsterSpawnPosition, m_mission.targetRadius);
	}

	public float SelectRotationY()
	{
		if (m_mission.monsterRorationType != 1)
		{
			return SFRandom.NextFloat(0f, m_mission.monsterRotationY);
		}
		return m_mission.monsterRotationY;
	}
}
