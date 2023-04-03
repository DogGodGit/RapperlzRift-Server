using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarNpc
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private NationWar m_nationWar;

	private int m_nId;

	private int m_nContinentId;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private float m_fInteractionMaxRange;

	private float m_fScale;

	private int m_nHeight;

	private float m_fRadius;

	private List<NationWarTransmissionExit> m_transmissionExits = new List<NationWarTransmissionExit>();

	public NationWar nationWar => m_nationWar;

	public int id => m_nId;

	public int continentId => m_nContinentId;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public float interactionMaxRange => m_fInteractionMaxRange;

	public float scale => m_fScale;

	public int height => m_nHeight;

	public float radius => m_fRadius;

	public NationWarNpc(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["npcId"]);
		m_nContinentId = Convert.ToInt32(dr["continentId"]);
		if (m_nContinentId > 0)
		{
			if (Resource.instance.GetContinent(m_nContinentId) == null)
			{
				SFLogUtil.Warn(GetType(), "대륙이 존재하지 않습니다. m_nId = " + m_nId + ", m_nContinentId = " + m_nContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "대륙ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nContinentId = " + m_nContinentId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fInteractionMaxRange = Convert.ToSingle(dr["interactionMaxRange"]);
		if (m_fInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용최대범위가 유효하지 않습니다. m_nId = " + m_nId + ", m_fInteractionMaxRange = " + m_fInteractionMaxRange);
		}
		m_fScale = Convert.ToSingle(dr["scale"]);
		if (m_fScale <= 0f)
		{
			SFLogUtil.Warn(GetType(), "크기가 유효하지 않습니다. m_nId = " + m_nId + ", m_fScale = " + m_fScale);
		}
		m_nHeight = Convert.ToInt32(dr["height"]);
		if (m_nHeight <= 0)
		{
			SFLogUtil.Warn(GetType(), "높이가 유효하지 않습니다. m_nId = " + m_nId + ", m_nHeight = " + m_nHeight);
		}
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
	}

	public void AddTransmissionExit(NationWarTransmissionExit transmissionExit)
	{
		if (transmissionExit == null)
		{
			throw new ArgumentNullException("transmissionExit");
		}
		m_transmissionExits.Add(transmissionExit);
	}

	public NationWarTransmissionExit GetTransmissionExit(int nExitNo)
	{
		int nIndex = nExitNo - 1;
		if (nIndex < 0 || nIndex >= m_transmissionExits.Count)
		{
			return null;
		}
		return m_transmissionExits[nIndex];
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}
}
