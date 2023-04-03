using System;

namespace GameServer;

public abstract class WisdomTempleMonsterInstance : MonsterInstance
{
	protected WisdomTempleMonsterAttrFactor m_attrFactor;

	public WisdomTempleMonsterAttrFactor attrFactor => m_attrFactor;

	protected void InitWisdomTempleMonster(WisdomTempleInstance wisdomTempleInst, WisdomTempleMonsterAttrFactor monsterAttrFactor, Vector3 position, float fRotationY)
	{
		m_attrFactor = monsterAttrFactor;
		InitMonsterInstance(wisdomTempleInst, position, fRotationY);
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}
}
