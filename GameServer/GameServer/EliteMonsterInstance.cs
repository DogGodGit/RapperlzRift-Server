namespace GameServer;

public abstract class EliteMonsterInstance : MonsterInstance
{
	protected EliteMonster m_eliteMonster;

	public override Monster monster => m_eliteMonster.monsterArrange.monster;

	public EliteMonster eliteMonster => m_eliteMonster;

	public int eliteMonsterId => m_eliteMonster.id;

	public EliteMonsterMaster eliteMonsterMaster => m_eliteMonster.master;

	public int eliteMonsterMasterId => eliteMonsterMaster.id;
}
