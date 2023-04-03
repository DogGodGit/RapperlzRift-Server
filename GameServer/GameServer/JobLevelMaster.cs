using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobLevelMaster
{
	private int m_nLevel;

	private long m_lnNextLevelUpExp;

	private int m_nInventorySlotAccCount;

	private ExpReward m_restMaxExpReward;

	private AttrValue m_cartMaxHpAttrValue;

	private int m_nPotionAttrMaxCount;

	public int level => m_nLevel;

	public long nextLevelUpExp => m_lnNextLevelUpExp;

	public int inventorySlotAccCount => m_nInventorySlotAccCount;

	public ExpReward restMaxExpReward => m_restMaxExpReward;

	public long restMaxExpRewardValue
	{
		get
		{
			if (m_restMaxExpReward == null)
			{
				return 0L;
			}
			return m_restMaxExpReward.value;
		}
	}

	public AttrValue cartMaxHpAttrValue => m_cartMaxHpAttrValue;

	public int cartMaxHp
	{
		get
		{
			if (m_cartMaxHpAttrValue == null)
			{
				return 0;
			}
			return m_cartMaxHpAttrValue.value;
		}
	}

	public int potionAttrMaxCount => m_nPotionAttrMaxCount;

	public bool isMaxLevel => m_nLevel >= Resource.instance.lastJobLevelMaster.level;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_lnNextLevelUpExp = Convert.ToInt64(dr["nextLevelUpExp"]);
		m_nInventorySlotAccCount = Convert.ToInt32(dr["inventorySlotAccCount"]);
		long lnRestMaxExpRewardId = Convert.ToInt64(dr["restMaxExpRewardId"]);
		m_restMaxExpReward = Resource.instance.GetExpReward(lnRestMaxExpRewardId);
		if (m_restMaxExpReward == null)
		{
			SFLogUtil.Warn(GetType(), "휴식보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnRestMaxExpRewardId = " + lnRestMaxExpRewardId);
		}
		long lnCartMaxHpAttrValueId = Convert.ToInt64(dr["cartMaxHpAttrValueId"]);
		m_cartMaxHpAttrValue = Resource.instance.GetAttrValue(lnCartMaxHpAttrValueId);
		if (m_cartMaxHpAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "카트최대HP속성값이 존재하지 않습니다. lnCartMaxHpAttrValueId = " + lnCartMaxHpAttrValueId);
		}
		m_nPotionAttrMaxCount = Convert.ToInt32(dr["potionAttrMaxCount"]);
		if (m_nPotionAttrMaxCount < 0)
		{
			SFLogUtil.Warn(GetType(), "속성물약증가최대횟수가 유효하지 않습니다. m_nPotionAttrMaxCount = " + m_nPotionAttrMaxCount);
		}
	}
}
