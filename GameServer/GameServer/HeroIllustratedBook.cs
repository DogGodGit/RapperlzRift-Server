using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HeroIllustratedBook
{
	private Hero m_hero;

	private IllustratedBook m_illustratedBook;

	public Hero hero => m_hero;

	public IllustratedBook illustratedBook => m_illustratedBook;

	public HeroIllustratedBook(Hero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nIllustratedBookId = Convert.ToInt32(dr["illustratedBookId"]);
		if (nIllustratedBookId > 0)
		{
			m_illustratedBook = Resource.instance.GetIllustratedBook(nIllustratedBookId);
			if (illustratedBook == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("도감이 존재하지 않습니다. heroId = ", m_hero.id, ", nIllustratedBookId = ", nIllustratedBookId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("도감ID가 유효하지 않습니다. heroId = ", m_hero.id, ", nIllustratedBookId = ", nIllustratedBookId));
		}
	}

	public void Init(IllustratedBook illustratedBook)
	{
		if (illustratedBook == null)
		{
			throw new ArgumentNullException("illustratedBook");
		}
		m_illustratedBook = illustratedBook;
	}
}
