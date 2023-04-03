using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearDisassembleResultPoolEntry : IPickEntry
{
	private MainGearDisassembleResultPool m_pool;

	private int m_nEntryNo;

	private int m_nPoint;

	private Item m_item;

	private int m_nItemCount;

	private bool m_bItemOwned;

	public MainGearDisassembleResultPool pool
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

	public int entryNo => m_nEntryNo;

	public int point => m_nPoint;

	public Item item => m_item;

	public int itemCount => m_nItemCount;

	public bool itemOwned => m_bItemOwned;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nEntryNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nEntryNo = " + m_nEntryNo + ", nItemId = " + nItemId);
		}
		m_nItemCount = Convert.ToInt32(dr["itemCount"]);
		if (m_nItemCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "아이템 수량이 유효하지 않습니다. m_nEntryNo = " + m_nEntryNo + ", m_nItemCount = " + m_nItemCount);
		}
		m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
	}
}
