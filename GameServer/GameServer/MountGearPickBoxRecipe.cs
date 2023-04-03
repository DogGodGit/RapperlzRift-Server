using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGearPickBoxRecipe
{
	private Item m_resultItem;

	private int m_nRequiredHeroLevel;

	private int m_nGold;

	private bool m_bResultItemOwned;

	private Item m_materialItem1;

	private int m_materialItem1Count;

	private Item m_materialItem2;

	private int m_materialItem2Count;

	private Item m_materialItem3;

	private int m_materialItem3Count;

	private Item m_materialItem4;

	private int m_materialItem4Count;

	public Item resultItem => m_resultItem;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int gold => m_nGold;

	public bool resultItemOwned => m_bResultItemOwned;

	public Item materialItem1 => m_materialItem1;

	public int materialItem1Count => m_materialItem1Count;

	public Item materialItem2 => m_materialItem2;

	public int materialItem2Count => m_materialItem2Count;

	public Item materialItem3 => m_materialItem3;

	public int materialItem3Count => m_materialItem3Count;

	public Item materialItem4 => m_materialItem4;

	public int materialItem4Count => m_materialItem4Count;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_resultItem = Resource.instance.GetItem(nItemId);
		if (m_resultItem == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. nItemId = " + nItemId);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. nItemId = " + nItemId + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nGold = Convert.ToInt32(dr["gold"]);
		if (m_nGold < 0)
		{
			SFLogUtil.Warn(GetType(), "골드가 유효하지 않습니다. nItemId = " + nItemId + ",m_nGold = " + m_nGold);
		}
		m_bResultItemOwned = Convert.ToBoolean(dr["owned"]);
		int nMaterialItem1Id = Convert.ToInt32(dr["materialItem1Id"]);
		m_materialItem1 = Resource.instance.GetItem(nMaterialItem1Id);
		if (m_materialItem1 == null)
		{
			SFLogUtil.Warn(GetType(), "재료아이템1이 존재하지 않습니다. nItemId = " + nItemId + ", nMaterialItem1Id = " + nMaterialItem1Id);
		}
		m_materialItem1Count = Convert.ToInt32(dr["materialItem1Count"]);
		if (m_materialItem1Count < 0)
		{
			SFLogUtil.Warn(GetType(), "재료아이템1 수량이 유효하지 않습니다. nItemId = " + nItemId + ", m_materialItem1Count = " + m_materialItem1Count);
		}
		int nMaterialItem2Id = Convert.ToInt32(dr["materialItem2Id"]);
		m_materialItem2 = Resource.instance.GetItem(nMaterialItem2Id);
		if (m_materialItem2 == null)
		{
			SFLogUtil.Warn(GetType(), "재료아이템2이 존재하지 않습니다. nItemId = " + nItemId + ", nMaterialItem2Id = " + nMaterialItem2Id);
		}
		m_materialItem2Count = Convert.ToInt32(dr["materialItem2Count"]);
		if (m_materialItem2Count < 0)
		{
			SFLogUtil.Warn(GetType(), "재료아이템2 수량이 유효하지 않습니다. nItemId = " + nItemId + ", m_materialItem2Count = " + m_materialItem2Count);
		}
		int nMaterialItem3Id = Convert.ToInt32(dr["materialItem3Id"]);
		m_materialItem3 = Resource.instance.GetItem(nMaterialItem3Id);
		if (m_materialItem3 == null)
		{
			SFLogUtil.Warn(GetType(), "재료아이템3이 존재하지 않습니다. nItemId = " + nItemId + ", nMaterialItem3Id = " + nMaterialItem3Id);
		}
		m_materialItem3Count = Convert.ToInt32(dr["materialItem3Count"]);
		if (m_materialItem3Count < 0)
		{
			SFLogUtil.Warn(GetType(), "재료아이템3 수량이 유효하지 않습니다. nItemId = " + nItemId + ", m_materialItem3Count = " + m_materialItem3Count);
		}
		int nMaterialItem4Id = Convert.ToInt32(dr["materialItem4Id"]);
		m_materialItem4 = Resource.instance.GetItem(nMaterialItem4Id);
		if (m_materialItem4 == null)
		{
			SFLogUtil.Warn(GetType(), "재료아이템4이 존재하지 않습니다. nItemId = " + nItemId + ", nMaterialItem4Id = " + nMaterialItem4Id);
		}
		m_materialItem4Count = Convert.ToInt32(dr["materialItem4Count"]);
		if (m_materialItem4Count < 0)
		{
			SFLogUtil.Warn(GetType(), "재료아이템4 수량이 유효하지 않습니다. nItemId = " + nItemId + ", m_materialItem4Count = " + m_materialItem4Count);
		}
		if (m_materialItem1 == m_materialItem2 || m_materialItem1 == m_materialItem3 || m_materialItem1 == m_materialItem4 || m_materialItem2 == m_materialItem3 || m_materialItem2 == m_materialItem4 || m_materialItem3 == m_materialItem4)
		{
			SFLogUtil.Warn(GetType(), "동일한 재료아이템이 존재합니다. nItemId = " + nItemId + ", nMaterialItem1Id = " + nMaterialItem1Id + ", nMaterialItem2Id = " + nMaterialItem2Id + ", nMaterialItem3Id = " + nMaterialItem3Id + ", nMaterialItem4Id = " + nMaterialItem4Id);
		}
	}
}
