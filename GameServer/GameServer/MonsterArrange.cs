using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MonsterArrange
{
	private long m_lnId;

	private Monster m_monster;

	public long id => m_lnId;

	public Monster monster => m_monster;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnId = Convert.ToInt64(dr["monsterArrangeId"]);
		int nMonsterId = Convert.ToInt32(dr["monsterId"]);
		if (nMonsterId > 0)
		{
			m_monster = Resource.instance.GetMonster(nMonsterId);
			if (m_monster == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터가 존재하지 않습니다. nMonsterId = " + nMonsterId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터ID가 유효하지 않습니다. nMonsterId = " + nMonsterId);
		}
	}
}
