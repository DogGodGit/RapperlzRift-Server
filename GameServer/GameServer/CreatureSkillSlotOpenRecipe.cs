using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureSkillSlotOpenRecipe
{
	private int m_nSlotCount;

	private int m_nRequiredItemId;

	private int m_nRequiredItemCount;

	public int slotCount => m_nSlotCount;

	public int requiredItemId => m_nRequiredItemId;

	public int requiredItemCount => m_nRequiredItemCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSlotCount = Convert.ToInt32(dr["slotCount"]);
		if (m_nSlotCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "슬롯카운트가 유효하지 않습니다. m_nSlotCount = " + m_nSlotCount);
		}
		m_nRequiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		if (m_nRequiredItemId < 0)
		{
			SFLogUtil.Warn(GetType(), "필요아이템ID가 유효하지 않습니다. m_nSlotCount = " + m_nSlotCount + ", m_nRequiredItemId = " + m_nRequiredItemId);
		}
		m_nRequiredItemCount = Convert.ToInt32(dr["requiredItemCount"]);
		if (m_nRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요아이템개수가 유효하지 않습니다. m_nSlotCount = " + m_nSlotCount + ", m_nRequiredItemCount = " + m_nRequiredItemCount);
		}
	}
}
