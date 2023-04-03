using System;
using ClientCommon;

namespace GameServer;

public class InfiniteWarMonsterInstance : MonsterInstance
{
	private InfiniteWarMonsterArrange m_arrange;

	private InfiniteWarMonsterAttrFactor m_attrFactor;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.InfiniteWarMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public InfiniteWarMonsterArrange arrange => m_arrange;

	public InfiniteWarMonsterAttrFactor attrFactor => m_attrFactor;

	public void Init(InfiniteWarInstance infiniteWarInst, InfiniteWarMonsterArrange arrange, InfiniteWarMonsterAttrFactor attrFactor)
	{
		if (infiniteWarInst == null)
		{
			throw new ArgumentNullException("infiniteWarInst");
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
		InitMonsterInstance(infiniteWarInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		return new PDInfiniteWarMonsterInstance();
	}
}
