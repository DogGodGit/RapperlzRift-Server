using System;
using System.Collections.Generic;

namespace GameServer;

public abstract class WarMemoryBaseMonsterInstance : MonsterInstance
{
	protected WarMemoryMonsterAttrFactor m_attrFactor;

	public WarMemoryMonsterAttrFactor attrFactor => m_attrFactor;

	public override bool normalItemLootingEnabled => false;

	protected void InitWarMemoryBaseMonster(WarMemoryInstance warMemoryInst, WarMemoryMonsterAttrFactor attrFactor, Vector3 position, float fRotationY)
	{
		m_attrFactor = attrFactor;
		InitMonsterInstance(warMemoryInst, position, fRotationY);
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override void OnDead()
	{
		base.OnDead();
		foreach (MonsterReceivedDamage damage in m_receivedDamages.Values)
		{
			Hero hero = m_currentPlace.GetHero(damage.attackerId);
			if (hero == null)
			{
				continue;
			}
			List<DropObject> dropObjects = CreateDropObjects();
			if (dropObjects.Count != 0)
			{
				lock (hero.syncObject)
				{
					ProcessHeroDropItemLooting(hero, dropObjects);
				}
			}
		}
	}
}
