using System;
using System.Data;

namespace GameServer;

public class HeroMainQuestDungeonReward
{
	private int m_nDungeonId;

	private int m_nStep;

	public int dungeonId => m_nDungeonId;

	public int step => m_nStep;

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDungeonId = Convert.ToInt32(dr["dungeonId"]);
		m_nStep = Convert.ToInt32(dr["step"]);
	}

	public void Init(int nDungeonId, int nStep)
	{
		m_nDungeonId = nDungeonId;
		m_nStep = nStep;
	}
}
