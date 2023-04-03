using System;
using ClientCommon;

namespace GameServer;

public class TamingMonsterSkillCastEventHandler : InGameEventHandler<CEBTamingMonsterSkillCastEventBody>
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
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			if (!tamingMonsterSkill.IsExpiredSkillCoolTime(currentTime))
			{
				throw new EventHandleException("쿨타임이 만료되지 않았습니다. nSkillId = " + nSkillId);
			}
			MonsterSkill monsterSkill = tamingMonsterSkill.monsterSkill;
			float fCastRange = monsterSkill.castRange * 1.1f;
			if (!currentPlace.Contains(skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 유효하지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			if (fCastRange > 0f && monsterSkill.skillType == 2 && !MathUtil.SphereContains(m_myHero.position, fCastRange, skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 스킬 시전거리내에 있지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			tamingMonsterSkill.castTime = currentTime;
			tamingMonsterSkill.currentHitId = 0;
			m_myHero.StartBattleMode(currentTime);
			if (monsterSkill.skillType == 2)
			{
				SkillEffect effect = new SkillEffect();
				effect.Init(currentPlace, tamingMonsterSkill.MakeOffense(), skillTargetPosition, Guid.Empty, m_myHero.level);
				currentPlace.AddSkillEffect(effect);
			}
		}
	}
}
