using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class IllustratedBookCategory
{
	public const int kCategoryId_Scenery = 1;

	public const int kCategoryId_Life = 2;

	public const int kCategoryId_RareGift = 3;

	public const int kCategoryId_Commemoration = 4;

	private int m_nId;

	private Dictionary<int, IllustratedBookType> m_types = new Dictionary<int, IllustratedBookType>();

	public int id => m_nId;

	public Dictionary<int, IllustratedBookType> types => m_types;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["categoryId"]);
		if (!IsDefinedCategory(m_nId))
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
	}

	public void AddType(IllustratedBookType type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_types.Add(type.type, type);
	}

	public IllustratedBookType GetType(int nType)
	{
		if (!m_types.TryGetValue(nType, out var value))
		{
			return null;
		}
		return value;
	}

	public static bool IsDefinedCategory(int nId)
	{
		if (nId != 1 && nId != 2 && nId != 3)
		{
			return nId == 4;
		}
		return true;
	}
}
