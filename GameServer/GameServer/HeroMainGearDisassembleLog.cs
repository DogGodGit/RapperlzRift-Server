using System;
using System.Collections.Generic;

namespace GameServer;

public class HeroMainGearDisassembleLog
{
	private Guid m_logId = Guid.Empty;

	private HeroMainGear m_heroMainGear;

	private List<HeroMainGearDisassembleDetailLog> m_detailLogs = new List<HeroMainGearDisassembleDetailLog>();

	public Guid id => m_logId;

	public HeroMainGear heroMainGear => m_heroMainGear;

	public List<HeroMainGearDisassembleDetailLog> detailLogs => m_detailLogs;

	public HeroMainGearDisassembleLog(HeroMainGear heroMainGear)
	{
		m_logId = Guid.NewGuid();
		m_heroMainGear = heroMainGear;
	}

	public void AddDetailLog(int nItemId, int nItemCount, bool bItemOwned)
	{
		m_detailLogs.Add(new HeroMainGearDisassembleDetailLog(m_detailLogs.Count + 1, nItemId, nItemCount, bItemOwned));
	}
}
