using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AttainmentEntryReward
{
	private AttainmentEntry m_entry;

	private int m_nNo;

	private AttainmentEntryRewardType m_type;

	private MainGear m_mainGear;

	private bool m_bMainGearOwned;

	private ItemReward m_itemReward;

	public AttainmentEntry entry => m_entry;

	public int no => m_nNo;

	public AttainmentEntryRewardType type => m_type;

	public MainGear mainGear => m_mainGear;

	public bool mainGearOwned => m_bMainGearOwned;

	public ItemReward itemReward => m_itemReward;

	public AttainmentEntryReward(AttainmentEntry entry)
	{
		m_entry = entry;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(AttainmentEntryRewardType), nType))
		{
			SFLogUtil.Warn(GetType(), "보상타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (AttainmentEntryRewardType)nType;
		if (m_type == AttainmentEntryRewardType.MainGear)
		{
			int nMainGearId = Convert.ToInt32(dr["mainGearId"]);
			m_mainGear = Resource.instance.GetMainGear(nMainGearId);
			if (m_mainGear == null)
			{
				SFLogUtil.Warn(GetType(), "메인장비가 존재하지 않습니다. m_nNo = " + m_nNo + ", nMainGearId = " + nMainGearId);
			}
			m_bMainGearOwned = Convert.ToBoolean(dr["mainGearOwned"]);
		}
		else
		{
			long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상아이템이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
	}
}
