using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainQuestReward
{
	private MainQuest m_mainQuest;

	private int m_nNo;

	private MainQuestRewardType m_type;

	private int m_nJobId;

	private MainGear m_mainGear;

	private bool m_bMainGearOwned;

	private SubGear m_subGear;

	private ItemReward m_itemReward;

	private Mount m_mount;

	private CreatureCard m_creatureCard;

	public MainQuest mainQuest
	{
		get
		{
			return m_mainQuest;
		}
		set
		{
			m_mainQuest = value;
		}
	}

	public int no => m_nNo;

	public MainQuestRewardType type => m_type;

	public int jobId => m_nJobId;

	public MainGear mainGear => m_mainGear;

	public bool mainGearOwned => m_bMainGearOwned;

	public SubGear subGear => m_subGear;

	public ItemReward itemReward => m_itemReward;

	public Mount mount => m_mount;

	public CreatureCard creatureCard => m_creatureCard;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(MainQuestRewardType), nType))
		{
			SFLogUtil.Warn(GetType(), "메인퀘스트 보상 타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (MainQuestRewardType)nType;
		m_nJobId = Convert.ToInt32(dr["jobId"]);
		if (m_nJobId < 0)
		{
			SFLogUtil.Warn(GetType(), "직업 ID가 유효하지 않습니다. m_nJobId = " + m_nJobId);
		}
		switch (m_type)
		{
		case MainQuestRewardType.MainGear:
		{
			int nMainGearId = Convert.ToInt32(dr["mainGearId"]);
			m_mainGear = Resource.instance.GetMainGear(nMainGearId);
			if (m_mainGear == null)
			{
				SFLogUtil.Warn(GetType(), "메인장비가 존재하지 않습니다. m_nNo = " + m_nNo + ", nMainGearId = " + nMainGearId);
			}
			m_bMainGearOwned = Convert.ToBoolean(dr["mainGearOwned"]);
			break;
		}
		case MainQuestRewardType.SubGear:
		{
			int nSubGearId = Convert.ToInt32(dr["subGearId"]);
			m_subGear = Resource.instance.GetSubGear(nSubGearId);
			if (m_subGear == null)
			{
				SFLogUtil.Warn(GetType(), "보조장비가 존재하지 않습니다. m_nNo = " + m_nNo + ", nSubGearId = " + nSubGearId);
			}
			break;
		}
		case MainQuestRewardType.Item:
		{
			long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상아이템이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
			if (m_itemReward.item == null)
			{
				SFLogUtil.Warn(GetType(), "보상아이템의 아이템이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
			break;
		}
		case MainQuestRewardType.Mount:
		{
			int nMountId = Convert.ToInt32(dr["mountId"]);
			m_mount = Resource.instance.GetMount(nMountId);
			if (m_mount == null)
			{
				SFLogUtil.Warn(GetType(), "탈것이 존재하지 않습니다. m_nNo = " + m_nNo + ", nMountId = " + nMountId);
			}
			break;
		}
		case MainQuestRewardType.CreatureCard:
		{
			int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
			m_creatureCard = Resource.instance.GetCreatureCard(nCreatureCardId);
			if (m_creatureCard == null)
			{
				SFLogUtil.Warn(GetType(), "크리처카드가 존재하지 않습니다. m_nNo = " + m_nNo + ", nCreatureCardId = " + nCreatureCardId);
			}
			break;
		}
		}
	}
}
