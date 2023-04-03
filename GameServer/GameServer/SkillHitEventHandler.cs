using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class SkillHitEventHandler : InGameEventHandler<CEBSkillHitEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace == null)
		{
			return;
		}
		if (m_body == null)
		{
			throw new EventHandleException("바디가 null입니다.");
		}
		Guid placeInstanceId = (Guid)m_body.placeInstanceId;
		if (placeInstanceId == Guid.Empty)
		{
			throw new EventHandleException("장소ID가 유효하지 않습니다");
		}
		if (placeInstanceId != currentPlace.instanceId)
		{
			return;
		}
		Guid heroId = (Guid)m_body.heroId;
		if (heroId == Guid.Empty)
		{
			throw new EventHandleException("영웅ID가 유효하지 않습니다.");
		}
		Hero attacker = ((heroId == m_myHero.id) ? m_myHero : currentPlace.GetHero(heroId));
		if (attacker == null)
		{
			throw new EventHandleException("공격자가 존재하지 않습니다. heroId = " + heroId);
		}
		if (attacker.controller.id != m_myHero.id)
		{
			throw new EventHandleException("공격자의 제어자가 내가 아닙니다. heroId = " + heroId);
		}
		int nSkillId = m_body.skillId;
		int nChainSkillId = m_body.chainSkillId;
		int nHitId = m_body.hitId;
		PDHitTarget[] rawTargets = m_body.targets;
		HeroSkill heroSkill = attacker.GetSkill(nSkillId);
		if (heroSkill == null)
		{
			throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		if (heroSkill.currentHitId < 0)
		{
			throw new EventHandleException("해당 스킬 캐스팅을 시전하지 않았습니다.");
		}
		JobSkill jobSkill = heroSkill.skill;
		SkillHit hit = null;
		float fHitValidationRadius = 0f;
		if (jobSkill.formType == 1)
		{
			JobChainSkill jobChainSkill = jobSkill.GetChainSkill(nChainSkillId);
			if (jobChainSkill == null)
			{
				throw new EventHandleException("직업연계스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId);
			}
			if (heroSkill.currentChainSkillId != nChainSkillId)
			{
				throw new EventHandleException("해당 직업연계스킬을 사용할 수 없습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", heroSkill.currentChainSkillId = " + heroSkill.currentChainSkillId);
			}
			hit = jobChainSkill.GetHit(nHitId);
			if (hit == null)
			{
				throw new EventHandleException("연계스킬적중ID가 유효하지 않습니다. nHitId = " + nHitId);
			}
			fHitValidationRadius = jobChainSkill.hitValidationRadius;
		}
		else
		{
			hit = jobSkill.GetHit(nHitId);
			if (hit == null)
			{
				throw new EventHandleException("스킬적중ID가 유효하지 않습니다. nHitId = " + nHitId);
			}
			nChainSkillId = 0;
			fHitValidationRadius = jobSkill.hitValidationRadius;
		}
		if (!attacker.skillEnabled)
		{
			return;
		}
		if (nHitId <= heroSkill.currentHitId)
		{
			throw new EventHandleException("적중ID가 유효하지 않습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", nHitId = " + nHitId + ", heroSkill.currentHitId = " + heroSkill.currentHitId);
		}
		heroSkill.currentHitId = nHitId;
		Offense offense = heroSkill.MakeOffense();
		List<PDHitTarget> targets = new List<PDHitTarget>();
		HashSet<Guid> heroTargets = new HashSet<Guid>();
		HashSet<long> monsterTargets = new HashSet<long>();
		HashSet<long> cartTargets = new HashSet<long>();
		PDHitTarget[] array = rawTargets;
		foreach (PDHitTarget rawTarget in array)
		{
			if (rawTarget == null)
			{
				continue;
			}
			if (rawTarget.type == 1)
			{
				PDHeroHitTarget heroTarget = (PDHeroHitTarget)rawTarget;
				if (heroTargets.Add((Guid)heroTarget.heroId))
				{
					targets.Add(heroTarget);
				}
			}
			else if (rawTarget.type == 2)
			{
				PDMonsterHitTarget monsterTarget = (PDMonsterHitTarget)rawTarget;
				if (monsterTargets.Add(monsterTarget.monsterInstanceId))
				{
					targets.Add(monsterTarget);
				}
			}
			else
			{
				PDCartHitTarget cartTarget = (PDCartHitTarget)rawTarget;
				if (cartTargets.Add(cartTarget.cartInstanceId))
				{
					targets.Add(cartTarget);
				}
			}
		}
		bool bIsHit = false;
		int nLevel = m_myHero.level;
		foreach (PDHitTarget target in targets)
		{
			if (attacker.isDead)
			{
				break;
			}
			OffenseHit offenseHit = new OffenseHit(offense, nHitId);
			if (target.type == 1)
			{
				if (m_myHero.isSafeMode || m_myHero.isDistorting || jobSkill.heroHitType == 0)
				{
					continue;
				}
				PDHeroHitTarget heroTarget2 = (PDHeroHitTarget)target;
				if (jobSkill.heroHitType == 1 && (Guid)heroTarget2.heroId != heroSkill.targetHeroId)
				{
					continue;
				}
				Hero hero = currentPlace.GetHero((Guid)heroTarget2.heroId);
				if (hero == null)
				{
					continue;
				}
				lock (hero.syncObject)
				{
					if (BattleUtil.IsHit(attacker.position, fHitValidationRadius * 1.1f, hero.position, hero.radius) && currentPlace.ProcessHeroSkillHit_Hero(offenseHit, nLevel, hero))
					{
						bIsHit = true;
					}
				}
			}
			else if (target.type == 2)
			{
				PDMonsterHitTarget monsterTarget2 = (PDMonsterHitTarget)target;
				MonsterInstance monsterInst = currentPlace.GetMonster(monsterTarget2.monsterInstanceId);
				if (monsterInst != null && BattleUtil.IsHit(attacker.position, fHitValidationRadius * 1.1f, monsterInst.position, monsterInst.monster.radius) && currentPlace.ProcessHeroSkillHit_Monster(offenseHit, monsterInst))
				{
					bIsHit = true;
				}
			}
			else if (target.type == 3 && currentPlace is ContinentInstance continentInst)
			{
				PDCartHitTarget cartTarget2 = (PDCartHitTarget)target;
				CartInstance cartInst = continentInst.GetCartInstance(cartTarget2.cartInstanceId);
				if (cartInst != null && BattleUtil.IsHit(attacker.position, fHitValidationRadius * 1.1f, cartInst.position, cartInst.radius) && continentInst.ProcessHeroSkillHit_Cart(offenseHit, cartInst))
				{
					bIsHit = true;
				}
			}
		}
		if (!bIsHit)
		{
			return;
		}
		attacker.AddLak(hit.acquireLak);
		foreach (AbnormalStateEffect effect in attacker.abnormalStateEffects.Values)
		{
			effect.AddDuration();
		}
	}
}
