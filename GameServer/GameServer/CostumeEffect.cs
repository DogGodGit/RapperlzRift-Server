using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeEffect
{
	private int m_nId;

	private int m_nRequiredItemId;

	public int id => m_nId;

	public int requiredItemId => m_nRequiredItemId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["costumeEffectId"]);
		m_nRequiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		if (m_nRequiredItemId > 0)
		{
			if (Resource.instance.GetItem(m_nRequiredItemId) == null)
			{
				SFLogUtil.Warn(GetType(), "필요아이템이 존재하지 않습니다. m_nId = " + m_nId + ", m_nRequiredItemId = " + m_nRequiredItemId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "필요아이템ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredItemId = " + m_nRequiredItemId);
		}
	}
}
