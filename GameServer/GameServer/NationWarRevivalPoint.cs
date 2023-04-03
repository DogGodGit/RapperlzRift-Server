using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarRevivalPoint
{
	public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	private NationWar m_nationWar;

	private int m_nId;

	private Continent m_continent;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private int m_nYRotationType;

	private float m_fYRotation;

	private int m_nPriority;

	private Dictionary<int, NationWarRevivalPointActivationCondition> m_activationConditions = new Dictionary<int, NationWarRevivalPointActivationCondition>();

	public NationWar nationWar => m_nationWar;

	public int id => m_nId;

	public Continent continent => m_continent;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public int yRotationType => m_nYRotationType;

	public float yRotation => m_fYRotation;

	public int priority => m_nPriority;

	public Dictionary<int, NationWarRevivalPointActivationCondition> activationConditions => m_activationConditions;

	public NationWarRevivalPoint(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["revivalPointId"]);
		int nContinentId = Convert.ToInt32(dr["continentId"]);
		if (nContinentId > 0)
		{
			m_continent = Resource.instance.GetContinent(nContinentId);
			if (m_continent == null)
			{
				SFLogUtil.Warn(GetType(), "대륙이 존재하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대륙ID가 유효하지 않습니다. m_nId = " + m_nId + ", nContinentId = " + nContinentId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_nYRotationType = Convert.ToInt32(dr["yRotationType"]);
		if (!IsDefinedYRotationType(m_nYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nYRotationType = " + m_nYRotationType);
		}
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_nPriority = Convert.ToInt32(dr["priority"]);
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_position, m_fRadius);
	}

	public float SelectRotationY()
	{
		if (m_nYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fYRotation);
		}
		return m_fYRotation;
	}

	public void AddActivationCondition(NationWarRevivalPointActivationCondition sctivationCondition)
	{
		if (sctivationCondition == null)
		{
			throw new ArgumentNullException("sctivationCondition");
		}
		m_activationConditions.Add(sctivationCondition.monsterArrange.id, sctivationCondition);
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
