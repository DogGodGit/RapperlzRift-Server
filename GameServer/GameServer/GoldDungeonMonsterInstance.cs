using System;
using ClientCommon;

namespace GameServer;

public class GoldDungeonMonsterInstance : MonsterInstance
{
	private GoldDungeonStepMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.GoldDungeonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public override bool hitEnabled => IsActivatedMonster();

	public override bool moveEnabled => IsActivatedMonster();

	public override bool skillEnabled => IsActivatedMonster();

	public override bool aggroManaged => IsActivatedMonster();

	public int stepNo => m_arrange.step.no;

	public int activationWaveNo => m_arrange.activationWaveNo;

	public void Init(GoldDungeonInstance goldDungeonInst, GoldDungeonStepMonsterArrange arrange)
	{
		if (goldDungeonInst == null)
		{
			throw new ArgumentNullException("goldDungeonInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(goldDungeonInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	private bool IsActivatedMonster()
	{
		GoldDungeonInstance goldDungeonInstance = (GoldDungeonInstance)m_currentPlace;
		if (goldDungeonInstance.stepNo > stepNo)
		{
			return true;
		}
		if (goldDungeonInstance.stepNo == stepNo && goldDungeonInstance.waveNo >= activationWaveNo)
		{
			return true;
		}
		return false;
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDGoldDungeonMonsterInstance inst = new PDGoldDungeonMonsterInstance();
		inst.stepNo = stepNo;
		inst.activationWaveNo = activationWaveNo;
		return inst;
	}
}
