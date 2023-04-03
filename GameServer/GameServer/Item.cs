using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Item
{
	public const float kItemCoolTimeFactor = 0.9f;

	private int m_nId;

	private ItemType m_type;

	private string m_sNameKey;

	private int m_nGrade;

	private int m_nLevel;

	private int m_nRequiredMinHeroLevel;

	private int m_nRequiredMaxHeroLevel;

	private bool m_bSaleable;

	private int m_nSaleGold;

	private int m_nValue1;

	private int m_nValue2;

	private long m_lnValue1;

	private long m_lnValue2;

	public int id => m_nId;

	public ItemType type => m_type;

	public int grade => m_nGrade;

	public int level => m_nLevel;

	public int requiredMinHeroLevel => m_nRequiredMinHeroLevel;

	public int requiredMaxHeroLevel => m_nRequiredMaxHeroLevel;

	public bool saleable => m_bSaleable;

	public int saleGold => m_nSaleGold;

	public int value1 => m_nValue1;

	public int value2 => m_nValue2;

	public long longValue1 => m_lnValue1;

	public long longValue2 => m_lnValue2;

	public Item(ItemType type)
	{
		m_type = type;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["itemId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (!ItemGrade.IsDefined(m_nGrade))
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. itemId = " + m_nId + ", m_nGrade = " + m_nGrade);
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. itemId = " + m_nId + ", m_nLevel = " + m_nLevel);
		}
		m_nRequiredMinHeroLevel = Convert.ToInt32(dr["requiredMinHeroLevel"]);
		if (m_nRequiredMinHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템 사용가능한 최소영웅레벨이 유효하지 않습니다. itemId = " + m_nId + ", m_nRequiredMinHeroLevel = " + m_nRequiredMinHeroLevel);
		}
		m_nRequiredMaxHeroLevel = Convert.ToInt32(dr["requiredMaxHeroLevel"]);
		if (m_nRequiredMaxHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템 사용가능한 최대영웅레벨이 유효하지 않습니다. itemId = " + m_nId + ", m_nRequiredMaxHeroLevel = " + m_nRequiredMaxHeroLevel);
		}
		m_bSaleable = Convert.ToBoolean(dr["saleable"]);
		if (m_bSaleable)
		{
			m_nSaleGold = Convert.ToInt32(dr["saleGold"]);
			if (m_nSaleGold <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템 판매가격이 유효하지 않습니다. itemId = " + m_nId + ", m_nSaleGold = " + m_nSaleGold);
			}
		}
		m_nValue1 = Convert.ToInt32(dr["value1"]);
		m_nValue2 = Convert.ToInt32(dr["value2"]);
		m_lnValue1 = Convert.ToInt64(dr["longValue1"]);
		m_lnValue2 = Convert.ToInt64(dr["longValue2"]);
		switch (m_type.id)
		{
		case 1:
		case 2:
		case 7:
		case 11:
			if (m_nValue1 <= 0 || m_nValue2 <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템 값이 유효하지 않습니다. itemId = " + m_nId + ", value1 = " + m_nValue1 + ", value2 = " + m_nValue2);
			}
			break;
		case 3:
		case 4:
		case 6:
		case 13:
		case 16:
		case 25:
		case 26:
			if (m_nValue1 <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템 값이 유효하지 않습니다. itemId = " + m_nId + ", value1 = " + m_nValue1);
			}
			break;
		case 101:
		case 102:
		case 103:
		case 104:
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 111:
		case 112:
		case 113:
		case 114:
		case 115:
		case 116:
			if (m_nValue1 <= 0 || m_lnValue1 <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템 값이 유효하지 않습니다. itemId = " + m_nId + ", value1 = " + m_nValue1 + ", longValue1 = " + m_lnValue1);
			}
			break;
		case 18:
		case 19:
		case 20:
		case 21:
			if (m_lnValue1 <= 0)
			{
				SFLogUtil.Warn(GetType(), "아이템 값이 유효하지 않습니다. itemId = " + m_nId + ", longValue1 = " + m_lnValue1);
			}
			break;
		}
	}

	public bool IsUseableHeroLevel(int nLevel)
	{
		if (nLevel <= 0)
		{
			return false;
		}
		if (nLevel < m_nRequiredMinHeroLevel)
		{
			return false;
		}
		if (m_nRequiredMaxHeroLevel > 0 && nLevel > m_nRequiredMaxHeroLevel)
		{
			return false;
		}
		return true;
	}
}
