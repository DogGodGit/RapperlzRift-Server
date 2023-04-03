using System;
using ClientCommon;

namespace GameServer;

public class BiographyQuestDungeonMonsterInstance : MonsterInstance
{
	private BiographyQuestMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.BiographQuestDungeonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public BiographyQuestMonsterArrange arrange => m_arrange;

	public void Init(BiographyQuestDungeonInstance biographyQuestDungeonInst, BiographyQuestMonsterArrange arrange)
	{
		if (biographyQuestDungeonInst == null)
		{
			throw new ArgumentNullException("biographyQuestDungeonInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(biographyQuestDungeonInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDBiographyQuestDungeonMonsterInstance inst = new PDBiographyQuestDungeonMonsterInstance();
		inst.arrangeKey = m_arrange.key;
		return inst;
	}
}
