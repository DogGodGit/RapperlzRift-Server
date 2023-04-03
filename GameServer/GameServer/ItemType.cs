using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ItemType
{
	public const int kType_HPPotion = 1;

	public const int kType_ReturnScroll = 2;

	public const int kType_MainGearBox = 3;

	public const int kType_PickBox = 4;

	public const int kType_Megaphone = 5;

	public const int kType_ExpPotion = 6;

	public const int kType_MountLevelUp = 7;

	public const int kType_DungeonSweepItem = 10;

	public const int kType_ExpScroll = 11;

	public const int kType_BountyHunterQuestScroll = 13;

	public const int kType_FishingBait = 14;

	public const int kType_DistortionScroll = 16;

	public const int kType_GoldItem = 18;

	public const int kType_OwnDiaItem = 19;

	public const int kType_HonorPointItem = 20;

	public const int kType_ExploitPointItem = 21;

	public const int kType_GuildCallingItem = 23;

	public const int kType_NationCallingItem = 24;

	public const int kType_Title = 25;

	public const int kType_IllustratedBook = 26;

	public const int kType_MountItem = 28;

	public const int kType_WingItem = 29;

	public const int kType_CreatureFeed = 37;

	public const int kType_CreatureEssence = 38;

	public const int kType_CreatureEgg = 39;

	public const int kType_Costume = 43;

	public const int kType_CostumeEffect = 44;

	public const int kType_StarEssenseItem = 46;

	public const int kType_PremiumStarEssenseItem = 47;

	public const int kType_HealthStone = 101;

	public const int kType_OffenseStone = 102;

	public const int kType_DefenseStone = 103;

	public const int kType_ResistanceStone = 104;

	public const int kType_HealthRune = 105;

	public const int kType_OffenseRune = 106;

	public const int kType_DefenseRune = 107;

	public const int kType_ResistanceRune = 108;

	public const int kType_FireRune = 109;

	public const int kType_LightningRune = 110;

	public const int kType_HolyRune = 111;

	public const int kType_DarkRune = 112;

	public const int kType_FireResistRune = 113;

	public const int kType_LightningResistRune = 114;

	public const int kType_HolyResistRune = 115;

	public const int kType_DarkResistRune = 116;

	private int m_nId;

	private int m_nMaxCountPerInventorySlot;

	private Dictionary<int, Item> m_items = new Dictionary<int, Item>();

	public int id => m_nId;

	public int maxCountPerInventorySlot => m_nMaxCountPerInventorySlot;

	public Dictionary<int, Item> items => m_items;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["itemType"]);
		m_nMaxCountPerInventorySlot = Convert.ToInt32(dr["maxCountPerInventorySlot"]);
		if (m_nMaxCountPerInventorySlot <= 0)
		{
			SFLogUtil.Warn(GetType(), "인벤토리슬롯당최대수량이 유효하지 않습니다. m_nMaxCountPerInventorySlot = " + m_nMaxCountPerInventorySlot);
		}
	}

	public void AddItem(Item item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		m_items.Add(item.id, item);
	}
}
