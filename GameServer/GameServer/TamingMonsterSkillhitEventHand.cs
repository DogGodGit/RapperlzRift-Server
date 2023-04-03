using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class TamingMonsterSkillhitEventHandler : InGameEventHandler<CEBTamingMonsterSkillHitEventBody>
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
		if (!m_myHero.isMonsterTaming)
		{
			throw new EventHandleException("영웅이 몬스터테이밍중이 아닙니다.");
		}
		if (!m_myHero.skillEnabled)
		{
			throw new EventHandleException("영웅이 스킬을 사용할 수 있는 상태가 아닙니다.");
		}
		HeroTamingMonsterSkill tamingMonsterSkill = m_myHero.GetTamingMonsterSkill(nSkillId);
		if (tamingMonsterSkill == null)
		{
			throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		if (tamingMonsterSkill.currentHitId < 0)
		{
			throw new EventHandleException("해당 스킬 캐스팅을 시전하지 않았습니다.");
		}
		MonsterSkill monsterSkill = tamingMonsterSkill.monsterSkill;
		SkillHit hit = monsterSkill.GetHit(nHitId);
		if (hit == null)
		{
			throw new EventHandleException("스킬적중ID가 유효하지 않습니다. nHitId = " + nHitId);
		}
		if (nHitId <= tamingMonsterSkill.currentHitId)
		{
			throw new EventHandleException("적중ID가 유효하지 않습니다. nSkillId = " + nSkillId + ", nHitId = " + nHitId + ", tamingMonsterSkillCurrentHitId = " + tamingMonsterSkill.currentHitId);
		}
		tamingMonsterSkill.currentHitId = nHitId;
		Offense offense = tamingMonsterSkill.MakeOffense();
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
