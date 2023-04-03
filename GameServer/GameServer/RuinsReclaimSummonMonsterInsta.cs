using System;
using ClientCommon;

namespace GameServer;

public class RuinsReclaimSummonMonsterInstance : MonsterInstance
{
	private RuinsReclaimSummonMonsterArrange m_arrange;

	private RuinsReclaimMonsterAttrFactor m_attrFactor;

	private RuinsReclaimMonsterInstance m_parentMonsterInst;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.RuinsReclaimSummonMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public RuinsReclaimSummonMonsterArrange arrange => m_arrange;

	public RuinsReclaimMonsterAttrFactor attrFactor => m_attrFactor;

	public RuinsReclaimMonsterInstance parentMonsterInst => m_parentMonsterInst;

	public void Init(RuinsReclaimInstance ruinsReclaimInst, RuinsReclaimSummonMonsterArrange arrange, RuinsReclaimMonsterAttrFactor attrFactor, RuinsReclaimMonsterInstance parentMonsterInst)
	{
		if (ruinsReclaimInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimInst");
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
		m_attrFactor = attrFactor;
		m_parentMonsterInst = parentMonsterInst;
		InitMonsterInstance(ruinsReclaimInst, arrange.position, arrange.SelectRotationY());
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		RuinsReclaimInstance ruinsReclaimInst = (RuinsReclaimInstance)m_currentPlace;
		if (!ruinsReclaimInst.isFinished && m_parentMonsterInst.arrange.key == ruinsReclaimInst.ruinsReclaim.lastBossArrangeKey)
		{
			Hero attacker = (Hero)m_lastAttacker;
			Hero heroAttacker = m_currentPlace.GetHero(attacker.id);
			if (heroAttacker != null)
			{
				ruinsReclaimInst.OnSummonMonsterDamage(attacker, m_nLastHPDamage, m_lastDamageTime);
			}
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDRuinsReclaimSummonMonsterInstance();
	}
}
