using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGearType
{
	public const int kType_Horsewhip = 1;

	public const int kType_Horsearmor = 2;

	public const int kType_Bridle = 3;

	public const int kType_Saddle = 4;

	public const int kType_Stirrups = 5;

	public const int kType_Horseshoe = 6;

	public const int kCount = 6;

	private int m_nId;

	private string m_sNameKey;

	private MountGearSlot m_gearSlot;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public MountGearSlot gearSlot => m_gearSlot;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["type"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		int nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
		m_gearSlot = Resource.instance.GetMountGearSlot(nSlotIndex);
		if (m_gearSlot == null)
		{
			SFLogUtil.Warn(GetType(), "탈것장비슬롯이 존재하지 않습니다. . m_nId = " + m_nId + ", nSlotIndex = " + nSlotIndex);
		}
		m_gearSlot.gearType = this;
	}

	public static bool IsDefined(int nType)
	{
		if (nType != 1 && nType != 2 && nType != 3 && nType != 4 && nType != 5)
		{
			return nType == 6;
		}
		return true;
	}
}
