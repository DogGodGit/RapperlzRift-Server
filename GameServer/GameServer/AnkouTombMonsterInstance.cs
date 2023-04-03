using System;
using ClientCommon;

namespace GameServer;

public class AnkouTombMonsterInstance : MonsterInstance
{
	private AnkouTombMonsterArrange m_arrange;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.AnkouTombMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public AnkouTombMonsterArrange arrange => m_arrange;

	public void Init(AnkouTombInstance ankouTombInst, AnkouTombMonsterArrange arrange, Vector3 position)
	{
		if (ankouTombInst == null)
		{
			throw new ArgumentNullException("ankouTombInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(ankouTombInst, position, 0f);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDAnkouTombMonsterInstance inst = new PDAnkouTombMonsterInstance();
		inst.monsterType = m_arrange.monsterType;
		return inst;
	}
}
