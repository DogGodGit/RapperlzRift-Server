using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ItemCompositionRecipe
{
	private Item m_item;

	private Item m_materialItem;

	private int m_nMaterialItemCount;

	private int m_nGold;

	public Item item => m_item;

	public Item materialItem => m_materialItem;

	public int materialItemCount => m_nMaterialItemCount;

	public int gold => m_nGold;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "결과아이템이 존재하지 않습니다. nItemId = " + nItemId);
		}
		int nMaterialItemId = Convert.ToInt32(dr["materialItemId"]);
		m_materialItem = Resource.instance.GetItem(nMaterialItemId);
		if (m_materialItem == null)
		{
			SFLogUtil.Warn(GetType(), "재료아이템이 존재하지 않습니다. nMaterialItemId = " + nMaterialItemId);
		}
		m_nMaterialItemCount = Convert.ToInt32(dr["materialItemCount"]);
		m_nGold = Convert.ToInt32(dr["gold"]);
	}
}
