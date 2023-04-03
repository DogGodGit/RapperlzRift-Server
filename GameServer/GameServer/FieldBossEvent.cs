using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FieldBossEvent
{
	private int m_nRequiredHeroLevel;

	private float m_fRewardRadius;

	private List<FieldBossEventSchedule> m_schedules = new List<FieldBossEventSchedule>();

	private Dictionary<int, FieldBoss> m_fieldBosses = new Dictionary<int, FieldBoss>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public float rewardRadius => m_fRewardRadius;

	public List<FieldBossEventSchedule> schedules => m_schedules;

	public Dictionary<int, FieldBoss> fieldBosses => m_fieldBosses;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_fRewardRadius = Convert.ToSingle(dr["rewardRadius"]);
		if (m_fRewardRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "보상반지름이 유효하지 않습니다. m_fRewardRadius = " + m_fRewardRadius);
		}
	}

	public void AddSchedule(FieldBossEventSchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedules.Add(schedule);
	}

	public FieldBossEventSchedule GetScheduleAt(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_schedules.Count)
		{
			return null;
		}
		return m_schedules[nIndex];
	}

	public void AddFieldBoss(FieldBoss fieldBoss)
	{
		if (fieldBoss == null)
		{
			throw new ArgumentNullException("fieldBoss");
		}
		m_fieldBosses.Add(fieldBoss.id, fieldBoss);
	}
}
