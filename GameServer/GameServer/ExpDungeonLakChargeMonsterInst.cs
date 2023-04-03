using System;
using ClientCommon;

namespace GameServer;

public class ExpDungeonLakChargeMonsterInstance : MonsterInstance
{
	private ExpDungeonDifficultyWave m_wave;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.ExpDungeonLakChargeMonster;

	public override Monster monster => m_wave.lakChargeMonsterArrange.monster;

	public int lakChargeAmount => m_wave.lakChargeAmount;

	public void Init(ExpDungeonInstance expDungeonInst, ExpDungeonDifficultyWave wave)
	{
		if (expDungeonInst == null)
		{
			throw new ArgumentNullException("expDungeonInst");
		}
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_wave = wave;
		InitMonsterInstance(expDungeonInst, wave.lakChargeMonsterPosition, wave.SelectRotationY());
	}

	protected override void OnDead()
	{
		base.OnDead();
		if (m_lastAttacker is Hero attacker)
		{
			attacker.AddLak(lakChargeAmount);
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDExpDungeonLakChargeMonsterInstance();
	}
}
