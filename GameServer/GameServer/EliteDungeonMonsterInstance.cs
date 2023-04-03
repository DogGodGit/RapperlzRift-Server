using System;
using ClientCommon;

namespace GameServer;

public class EliteDungeonMonsterInstance : EliteMonsterInstance
{
	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.EliteDungeonMonster;

	public void Init(EliteDungeonInstance eliteDungeonInst, EliteMonsterMaster master)
	{
		if (eliteDungeonInst == null)
		{
			throw new ArgumentNullException("eliteDungeonInst");
		}
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_eliteMonster = master.SelectMonster();
		EliteDungeon eliteDungeon = eliteDungeonInst.eliteDungeon;
		InitMonsterInstance(eliteDungeonInst, eliteDungeon.monsterPosition, eliteDungeon.monsterYRotation);
	}

	protected override void OnDead()
	{
		base.OnDead();
		Hero hero = (Hero)m_lastAttacker;
		hero.IncreaseEliteMonsterKillCount(m_eliteMonster);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDEliteDungeonMonsterInstance inst = new PDEliteDungeonMonsterInstance();
		inst.eliteMonsterId = m_eliteMonster.id;
		return inst;
	}
}
