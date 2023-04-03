using System;
using ClientCommon;

namespace GameServer;

public class SoulCoveterMonsterInstance : MonsterInstance
{
	private SoulCoveterMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.SoulCoveterMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public SoulCoveterMonsterArrange arrange => m_arrange;

	public void Init(SoulCoveterInstance soulCoveterInst, SoulCoveterMonsterArrange arrange)
	{
		if (soulCoveterInst == null)
		{
			throw new ArgumentNullException("soulCoveterInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(soulCoveterInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDSoulCoveterMonsterInstance();
	}
}
