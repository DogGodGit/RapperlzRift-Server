using System;
using ClientCommon;

namespace GameServer;

public class WarMemorySummonMonsterInstance : WarMemoryBaseMonsterInstance
{
	private WarMemorySummonMonsterArrange m_arrange;

	private WarMemoryMonsterInstance m_parentMonsterInst;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.WarMemorySummonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public WarMemorySummonMonsterArrange arrange => m_arrange;

	public WarMemoryMonsterInstance parentMonsterInst => m_parentMonsterInst;

	public void Init(WarMemoryInstance warMemoryInst, WarMemorySummonMonsterArrange arrange, WarMemoryMonsterAttrFactor attrFactor, WarMemoryMonsterInstance parentMonsterInst)
	{
		if (warMemoryInst == null)
		{
			throw new ArgumentNullException("warMemoryInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		if (parentMonsterInst == null)
		{
			throw new ArgumentNullException("parentMonsterInst");
		}
		m_arrange = arrange;
		m_parentMonsterInst = parentMonsterInst;
		InitWarMemoryBaseMonster(warMemoryInst, attrFactor, arrange.position, arrange.SelectRotationY());
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDWarMemorySummonMonsterInstance();
	}
}
