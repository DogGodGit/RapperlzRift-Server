using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroOrdealQuestSlot
{
	private HeroOrdealQuest m_quest;

	private int m_nIndex;

	private OrdealQuestMission m_mission;

	private int m_nProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public HeroOrdealQuest quest => m_quest;

	public int index => m_nIndex;

	public OrdealQuestMission mission => m_mission;

	public int progressCount => m_nProgressCount;

	public DateTimeOffset regTime => m_regTime;

	public HeroOrdealQuestSlot(HeroOrdealQuest quest, int nIndex)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
		m_nIndex = nIndex;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nMissionNo = Convert.ToInt32(dr["missionNo"]);
		m_mission = m_quest.quest.GetSlot(m_nIndex).GetMission(nMissionNo);
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
	}

	public void StartMission(OrdealQuestMission mission, DateTimeOffset regTime)
	{
		m_mission = mission;
		m_nProgressCount = 0;
		m_huntedMonsters.Clear();
		m_regTime = regTime;
	}

	public void CompleteMission()
	{
		m_mission = null;
		m_nProgressCount = 0;
		m_huntedMonsters.Clear();
		m_regTime = DateTimeOffset.MinValue;
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		if (m_mission == null)
		{
			return 0f;
		}
		if (m_mission.autoCompletionRequiredTime <= 0)
		{
			return 0f;
		}
		float fElapsedTime = (float)(time - m_regTime).TotalSeconds;
		return Math.Max((float)m_mission.autoCompletionRequiredTime - fElapsedTime, 0f);
	}

	public void IncreaseProgressCount(int nProgressCount)
	{
		m_nProgressCount += nProgressCount;
		if (m_nProgressCount > m_mission.targetCount)
		{
			m_nProgressCount = m_mission.targetCount;
		}
	}

	public bool IncreaseProgressCountByMonsterHuntType(long lnMonsterInstanceId)
	{
		if (!m_huntedMonsters.Add(lnMonsterInstanceId))
		{
			return false;
		}
		IncreaseProgressCount(1);
		return true;
	}

	public bool IsObjectiveCompleted(DateTimeOffset time)
	{
		_ = m_quest.hero;
		if (m_mission == null)
		{
			return false;
		}
		if (GetRealProgressCount() >= m_mission.targetCount)
		{
			return true;
		}
		if (m_mission.autoCompletionRequiredTime > 0)
		{
			return GetRemainingTime(time) <= 0f;
		}
		return false;
	}

	private int GetRealProgressCount()
	{
		Hero hero = m_quest.hero;
		return m_mission.type switch
		{
			OrdealQuestMissionType.SubGearSoulStoneTotalLevel => hero.totalMountedSoulstoneLevel, 
			OrdealQuestMissionType.MountLevel => hero.maxMountLevel, 
			OrdealQuestMissionType.CreatureLevel => hero.maxCreatureLevel, 
			OrdealQuestMissionType.WingLevel => hero.wingStepLevel.level, 
			OrdealQuestMissionType.RankNo => hero.rankNo, 
			OrdealQuestMissionType.AccomplishmentPoint => hero.accomplishmentPoint, 
			_ => m_nProgressCount, 
		};
	}

	public PDHeroOrdealQuestSlot ToPDHeroOrdealQuestSlot(DateTimeOffset time)
	{
		PDHeroOrdealQuestSlot inst = new PDHeroOrdealQuestSlot();
		inst.index = m_nIndex;
		inst.missionNo = ((m_mission != null) ? m_mission.no : 0);
		inst.progressCount = m_nProgressCount;
		inst.remainingTime = GetRemainingTime(time);
		return inst;
	}

	public PDHeroOrdealQuestSlotProgressCount ToPDHeroOrdealQuestSlotProgressCount()
	{
		PDHeroOrdealQuestSlotProgressCount inst = new PDHeroOrdealQuestSlotProgressCount();
		inst.index = m_mission.slot.index;
		inst.progressCount = m_nProgressCount;
		return inst;
	}

	public static List<PDHeroOrdealQuestSlot> ToPDHeroOrdealQuestSlots(IEnumerable<HeroOrdealQuestSlot> slots, DateTimeOffset time)
	{
		List<PDHeroOrdealQuestSlot> insts = new List<PDHeroOrdealQuestSlot>();
		foreach (HeroOrdealQuestSlot slot in slots)
		{
			insts.Add(slot.ToPDHeroOrdealQuestSlot(time));
		}
		return insts;
	}

	public static List<PDHeroOrdealQuestSlotProgressCount> ToPDHeroOrdealQuestSlotProgressCounts(IEnumerable<HeroOrdealQuestSlot> slots)
	{
		List<PDHeroOrdealQuestSlotProgressCount> insts = new List<PDHeroOrdealQuestSlotProgressCount>();
		foreach (HeroOrdealQuestSlot slot in slots)
		{
			insts.Add(slot.ToPDHeroOrdealQuestSlotProgressCount());
		}
		return insts;
	}
}
