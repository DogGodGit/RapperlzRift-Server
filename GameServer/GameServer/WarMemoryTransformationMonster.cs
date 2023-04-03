using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class WarMemoryTransformationMonsterSkillCastEventHandler : InGameEventHandler<CEBWarMemoryTransformationMonsterSkillCastEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			if (m_body == null)
			{
				throw new EventHandleException("body가 null입니다.");
			}
			Guid placeInstanceId = (Guid)m_body.placeInstanceId;
			int nSkillId = m_body.skillId;
			Vector3 skillTargetPosition = m_body.skillTargetPosition;
			if (placeInstanceId == Guid.Empty)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다.");
			}
			if (placeInstanceId != currentPlace.instanceId)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다. placeInstanceId = " + placeInstanceId);
			}
			if (!m_myHero.isTransformWarMemoryMonster)
			{
				throw new EventHandleException("영웅이 전쟁의기억 몬스터변신중이 아닙니다.");
			}
			if (!m_myHero.skillEnabled)
			{
				throw new EventHandleException("영웅이 스킬을 사용할 수 있는 상태가 아닙니다.");
			}
			HeroWarMemoryTransformationMonsterSkill warMemoryTransformationMonsterSkill = m_myHero.GetWarMemoryTransformationMonsterSkill(nSkillId);
			if (warMemoryTransformationMonsterSkill == null)
			{
				throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
			}
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			if (!warMemoryTransformationMonsterSkill.IsExpiredSkillCoolTime(currentTime))
			{
				throw new EventHandleException("쿨타임이 만료되지 않았습니다. nSkillId = " + nSkillId);
			}
			MonsterSkill monsterSkill = warMemoryTransformationMonsterSkill.monsterSkill;
			float fCastRange = monsterSkill.castRange * 1.1f;
			if (!currentPlace.Contains(skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 유효하지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			if (fCastRange > 0f && monsterSkill.skillType == 2 && !MathUtil.SphereContains(m_myHero.position, fCastRange, skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 스킬 시전거리내에 있지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			warMemoryTransformationMonsterSkill.castTime = currentTime;
			warMemoryTransformationMonsterSkill.currentHitId = 0;
			m_myHero.StartBattleMode(currentTime);
			ServerEvent.SendHeroWarMemoryTransformationMonsterSkillCast(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, nSkillId, skillTargetPosition);
			if (monsterSkill.skillType == 2)
			{
				SkillEffect effect = new SkillEffect();
				effect.Init(currentPlace, warMemoryTransformationMonsterSkill.MakeOffense(), skillTargetPosition, Guid.Empty, m_myHero.level);
				currentPlace.AddSkillEffect(effect);
			}
		}
	}
}
public class WarMemoryTransformationMonsterSkillHitEventHandler : InGameEventHandler<CEBWarMemoryTransformationMonsterSkillHitEventBody>
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
			throw new EventHandleException("body가 null입니다.");
		}
		Guid placeInstanceId = (Guid)m_body.placeInstanceId;
		int nSkillId = m_body.skillId;
		int nHitId = m_body.hitId;
		PDHitTarget[] rawTargets = m_body.targets;
		if (placeInstanceId == Guid.Empty)
		{
			throw new EventHandleException("장소ID가 유효하지 않습니다.");
		}
		if (placeInstanceId != currentPlace.instanceId)
		{
			throw new EventHandleException("장소ID가 유효하지 않습니다. placeInstanceId = " + placeInstanceId);
		}
		if (!m_myHero.isTransformWarMemoryMonster)
		{
			throw new EventHandleException("영웅이 전쟁의기억 몬스터변신중이 아닙니다.");
		}
		if (!m_myHero.skillEnabled)
		{
			throw new EventHandleException("영웅이 스킬을 사용할 수 있는 상태가 아닙니다.");
		}
		HeroWarMemoryTransformationMonsterSkill warMemoryTransformationMonsterSkill = m_myHero.GetWarMemoryTransformationMonsterSkill(nSkillId);
		if (warMemoryTransformationMonsterSkill == null)
		{
			throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		if (warMemoryTransformationMonsterSkill.currentHitId < 0)
		{
			throw new EventHandleException("해당 스킬 캐스팅을 시전하지 않았습니다.");
		}
		MonsterSkill monsterSkill = warMemoryTransformationMonsterSkill.monsterSkill;
		SkillHit hit = monsterSkill.GetHit(nHitId);
		if (hit == null)
		{
			throw new EventHandleException("스킬적중ID가 유효하지 않습니다. nHitId = " + nHitId);
		}
		if (nHitId <= warMemoryTransformationMonsterSkill.currentHitId)
		{
			throw new EventHandleException("적중ID가 유효하지 않습니다. nSkillId = " + nSkillId + ", nHitId = " + nHitId + ", warMemoryTransformationMonsterSkillCurrentHitId = " + warMemoryTransformationMonsterSkill.currentHitId);
		}
		warMemoryTransformationMonsterSkill.currentHitId = nHitId;
		Offense offense = warMemoryTransformationMonsterSkill.MakeOffense();
		List<PDHitTarget> targets = new List<PDHitTarget>();
		HashSet<long> monsterTargets = new HashSet<long>();
		PDHitTarget[] array = rawTargets;
		foreach (PDHitTarget rawTarget in array)
		{
			if (rawTarget != null && rawTarget.type == 2)
			{
				PDMonsterHitTarget monsterTarget = (PDMonsterHitTarget)rawTarget;
				if (monsterTargets.Add(monsterTarget.monsterInstanceId))
				{
					targets.Add(monsterTarget);
				}
			}
		}
		float fHitRange = monsterSkill.hitRange;
		foreach (PDHitTarget target in targets)
		{
			if (m_myHero.isDead)
			{
				break;
			}
			OffenseHit offenseHit = new OffenseHit(offense, nHitId);
			if (target.type == 2)
			{
				PDMonsterHitTarget monsterTarget2 = (PDMonsterHitTarget)target;
				MonsterInstance monsterInst = currentPlace.GetMonster(monsterTarget2.monsterInstanceId);
				if (monsterInst != null && BattleUtil.IsHit(m_myHero.position, fHitRange * 1.1f, monsterInst.position, monsterInst.monster.radius))
				{
					currentPlace.ProcessTamingMonsterSkillHit(offenseHit, monsterInst);
				}
			}
		}
	}
}
