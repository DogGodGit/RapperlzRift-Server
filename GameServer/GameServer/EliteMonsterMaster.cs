using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class EliteMonsterMaster
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private int m_nId;

	private int m_nLevel;

	private EliteMonsterCategory m_category;

	private Vector3 m_position = Vector3.zero;

	private int m_nYRotationType;

	private float m_fYRotation;

	private List<EliteMonsterSpawnSchedule> m_spawnSchedules = new List<EliteMonsterSpawnSchedule>();

	private Dictionary<int, EliteMonster> m_eliteMonsters = new Dictionary<int, EliteMonster>();

	private int m_nEliteMonsterTotalPoint;

	public int id => m_nId;

	public int level => m_nLevel;

	public EliteMonsterCategory category => m_category;

	public Vector3 position => m_position;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public List<EliteMonsterSpawnSchedule> spawnSchedules => m_spawnSchedules;

	public Dictionary<int, EliteMonster> eliteMonsters => m_eliteMonsters;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["eliteMonsterMasterId"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nLevel = " + m_nLevel);
		}
		int nCategoryId = Convert.ToInt32(dr["categoryId"]);
		if (nCategoryId > 0)
		{
			m_category = Resource.instance.GetEliteMonsterCategoryByCategoryId(nCategoryId);
			if (m_category == null)
			{
				SFLogUtil.Warn(GetType(), "카테고리가 존재하지 않습니다. m_nId = " + m_nId + ", nCategoryId = " + nCategoryId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId + ", nCategoryId = " + nCategoryId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
	}

	public void AddSpawnSchedule(EliteMonsterSpawnSchedule spawnSchedule)
	{
		if (spawnSchedule == null)
		{
			throw new ArgumentNullException("spawnSchedule");
		}
		m_spawnSchedules.Add(spawnSchedule);
	}

	public EliteMonsterSpawnSchedule GetSpawnScheduleAt(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_spawnSchedules.Count)
		{
			return null;
		}
		return m_spawnSchedules[nIndex];
	}

	public void AddEliteMonster(EliteMonster eliteMonster)
	{
		if (eliteMonster == null)
		{
			throw new ArgumentNullException("eliteMonster");
		}
		m_eliteMonsters.Add(eliteMonster.id, eliteMonster);
		m_nEliteMonsterTotalPoint += eliteMonster.point;
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public EliteMonster SelectMonster()
	{
		return Util.SelectPickEntry(m_eliteMonsters.Values, m_nEliteMonsterTotalPoint);
	}

	public static bool IsDefinedYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
