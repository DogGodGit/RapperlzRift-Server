using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ContinentObject
{
	private int m_nId;

	private string m_sNameKey;

	private float m_fInteractionDuration;

	private float m_fInteractionMaxRange;

	private float m_fRadius;

	private bool m_bIsPublic;

	private int m_nRegenTime;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public float interactionDuration => m_fInteractionDuration;

	public float interactionMaxRange => m_fInteractionMaxRange;

	public float radius => m_fRadius;

	public bool isPublic => m_bIsPublic;

	public int regenTime => m_nRegenTime;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["objectId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_fInteractionDuration = Convert.ToSingle(dr["interactionDuration"]);
		if (m_fInteractionDuration < 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용지속시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_fInteractionDuration = " + m_fInteractionDuration);
		}
		m_fInteractionMaxRange = Convert.ToSingle(dr["interactionMaxRange"]);
		if (m_fInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용최대범위가 유효하지 않습니다. m_nId = " + m_nId + ", m_fInteractionMaxRange = " + m_fInteractionMaxRange);
		}
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_bIsPublic = Convert.ToBoolean(dr["isPublic"]);
		m_nRegenTime = Convert.ToInt32(dr["regenTime"]);
		if (m_nRegenTime < 0)
		{
			SFLogUtil.Warn(GetType(), "리젠시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRegenTime = " + m_nRegenTime);
		}
	}
}
