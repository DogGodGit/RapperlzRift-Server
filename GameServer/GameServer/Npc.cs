using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Npc
{
	public const int kType_Normal = 1;

	public const int kType_ContinentTransmission = 2;

	public const int kType_NationTransmission = 3;

	public const float kInteractionMaxRangeFactor = 1.3f;

	private int m_nId;

	private string m_sNameKey;

	private string m_sNickKey;

	private string m_sDialogueKey;

	private int m_nType;

	private Continent m_continent;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private float m_fInteractionMaxRange;

	private float m_fScale;

	private int m_nHeight;

	private float m_fRadius;

	private List<ContinentTransmissionExit> m_continentTransmissionExits = new List<ContinentTransmissionExit>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public string NickKey => m_sNickKey;

	public string dialogueKey => m_sDialogueKey;

	public int type => m_nType;

	public Continent continent => m_continent;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public float interactionMaxRange => m_fInteractionMaxRange;

	public float scale => m_fScale;

	public int height => m_nHeight;

	public float radius => m_fRadius;

	public Npc(Continent continent)
	{
		m_continent = continent;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["npcId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sNickKey = Convert.ToString(dr["nickKey"]);
		m_sDialogueKey = Convert.ToString(dr["dialogueKey"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nType = " + m_nType);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fInteractionMaxRange = Convert.ToSingle(dr["interactionMaxRange"]);
		m_fScale = Convert.ToSingle(dr["scale"]);
		m_nHeight = Convert.ToInt32(dr["height"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
	}

	public void AddContinentTransmissionExit(ContinentTransmissionExit continentTransmissionExit)
	{
		if (continentTransmissionExit == null)
		{
			throw new ArgumentNullException("continentTransmissionExit");
		}
		m_continentTransmissionExits.Add(continentTransmissionExit);
	}

	public ContinentTransmissionExit GetContinentTransmissionExit(int nExitNo)
	{
		int nIndex = nExitNo - 1;
		if (nIndex < 0 || nIndex >= m_continentTransmissionExits.Count)
		{
			return null;
		}
		return m_continentTransmissionExits[nIndex];
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fInteractionMaxRange * 1.3f + fRadius * 2f, position);
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1 && nType != 3)
		{
			return nType == 2;
		}
		return true;
	}
}
