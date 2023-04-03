using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationAlliance
{
	private Guid m_id = Guid.Empty;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private Dictionary<int, NationInstance> m_nationInsts = new Dictionary<int, NationInstance>();

	public Guid id => m_id;

	public DateTimeOffset regTime => m_regTime;

	public Dictionary<int, NationInstance> nationInsts => m_nationInsts;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		_ = Cache.instance;
		m_id = SFDBUtil.ToGuid(dr["allianceId"], Guid.Empty);
		m_regTime = SFDBUtil.ToDateTimeOffset(dr["regTime"]);
	}

	public void Init(DateTimeOffset time)
	{
		m_id = Guid.NewGuid();
		m_regTime = time;
	}

	public bool IsAlliance(int nNationId1, int nNationId2)
	{
		if (m_nationInsts.ContainsKey(nNationId1) && m_nationInsts.ContainsKey(nNationId2))
		{
			return true;
		}
		return false;
	}

	public int GetAllianceNationId(int nNationId)
	{
		foreach (int nAllianceNationId in m_nationInsts.Keys)
		{
			if (nAllianceNationId != nNationId)
			{
				return nAllianceNationId;
			}
		}
		return 0;
	}

	public NationInstance GetAllianceNation(int nNationId)
	{
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			if (nationInst.nationId != nNationId)
			{
				return nationInst;
			}
		}
		return null;
	}

	public float GetAllianceRenounceAvailableRemainingTime(DateTimeOffset time)
	{
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_regTime, time);
		return Math.Max((float)Resource.instance.nationAllianceRenounceUnavailableDuration - fElapsedTime, 0f);
	}

	public void AddNationInstance(NationInstance nationInst)
	{
		if (nationInst == null)
		{
			throw new ArgumentNullException("nationInst");
		}
		m_nationInsts.Add(nationInst.nationId, nationInst);
		nationInst.SetAlliance(this);
	}

	public PlaceAlliance ToPlaceAlliance()
	{
		return new PlaceAlliance(m_id, m_nationInsts.Keys.ToList());
	}

	public PDNationAlliance ToPDNationAlliance(DateTimeOffset time)
	{
		PDNationAlliance inst = new PDNationAlliance();
		inst.id = (Guid)m_id;
		inst.nationIds = m_nationInsts.Keys.ToArray();
		inst.allianceRenounceAvailableRemainingTime = GetAllianceRenounceAvailableRemainingTime(time);
		return inst;
	}
}
