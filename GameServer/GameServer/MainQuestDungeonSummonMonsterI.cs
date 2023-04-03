using System;
using ClientCommon;

namespace GameServer;

public class MainQuestDungeonSummonMonsterInstance : MonsterInstance
{
	private MainQuestDungeonSummon m_summon;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.MainQuestDungeonSummonMonster;

	public override Monster monster => m_summon.monsterArrange.monster;

	public void Init(MainQuestDungeonInstance mainQuestDungeonInst, MainQuestDungeonSummon summon)
	{
		if (mainQuestDungeonInst == null)
		{
			throw new ArgumentNullException("mainQuestDungeonInst");
		}
		if (summon == null)
		{
			throw new ArgumentNullException("summon");
		}
		m_summon = summon;
		InitMonsterInstance(mainQuestDungeonInst, summon.position, summon.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDMainQuestDungeonSummonMonsterInstance();
	}
}
