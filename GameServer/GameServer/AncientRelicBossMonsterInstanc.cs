using ClientCommon;

namespace GameServer;

public class AncientRelicBossMonsterInstance : AncientRelicMonsterInstance
{
	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.AncientRelicBossMonster;

	protected override void InitAncientRelicMonster()
	{
		m_fMaxHpFactor = m_attrFactor.bossMonsterMaxHpFactor;
		m_fOffenseFactor = m_attrFactor.bossMonsterOffenseFactor;
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDAncientRelicBossMonsterInstance();
	}
}
