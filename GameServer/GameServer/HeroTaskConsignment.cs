using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroTaskConsignment
{
	private Guid m_instanceId = Guid.Empty;

	private Hero m_hero;

	private TaskConsignment m_consignment;

	private int m_nUsedExpItemId;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public Guid instanceId => m_instanceId;

	public Hero hero => m_hero;

	public TaskConsignment consignment => m_consignment;

	public int usedExpItemId => m_nUsedExpItemId;

	public DateTimeOffset regTime => m_regTime;

	public HeroTaskConsignment(Hero hero)
		: this(hero, null, 0, DateTimeOffset.MinValue)
	{
	}

	public HeroTaskConsignment(Hero hero, TaskConsignment consignment, int nUsedExpItemId, DateTimeOffset regTime)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_instanceId = Guid.NewGuid();
		m_hero = hero;
		m_consignment = consignment;
		m_nUsedExpItemId = nUsedExpItemId;
		m_regTime = regTime;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_instanceId = (Guid)dr["consignmentInstanceId"];
		int nConsignmentId = Convert.ToInt32(dr["consignmentId"]);
		m_consignment = Resource.instance.GetTaskConsignment(nConsignmentId);
		if (m_consignment == null)
		{
			throw new Exception(string.Concat("존재하지 않는 위탁입니다. heroId = ", m_hero.id, ", nConsignmentId = ", nConsignmentId));
		}
		m_nUsedExpItemId = Convert.ToInt32(dr["usedExpItemId"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
	}

	public float GetRemainingTime(DateTimeOffset currentTime)
	{
		float fElapsedTime = GetElapsedTime(currentTime);
		return Math.Max((float)m_consignment.completionRequiredTime - fElapsedTime, 0f);
	}

	public float GetElapsedTime(DateTimeOffset currentTime)
	{
		return (float)(currentTime - m_regTime).TotalSeconds;
	}

	public PDHeroTaskConsignment ToPDHeroTaskConsignment(DateTimeOffset currentTime)
	{
		PDHeroTaskConsignment inst = new PDHeroTaskConsignment();
		inst.instanceId = (Guid)m_instanceId;
		inst.consignmentId = m_consignment.id;
		inst.usedExpItemId = m_nUsedExpItemId;
		inst.remainingTime = GetRemainingTime(currentTime);
		return inst;
	}

	public static List<PDHeroTaskConsignment> ToPDHeroTaskConsignments(IEnumerable<HeroTaskConsignment> consignments, DateTimeOffset currentTime)
	{
		List<PDHeroTaskConsignment> insts = new List<PDHeroTaskConsignment>();
		foreach (HeroTaskConsignment consignment in consignments)
		{
			insts.Add(consignment.ToPDHeroTaskConsignment(currentTime));
		}
		return insts;
	}
}
