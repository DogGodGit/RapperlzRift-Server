using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroBiographyQuest
{
	private HeroBiography m_biography;

	private BiographyQuest m_quest;

	private int m_nProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	private bool m_bCompleted;

	public HeroBiography biography => m_biography;

	public BiographyQuest quest => m_quest;

	public int progressCount => m_nProgressCount;

	public bool isObjectiveCompleted => m_nProgressCount >= m_quest.targetCount;

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

	public bool isLastQuest => m_quest.isLastQuest;

	public HeroBiographyQuest(HeroBiography biography)
		: this(biography, null)
	{
	}

	public HeroBiographyQuest(HeroBiography biography, BiographyQuest quest)
	{
		if (biography == null)
		{
			throw new ArgumentNullException("biography");
		}
		m_biography = biography;
		m_quest = quest;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nQuestNo = Convert.ToInt32(dr["questNo"]);
		m_quest = m_biography.biography.GetQuest(nQuestNo);
		if (m_quest == null)
		{
			throw new Exception("퀘스트가 존재하지 않습니다. nQuestNo = " + nQuestNo);
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
	}

	public bool IncreaseProgressCountByHunt(long lnMonsterInstanceId)
	{
		if (!m_huntedMonsters.Add(lnMonsterInstanceId))
		{
			return false;
		}
		IncreaseProgressCount();
		return true;
	}

	public PDHeroBiograhyQuest ToPDHeroBiograhyQuest()
	{
		PDHeroBiograhyQuest inst = new PDHeroBiograhyQuest();
		inst.questNo = m_quest.no;
		inst.progressCount = m_nProgressCount;
		inst.completed = m_bCompleted;
		return inst;
	}
}
