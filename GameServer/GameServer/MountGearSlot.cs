using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGearSlot
{
	public const int kCount = 6;

	private MountGearType m_geartype;

	private int m_nSlotIndex;

	private int m_nOpenHeroLevel;

	public MountGearType gearType
	{
		get
		{
			return m_geartype;
		}
		set
		{
			m_geartype = value;
		}
	}

	public int index => m_nSlotIndex;

	public int openHeroLevel => m_nOpenHeroLevel;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
		if (m_nSlotIndex < 0)
		{
			SFLogUtil.Warn(GetType(), "슬롯 인덱스가 유효하지 않습니다. m_nSlotIndex = " + m_nSlotIndex);
		}
		m_nOpenHeroLevel = Convert.ToInt32(dr["openHeroLevel"]);
		if (m_nOpenHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "개방영웅레벨이 유효하지 않습니다. m_nOpenHeroLevel = " + m_nOpenHeroLevel);
		}
	}
}
