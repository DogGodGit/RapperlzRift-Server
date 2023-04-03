using System;
using ClientCommon;

namespace GameServer;

public class MainQuestDungeonMonsterInstance : MonsterInstance
{
	private MainQuestDungeonMonsterArrange m_arrange;

	private bool m_bIsUsedSummon;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.MainQuestDungeonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public MainQuestDungeonMonsterArrange arrange => m_arrange;

	public void Init(MainQuestDungeonInstance mainQuestDungeonInst, MainQuestDungeonMonsterArrange arrange)
	{
		if (mainQuestDungeonInst == null)
		{
			throw new ArgumentNullException("mainQuestDungeonInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		InitMonsterInstance(mainQuestDungeonInst, arrange.position, arrange.SelectRotationY());
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		float fSummonMinHpFacter = m_arrange.summonMinHpFactor;
		if (fSummonMinHpFacter > 0f && !m_bIsUsedSummon && m_nHP <= (int)Math.Floor((float)m_nRealMaxHP * fSummonMinHpFacter))
		{
			((MainQuestDungeonInstance)m_currentPlace).SummonBySummonerMonster(this, m_lastDamageTime);
			m_bIsUsedSummon = true;
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDMainQuestDungeonMonsterInstance();
	}
}
