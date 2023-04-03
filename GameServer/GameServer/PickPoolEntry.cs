using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class PickPoolEntry : IPickEntry
{
	private PickPool m_pool;

	private int m_nNo;

	private int m_nPoint;

	private PickPoolEntryType m_type;

	private int m_nJobId;

	private int m_nMinHeroLevel;

	private int m_nMaxHeroLevel;

	private MainGear m_mainGear;

	private bool m_bMainGearOwned;

	private Item m_item;

	private int m_nItemCount;

	private bool m_bItemOwned;

	private MountGear m_mountGear;

	private bool m_bMountGearOwned;

	private CreatureCard m_creatureCard;

	private Creature m_creature;

	public PickPool pool
	{
		get
		{
			return m_pool;
		}
		set
		{
			m_pool = value;
		}
	}

	public int no => m_nNo;

	public int point => m_nPoint;

	public PickPoolEntryType type => m_type;

	public int jobId => m_nJobId;

	public int minHeroLevel => m_nMinHeroLevel;

	public int maxHeroLevel => m_nMaxHeroLevel;

	public MainGear mainGear => m_mainGear;

	public bool mainGearOwned => m_bMainGearOwned;

	public Item item => m_item;

	public int itemCount => m_nItemCount;

	public bool itemOwned => m_bItemOwned;

	public MountGear mountGear => m_mountGear;

	public bool mountGearOwned => m_bMountGearOwned;

	public CreatureCard creatureCard => m_creatureCard;

	public Creature creature => m_creature;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(PickPoolEntryType), nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (PickPoolEntryType)nType;
		m_nJobId = Convert.ToInt32(dr["jobId"]);
		if (m_nJobId < 0)
		{
			SFLogUtil.Warn(GetType(), "직업 ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nJobId = " + m_nJobId);
		}
		m_nMinHeroLevel = Convert.ToInt32(dr["minHeroLevel"]);
		if (m_nMinHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "최소영웅레벨이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nMinHeroLevel = " + m_nMinHeroLevel);
		}
		m_nMaxHeroLevel = Convert.ToInt32(dr["maxHeroLevel"]);
		if (m_nMaxHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "최대영웅레벨이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nMaxHeroLevel = " + m_nMaxHeroLevel);
		}
		if (m_nMinHeroLevel > m_nMaxHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "최소영웅레벨이 최대영웅레벨을 넘어갑니다. m_nNo = " + m_nNo + ", m_nMinHeroLevel = " + m_nMinHeroLevel + ", m_nMaxHeroLevel = " + m_nMaxHeroLevel);
		}
		switch (m_type)
		{
		case PickPoolEntryType.MainGear:
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
		case PickPoolEntryType.Item:
		{
			int nItemId = Convert.ToInt32(dr["itemId"]);
			m_item = Resource.instance.GetItem(nItemId);
			if (m_item == null)
			{
				SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다 m_nNo = " + m_nNo + ", nItemId = " + nItemId);
			}
			m_nItemCount = Convert.ToInt32(dr["itemCount"]);
			if (m_nItemCount <= 0)
			{
				SFLogUtil.Warn(GetType(), "수량이 유효하지 않습니다. m_nNo = " + m_nNo + ", nItemId = " + nItemId + ", m_nItemCount = " + m_nItemCount);
			}
			m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
			break;
		}
		case PickPoolEntryType.MountGear:
		{
			int nMountGearId = Convert.ToInt32(dr["mountGearId"]);
			m_mountGear = Resource.instance.GetMountGear(nMountGearId);
			if (m_mountGear == null)
			{
				SFLogUtil.Warn(GetType(), "탈것장비가 존재하지 않습니다. m_nNo = " + m_nNo + ", nMountGearId = " + nMountGearId);
			}
			m_bMountGearOwned = Convert.ToBoolean(dr["mountGearOwned"]);
			break;
		}
		case PickPoolEntryType.CreatureCard:
		{
			int nCreatureCardId = Convert.ToInt32(dr["creatureCardId"]);
			m_creatureCard = Resource.instance.GetCreatureCard(nCreatureCardId);
			if (m_creatureCard == null)
			{
				SFLogUtil.Warn(GetType(), "크리처카드가 존재하지 않습니다. m_nNo = " + m_nNo + ", nCreatureCardId = " + nCreatureCardId);
			}
			break;
		}
		case PickPoolEntryType.Creature:
		{
			int nCreatureId = Convert.ToInt32(dr["creatureId"]);
			m_creature = Resource.instance.GetCreature(nCreatureId);
			if (m_creature == null)
			{
				SFLogUtil.Warn(GetType(), "크리처가 존재하지 않습니다. m_nNo = " + m_nNo + ", nCreatureId = " + nCreatureId);
			}
			break;
		}
		default:
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
			break;
		}
	}

	public bool IsAvaliableHeroLevel(int nLevel)
	{
		if (nLevel <= 0)
		{
			return false;
		}
		if (nLevel < m_nMinHeroLevel)
		{
			return false;
		}
		if (0 < m_nMaxHeroLevel && nLevel > m_nMaxHeroLevel)
		{
			return false;
		}
		return true;
	}
}
