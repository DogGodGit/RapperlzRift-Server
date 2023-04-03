using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class IllustratedBookType
{
	private int m_nType;

	private IllustratedBookCategory m_category;

	private Dictionary<int, IllustratedBook> m_illustratedBooks = new Dictionary<int, IllustratedBook>();

	public int type => m_nType;

	public IllustratedBookCategory category => m_category;

	public Dictionary<int, IllustratedBook> illustratedBooks => m_illustratedBooks;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nType = Convert.ToInt32(dr["type"]);
		int nCategoryId = Convert.ToInt32(dr["categoryId"]);
		if (nCategoryId > 0)
		{
			m_category = Resource.instance.GetIllustratedBookCategory(nCategoryId);
			if (m_category == null)
			{
				SFLogUtil.Warn(GetType(), "카테고리가 존재하지 않습니다. m_nType = " + m_nType + ", nCategoryId = " + nCategoryId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nType = " + m_nType + ", nCategoryId = " + nCategoryId);
		}
	}

	public void AddIllustratedBook(IllustratedBook illustratedBook)
	{
		if (illustratedBook == null)
		{
			throw new ArgumentNullException("illustratedBook");
		}
		m_illustratedBooks.Add(illustratedBook.id, illustratedBook);
	}

	public IllustratedBook GetIllustratedBook(int nId)
	{
		if (!m_illustratedBooks.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
