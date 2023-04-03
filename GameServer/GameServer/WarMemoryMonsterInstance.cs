using System;
using ClientCommon;

namespace GameServer;

public class WarMemoryMonsterInstance : WarMemoryBaseMonsterInstance
{
	private WarMemoryMonsterArrange m_arrange;

	private bool m_bSummonMonster;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.WarMemoryMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public WarMemoryMonsterArrange arrange => m_arrange;

	public void Init(WarMemoryInstance warMemoryInst, WarMemoryMonsterArrange arrange, WarMemoryMonsterAttrFactor attrFactor)
	{
		if (warMemoryInst == null)
		{
			throw new ArgumentNullException("warMemoryInst");
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
		InitWarMemoryBaseMonster(warMemoryInst, attrFactor, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		WarMemoryInstance warMemoryInst = (WarMemoryInstance)m_currentPlace;
		if (warMemoryInst.isFinished)
		{
			return;
		}
		float fSummonMinHpFactor = m_arrange.summonMinHpFactor;
		if (fSummonMinHpFactor > 0f && !m_bSummonMonster && !base.isDead)
		{
			int nSummonHp = (int)Math.Floor((float)m_nRealMaxHP * fSummonMinHpFactor);
			if (m_nHP < nSummonHp)
			{
				m_bSummonMonster = true;
				warMemoryInst.SummonMonster(this);
			}
		}
	}

	protected override void OnDead()
	{
		base.OnDead();
		WarMemoryInstance warMemoryInst = (WarMemoryInstance)m_currentPlace;
		Hero lastAttacker = (Hero)m_lastAttacker;
		Hero killer = m_currentPlace.GetHero(lastAttacker.id);
		if (killer != null)
		{
			lock (killer.syncObject)
			{
				HeroWarMemoryPoint heroPoint2 = warMemoryInst.GetOrCreateHeroPoint(killer);
				heroPoint2.AddPoint(m_lastDamageTime, m_arrange.killPoint);
				ServerEvent.SendWarMemoryPointAcquisition(killer.account.peer, heroPoint2.point, heroPoint2.lastPointAcquisitionTime.Ticks);
				ServerEvent.SendHeroWarMemoryPointAcquisition(m_currentPlace.GetClientPeers(killer.id), killer.id, heroPoint2.point, heroPoint2.lastPointAcquisitionTime.Ticks);
			}
		}
		foreach (MonsterReceivedDamage damage in m_receivedDamages.Values)
		{
			Guid assistantId = damage.attackerId;
			if (assistantId == lastAttacker.id)
			{
				continue;
			}
			Hero assistant = m_currentPlace.GetHero(assistantId);
			if (assistant != null)
			{
				lock (assistant.syncObject)
				{
					HeroWarMemoryPoint heroPoint = warMemoryInst.GetOrCreateHeroPoint(assistant);
					heroPoint.AddPoint(m_lastDamageTime, m_arrange.assistPoint);
					ServerEvent.SendWarMemoryPointAcquisition(assistant.account.peer, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks);
					ServerEvent.SendHeroWarMemoryPointAcquisition(m_currentPlace.GetClientPeers(assistant.id), assistant.id, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks);
				}
			}
		}
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDWarMemoryMonsterInstance inst = new PDWarMemoryMonsterInstance();
		inst.monsterType = m_arrange.type;
		return inst;
	}
}
