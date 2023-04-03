using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroEliteMonsterKill
{
	private Hero m_hero;

	private EliteMonster m_eliteMonster;

	private int m_nKillCount;

	public Hero hero => m_hero;

	public EliteMonster eliteMonster => m_eliteMonster;

	public int killCount
	{
		get
		{
			return m_nKillCount;
		}
		set
		{
			m_nKillCount = value;
		}
	}

	public HeroEliteMonsterKill(Hero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nEliteMonsterId = Convert.ToInt32(dr["eliteMonsterId"]);
		if (nEliteMonsterId > 0)
		{
			m_eliteMonster = Resource.instance.GetEliteMonster(nEliteMonsterId);
			if (m_eliteMonster == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("정예몬스터가 존재하지 않습니다. heroId = ", m_hero.id, ", nEliteMonsterId = ", nEliteMonsterId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("정예몬스터ID가 유효하지 않습니다. heroId = ", m_hero.id, ", nEliteMonsterId = ", nEliteMonsterId));
		}
		m_nKillCount = Convert.ToInt32(dr["killCount"]);
	}

	public void Init(EliteMonster eliteMonster)
	{
		if (eliteMonster == null)
		{
			throw new ArgumentNullException("eliteMonster");
		}
		m_eliteMonster = eliteMonster;
	}

	public PDHeroEliteMonsterKill ToPDHeroEliteMonsterKill()
	{
		PDHeroEliteMonsterKill inst = new PDHeroEliteMonsterKill();
		inst.eliteMonsterId = m_eliteMonster.id;
		inst.killCount = m_nKillCount;
		return inst;
	}
}
