using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroOrdealQuest
{
	private Hero m_hero;

	private OrdealQuest m_quest;

	private bool m_bCompleted;

	private HeroOrdealQuestSlot[] m_slots = new HeroOrdealQuestSlot[Resource.instance.ordealQuestSlotCount];

	public Hero hero => m_hero;

	public int no => m_quest.no;

	public OrdealQuest quest => m_quest;

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

	public bool isObjectiveCompleted
	{
		get
		{
			HeroOrdealQuestSlot[] array = m_slots;
			foreach (HeroOrdealQuestSlot slot in array)
			{
				if (slot.mission != null)
				{
					return false;
				}
			}
			return true;
		}
	}

	public HeroOrdealQuestSlot[] slots => m_slots;

	public HeroOrdealQuest(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		for (int i = 0; i < m_slots.Length; i++)
		{
			m_slots[i] = new HeroOrdealQuestSlot(this, i);
		}
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nQuestNo = Convert.ToInt32(dr["questNo"]);
		m_quest = Resource.instance.GetOrdealQuest(nQuestNo);
		if (m_quest == null)
		{
			throw new Exception("퀘스트가 존재하지 않습니다. nQuestNo = " + nQuestNo);
		}
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
	}

	public void Start(OrdealQuest quest, DateTimeOffset time)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
		OrdealQuestSlot[] array = quest.slots;
		foreach (OrdealQuestSlot slot in array)
		{
			HeroOrdealQuestSlot heroSlot = GetSlot(slot.index);
			heroSlot.StartMission(slot.GetMission(1), time);
		}
	}

	public HeroOrdealQuestSlot GetSlot(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_slots.Length)
		{
			return null;
		}
		return m_slots[nIndex];
	}

	public PDHeroOrdealQuest ToPDHeroOrdealQuest(DateTimeOffset time)
	{
		PDHeroOrdealQuest inst = new PDHeroOrdealQuest();
		inst.questNo = m_quest.no;
		inst.completed = m_bCompleted;
		inst.slots = HeroOrdealQuestSlot.ToPDHeroOrdealQuestSlots(m_slots, time).ToArray();
		return inst;
	}
}
