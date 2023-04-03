using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroBountyHunterQuest
{
	public const int kStatus_Start = 0;

	public const int kStatus_Complete = 1;

	public const int kStatus_Fail = 2;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private BountyHunterQuest m_quest;

	private int m_nItemGrade;

	private int m_nProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	public Guid id => m_id;

	public Hero hero => m_hero;

	public BountyHunterQuest quest => m_quest;

	public int itemGrade => m_nItemGrade;

	public int progressCount => m_nProgressCount;

	public bool objectiveCompleted => m_nProgressCount >= m_quest.targetCount;

	public HeroBountyHunterQuest(Hero hero)
	{
		m_hero = hero;
	}

	public HeroBountyHunterQuest(Hero hero, BountyHunterQuest quest, int nItemGrade)
	{
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_quest = quest;
		m_nItemGrade = nItemGrade;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		int nQuestId = Convert.ToInt32(dr["questId"]);
		m_quest = Resource.instance.GetBountyQuest(nQuestId);
		if (m_quest == null)
		{
			throw new Exception(string.Concat("존재하지 않는 현상금사냥꾼퀘스트입니다. m_id = ", m_id, ", nQuestId = ", nQuestId));
		}
		m_nItemGrade = Convert.ToInt32(dr["questItemGrade"]);
		if (!ItemGrade.IsDefined(m_nItemGrade))
		{
			throw new Exception(string.Concat("아이템등급이 유효하지 않습니다. m_id = ", m_id, ", m_nItemGrade = ", m_nItemGrade));
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
	}

	public bool IncreaseProgressCount(long lnMonsterInstanceId)
	{
		if (m_huntedMonsters.Contains(lnMonsterInstanceId))
		{
			return false;
		}
		m_huntedMonsters.Add(lnMonsterInstanceId);
		m_nProgressCount++;
		ServerEvent.SendBountyHunterQuestUpdated(m_hero.account.peer, m_nProgressCount);
		return true;
	}

	public PDHeroBountyHunterQuest ToPDHeroBountyHunterQuest()
	{
		PDHeroBountyHunterQuest inst = new PDHeroBountyHunterQuest();
		inst.questId = m_quest.id;
		inst.itemGrade = m_nItemGrade;
		inst.progressCount = m_nProgressCount;
		return inst;
	}
}
