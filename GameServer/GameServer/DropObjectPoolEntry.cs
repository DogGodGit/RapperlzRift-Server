using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DropObjectPoolEntry : IPickEntry
{
	public const int kType_MainGear = 1;

	public const int kType_Item = 2;

	private DropObjectPool m_pool;

	private int m_nNo;

	private int m_nPoint;

	private int m_nType;

	private MainGear m_mainGear;

	private bool m_bMainGearOwned;

	private Item m_item;

	private int m_nItemCount;

	private bool m_bItemOwned;

	public DropObjectPool pool => m_pool;

	public int no => m_nNo;

	public int point => m_nPoint;

	public int type => m_nType;

	public MainGear mainGear => m_mainGear;

	public bool mainGearOwned => m_bMainGearOwned;

	public Item item => m_item;

	public int itemCount => m_nItemCount;

	public bool itemOwned => m_bItemOwned;

	int IPickEntry.point => m_nPoint;

	public DropObjectPoolEntry(DropObjectPool pool)
	{
		m_pool = pool;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "포인트가 유효하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo);
		}
		m_nType = Convert.ToInt32(dr["type"]);
		switch (m_nType)
		{
		case 1:
		{
			int nMainGearId = Convert.ToInt32(dr["mainGearId"]);
			if (nMainGearId > 0)
			{
				m_mainGear = Resource.instance.GetMainGear(nMainGearId);
				if (m_mainGear == null)
				{
					SFLogUtil.Warn(GetType(), "메인장비가 존재하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo + ", nMainGearId = " + nMainGearId);
				}
			}
			else
			{
				SFLogUtil.Warn(GetType(), "메인장비ID가 유효하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo + ", nMainGearId = " + nMainGearId);
			}
			m_bMainGearOwned = Convert.ToBoolean(dr["mainGearOwned"]);
			break;
		}
		case 2:
		{
			int nItemId = Convert.ToInt32(dr["itemId"]);
			if (nItemId > 0)
			{
				m_item = Resource.instance.GetItem(nItemId);
				if (m_item == null)
				{
					SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo + ", nItemId = " + nItemId);
				}
			}
			else
			{
				SFLogUtil.Warn(GetType(), "아이템ID가 유효하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo + ", nItemId = " + nItemId);
			}
			m_nItemCount = Convert.ToInt32(dr["itemCount"]);
			if (m_nItemCount <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템수량이 유효하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo + ", m_nItemCount = " + m_nItemCount);
			}
			m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
			break;
		}
		default:
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo + ", m_nType = " + m_nType);
			break;
		}
	}
}
