using System;
using ClientCommon;

namespace GameServer;

public class ItemDropObject : DropObject
{
	private Item m_item;

	private int m_nCount;

	private bool m_bOwned;

	public override int type => 2;

	public Item item => m_item;

	public int count => m_nCount;

	public bool owned => m_bOwned;

	public ItemDropObject(Item item, int nCount, bool bOwned)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (nCount <= 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		m_item = item;
		m_nCount = nCount;
		m_bOwned = bOwned;
	}

	public override PDDropObject ToPDDropObject()
	{
		PDItemDropObject inst = new PDItemDropObject();
		inst.id = m_item.id;
		inst.count = m_nCount;
		inst.owned = m_bOwned;
		return inst;
	}
}
