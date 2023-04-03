using ClientCommon;

namespace GameServer;

public class AncientRelicNormalMonsterinstance : AncientRelicMonsterInstance
{
	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.AncientRelicNormalMonster;

	protected override void InitAncientRelicMonster()
	{
		m_fMaxHpFactor = m_attrFactor.normalMonsterMaxHpFactor;
		m_fOffenseFactor = m_attrFactor.normalMonsterOffenseFactor;
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDAncientRelicNormalMonsterInstance();
	}
}
