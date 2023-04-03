using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroSubQuest
{
	private Hero m_hero;

	private SubQuest m_quest;

	private int m_nProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	private HeroSubQuestStatus m_status;

	public Hero hero => m_hero;

	public SubQuest quest => m_quest;

	public int progressCount => m_nProgressCount;

	public bool isObjectiveCompleted => m_nProgressCount >= m_quest.targetCount;

	public HeroSubQuestStatus status => m_status;

	public bool isAccepted => m_status == HeroSubQuestStatus.Accepted;

	public bool isCompleted => m_status == HeroSubQuestStatus.Completed;

	public bool isAbandoned => m_status == HeroSubQuestStatus.Abandoned;

	public HeroSubQuest(Hero hero)
		: this(hero, null)
	{
	}

	public HeroSubQuest(Hero hero, SubQuest quest)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_quest = quest;
		m_status = HeroSubQuestStatus.Accepted;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nQuestId = Convert.ToInt32(dr["subQuestId"]);
		m_quest = Resource.instance.GetSubQuest(nQuestId);
		if (m_quest == null)
		{
			throw new Exception(string.Concat("퀘스트가 존재하지 않습니다. heroId = ", m_hero.id, ", nQuestId = ", nQuestId));
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		int nStatus = Convert.ToInt32(dr["status"]);
		if (Enum.IsDefined(typeof(HeroSubQuestStatus), nStatus))
		{
			m_status = (HeroSubQuestStatus)nStatus;
		}
	}

	public void Abandon()
	{
		m_status = HeroSubQuestStatus.Abandoned;
		m_hero.RemoveCurrentSubQuest(m_quest.id);
	}

	public void Complete()
	{
		m_status = HeroSubQuestStatus.Completed;
		m_hero.RemoveCurrentSubQuest(m_quest.id);
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
	}

	public bool IncreaseProgressCountBySubQuest(long lnMonsterInstanceId)
	{
		if (!m_huntedMonsters.Add(lnMonsterInstanceId))
		{
			return false;
		}
		IncreaseProgressCount();
		return true;
	}

	public PDHeroSubQuest ToPDHeroSubQuest()
	{
		PDHeroSubQuest inst = new PDHeroSubQuest();
		inst.questId = m_quest.id;
		inst.progressCount = m_nProgressCount;
		inst.status = (int)m_status;
		return inst;
	}

	public PDHeroSubQuestProgressCount ToPDHeroSubQuestProgressCount()
	{
		PDHeroSubQuestProgressCount inst = new PDHeroSubQuestProgressCount();
		inst.questId = m_quest.id;
		inst.progressCount = m_nProgressCount;
		return inst;
	}

	public static List<PDHeroSubQuest> ToPDHeroSubQuests(IEnumerable<HeroSubQuest> quests)
	{
		List<PDHeroSubQuest> insts = new List<PDHeroSubQuest>();
		foreach (HeroSubQuest quest in quests)
		{
			insts.Add(quest.ToPDHeroSubQuest());
		}
		return insts;
	}

	public static List<PDHeroSubQuestProgressCount> ToPDHeroSubQuestProgressCounts(IEnumerable<HeroSubQuest> quests)
	{
		List<PDHeroSubQuestProgressCount> insts = new List<PDHeroSubQuestProgressCount>();
		foreach (HeroSubQuest quest in quests)
		{
			insts.Add(quest.ToPDHeroSubQuestProgressCount());
		}
		return insts;
	}
}
