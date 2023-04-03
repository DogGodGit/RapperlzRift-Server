using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroBiography
{
	private Hero m_hero;

	private Biography m_biography;

	private bool m_bCompleted;

	private HeroBiographyQuest m_quest;

	public Hero hero => m_hero;

	public Biography biography => m_biography;

	public bool completed
	{
		get
		{
			return m_bCompleted;
		}
		set
		{
			m_bCompleted = value;
		}
	}

	public HeroBiographyQuest quest => m_quest;

	public HeroBiography(Hero hero)
		: this(hero, null)
	{
	}

	public HeroBiography(Hero hero, Biography biography)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_biography = biography;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nBiographyId = Convert.ToInt32(dr["biographyId"]);
		m_biography = Resource.instance.GetBiography(nBiographyId);
		if (m_biography == null)
		{
			throw new Exception(string.Concat("존재하지 않는 전기입니다. heroId = ", m_hero.id, ", nBiographyId = ", nBiographyId));
		}
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
	}

	public void SetQuest(HeroBiographyQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
	}

	public PDHeroBiography ToPDHeroBiography()
	{
		PDHeroBiography inst = new PDHeroBiography();
		inst.biographyId = m_biography.id;
		inst.completed = m_bCompleted;
		inst.quest = ((m_quest != null) ? m_quest.ToPDHeroBiograhyQuest() : null);
		return inst;
	}

	public PDHeroBiographyQuestProgressCount ToPDHeroBiographyQuestProgressCount()
	{
		PDHeroBiographyQuestProgressCount inst = new PDHeroBiographyQuestProgressCount();
		inst.biographyId = m_biography.id;
		if (m_quest != null)
		{
			inst.questNo = m_quest.quest.no;
			inst.progressCount = m_quest.progressCount;
		}
		else
		{
			inst.questNo = 0;
			inst.progressCount = 0;
		}
		return inst;
	}

	public static List<PDHeroBiography> ToPDHeroBiographies(IEnumerable<HeroBiography> biographies)
	{
		List<PDHeroBiography> insts = new List<PDHeroBiography>();
		foreach (HeroBiography biography in biographies)
		{
			insts.Add(biography.ToPDHeroBiography());
		}
		return insts;
	}

	public static List<PDHeroBiographyQuestProgressCount> ToPDHeroBiographyQuestProgressCounts(IEnumerable<HeroBiography> biographies)
	{
		List<PDHeroBiographyQuestProgressCount> insts = new List<PDHeroBiographyQuestProgressCount>();
		foreach (HeroBiography biography in biographies)
		{
			insts.Add(biography.ToPDHeroBiographyQuestProgressCount());
		}
		return insts;
	}
}
