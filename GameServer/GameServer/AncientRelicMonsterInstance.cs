using System;

namespace GameServer;

public abstract class AncientRelicMonsterInstance : MonsterInstance
{
	protected AncientRelicMonsterArrange m_arrange;

	protected AncientRelicMonsterAttrFactor m_attrFactor;

	protected float m_fMaxHpFactor;

	protected float m_fOffenseFactor;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public AncientRelicMonsterArrange arrange => m_arrange;

	public int point => m_arrange.point;

	public int monsterType => m_arrange.type;

	public void Init(AncientRelicInstance ancientRelicInst, AncientRelicMonsterArrange arrange)
	{
		if (ancientRelicInst == null)
		{
			throw new ArgumentNullException("ancientRelicInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arrange = arrange;
		m_attrFactor = ancientRelicInst.ancientRelic.GetMonsterAttrFactor(ancientRelicInst.averageHeroLevel);
		InitAncientRelicMonster();
		InitMonsterInstance(ancientRelicInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected abstract void InitAncientRelicMonster();

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_fMaxHpFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_fOffenseFactor);
	}
}
