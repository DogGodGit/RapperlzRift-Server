using System;
using ClientCommon;

namespace GameServer;

public class JobCommonSkillHitEventHandler : InGameEventHandler<CEBJobCommonSkillHitEventBody>
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
			int nHitId = m_body.hitId;
			long lnTargetMonsterInstanceId = m_body.targetMonsterInstanceId;
			if (placeInstanceId == Guid.Empty)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다.");
			}
			if (placeInstanceId != currentPlace.instanceId)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다. placeInstanceId = " + placeInstanceId);
			}
			if (!m_myHero.skillEnabled)
			{
				throw new EventHandleException("영웅이 스킬을 사용할 수 있는 상태가 아닙니다.");
			}
			HeroJobCommonSkill heroSkill = m_myHero.GetJobCommonSkill(nSkillId);
			if (heroSkill == null)
			{
				throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
			}
			if (heroSkill.currentHitId < 0)
			{
				throw new EventHandleException("해당 스킬 캐스팅을 시전하지 않았습니다.");
			}
			JobCommonSkill jobCommonSkill = heroSkill.skill;
			if (nHitId > jobCommonSkill.hitCount)
			{
				throw new EventHandleException("스킬적중ID가 유효하지 않습니다. nHitId = " + nHitId);
			}
			if (nHitId <= heroSkill.currentHitId)
			{
				throw new EventHandleException("적중ID가 유효하지 않습니다. nSkillId = " + nSkillId + ", nHitId = " + nHitId + ", heroSkillCurrentHitId = " + heroSkill.currentHitId);
			}
			heroSkill.currentHitId = nHitId;
			float fHitValidationRadius = jobCommonSkill.hitValidationRadius;
			MonsterInstance monsterInst = currentPlace.GetMonster(lnTargetMonsterInstanceId);
			if (monsterInst != null && BattleUtil.IsHit(m_myHero.position, fHitValidationRadius * 1.1f, monsterInst.position, monsterInst.monster.radius))
			{
				currentPlace.ProcessHeroJobCommonSkillHit_Monster(m_myHero, jobCommonSkill, monsterInst);
			}
		}
	}
}
