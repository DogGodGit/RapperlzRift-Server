using System;
using ClientCommon;

namespace GameServer;

public class DragonNestMonsterInstance : MonsterInstance
{
	private DragonNestMonsterArrange m_arrange;

	private DragonNestMonsterAttrFactor m_attrFactor;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.DragonNestMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public DragonNestMonsterArrange arrange => m_arrange;

	public DragonNestMonsterAttrFactor attrFactor => m_attrFactor;

	public void Init(DragonNestInstance dragonNestInst, DragonNestMonsterArrange arrange, DragonNestMonsterAttrFactor attrFactor)
	{
		if (dragonNestInst == null)
		{
			throw new ArgumentNullException("dragonNestInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		m_arrange = arrange;
		m_attrFactor = attrFactor;
		InitMonsterInstance(dragonNestInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.maxHpFactor);
		m_nRealPhysicalDefense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDDragonNestMonsterInstance inst = new PDDragonNestMonsterInstance();
		inst.monsterType = m_arrange.type;
		return inst;
	}
}
