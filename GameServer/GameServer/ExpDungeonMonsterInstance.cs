using System;
using ClientCommon;

namespace GameServer;

public class ExpDungeonMonsterInstance : MonsterInstance
{
	private ExpDungeonMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.ExpDungeonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public ExpDungeonMonsterArrange arrange => m_arrange;

	public void Init(ExpDungeonInstance expDungeonInst, ExpDungeonMonsterArrange arrange)
	{
		if (expDungeonInst == null)
		{
			throw new ArgumentNullException("expDungeonInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(expDungeonInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDExpDungeonMonsterInstance();
	}
}
