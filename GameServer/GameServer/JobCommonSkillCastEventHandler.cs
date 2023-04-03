using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class JobCommonSkillCastEventHandler : InGameEventHandler<CEBJobCommonSkillCastEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			if (m_body == null)
			{
				throw new EventHandleException("바디가 null입니다");
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
			if (!m_myHero.skillEnabled)
			{
				throw new EventHandleException("영웅이 스킬을 사용할 수 있는 상태가 아닙니다.");
			}
			if (m_myHero.isRiding)
			{
				throw new EventHandleException("영웅이 탈것에 탑승중입니다.");
			}
			if (m_myHero.isRidingCart)
			{
				throw new EventHandleException("영웅이 카트에 탑승중입니다.");
			}
			if (!m_myHero.heroSkillEnabled)
			{
				throw new EventHandleException("영웅이 영웅스킬을 사용할 수 있는 상태가 아닙니다.");
			}
			HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
			if (currentExclusiveAction != 0)
			{
				throw new EventHandleException("영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
			}
			HeroJobCommonSkill heroSkill = m_myHero.GetJobCommonSkill(nSkillId);
			if (heroSkill == null)
			{
				throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
			}
			if (!currentPlace.evasionCastEnabled && heroSkill.skill.skillId == 1)
			{
				throw new EventHandleException("회피스킬을 시전할 수 없는 장소입니다.");
			}
			if (!heroSkill.isOpened)
			{
				throw new EventHandleException("스킬을 사용하기 위한 메인퀘스트를 완료하지 않았습니다.");
			}
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			if (!heroSkill.IsExpiredSkillCoolTime(currentTime))
			{
				throw new EventHandleException("쿨타임이 만료되지 않았습니다. nSkillId = " + nSkillId);
			}
			if (!currentPlace.Contains(skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 유효하지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			heroSkill.castTime = currentTime;
			heroSkill.currentHitId = 0;
			m_myHero.StartBattleMode(currentTime);
			ServerEvent.SendHeroJobCommonSkillCast(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, nSkillId, skillTargetPosition);
			ProcessJobCommonSkillCastAbnormalState(heroSkill, currentTime);
		}
	}

	private void ProcessJobCommonSkillCastAbnormalState(HeroJobCommonSkill heroSkill, DateTimeOffset time)
	{
		AbnormalState abnormalState = heroSkill.skill.buffAbnormalState;
		if (abnormalState != null)
		{
			Hero target = heroSkill.hero;
			AbnormalStateLevel abnormalStateLevel = abnormalState.GetJobAbnormalState(target.jobId).GetAbnormalStateJobSkillLevel(1);
			if (abnormalStateLevel == null)
			{
				SFLogUtil.Error(GetType(), "상태이상레벨이 존재하지 않습니다. AbnoramalStateId = " + abnormalState.id + ", jobId = " + target.jobId);
			}
			else
			{
				target.ProcessAbnormalState(target, abnormalStateLevel, time);
			}
		}
	}
}
