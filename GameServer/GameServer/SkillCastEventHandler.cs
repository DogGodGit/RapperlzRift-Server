using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SkillCastEventHandler : InGameEventHandler<CEBSkillCastEventBody>
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
			ServerEvent.SendSkillCastResult(m_myHero.account.peer, Guid.Empty, 0, 0, bIsSucceeded: false, 0);
			throw new EventHandleException("바디가 null입니다");
		}
		Guid placeInstanceId = (Guid)m_body.placeInstanceId;
		Guid heroId = (Guid)m_body.heroId;
		int nSkillId = m_body.skillId;
		int nChainSkillId = m_body.chainSkillId;
		Vector3 skillTargetPosition = m_body.skillTargetPosition;
		float fRotationY = m_body.heroTargetRotationY;
		Vector3 heroTargetPosition = m_body.heroTargetPosition;
		Guid targetHeroId = (Guid)m_body.targetHeroId;
		Hero attacker = null;
		try
		{
			if (placeInstanceId == Guid.Empty)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다.");
			}
			if (placeInstanceId != currentPlace.instanceId)
			{
				throw new EventHandleException("장소ID가 유효하지 않습니다. placeInstanceId = " + placeInstanceId);
			}
			if (heroId == Guid.Empty)
			{
				throw new EventHandleException("영웅ID가 유효하지 않습니다.");
			}
			attacker = ((heroId == m_myHero.id) ? m_myHero : currentPlace.GetHero(heroId));
			if (attacker == null)
			{
				throw new EventHandleException("공격자가 존재하지 않습니다. heroId = " + heroId);
			}
			if (attacker.controller.id != m_myHero.id)
			{
				throw new EventHandleException("공격자의 제어자가 아닙니다. heroId = " + heroId);
			}
			if (!attacker.skillEnabled)
			{
				throw new EventHandleException("공격자가 스킬을 사용할 수 있는 상태가 아닙니다. heroId = " + heroId);
			}
			if (attacker.isRiding)
			{
				throw new EventHandleException("영웅이 탈것에 탑승중입니다.");
			}
			if (attacker.isRidingCart)
			{
				throw new EventHandleException("영웅이 카트에 탑승중입니다.");
			}
			if (!m_myHero.heroSkillEnabled)
			{
				throw new EventHandleException("영웅이 영웅스킬을 사용할 수 있는 상태가 아닙니다.");
			}
			HeroExclusiveAction currentExclusiveAction = attacker.currentExclusiveAction;
			if (currentExclusiveAction != 0)
			{
				throw new EventHandleException("영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
			}
			HeroSkill heroSkill = attacker.GetSkill(nSkillId);
			if (heroSkill == null)
			{
				throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
			}
			if (!heroSkill.isOpened)
			{
				throw new EventHandleException("스킬을 사용하기 위한 메인퀘스트를 완료하지 않았습니다.");
			}
			JobSkill jobSkill = heroSkill.skill;
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			int nConsumeLak = 0;
			if (jobSkill.formType == 1)
			{
				JobChainSkill jobChainSkill = jobSkill.GetChainSkill(nChainSkillId);
				if (jobChainSkill == null)
				{
					throw new EventHandleException("직업연계스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId + ", nChainskillId = " + nChainSkillId);
				}
				if (nChainSkillId == 1)
				{
					if (jobSkill.isSpecialSkill)
					{
						nConsumeLak = Resource.instance.specialSkillMaxLak;
						if (attacker.lak < nConsumeLak)
						{
							throw new EventHandleException("라크가 부족합니다. attackerLak = " + attacker.lak + ", nConsumeLak = " + nConsumeLak);
						}
					}
					else
					{
						float fElapsedTime2 = DateTimeUtil.GetTimeSpanSeconds(heroSkill.castTime, currentTime);
						if (fElapsedTime2 < jobSkill.coolTime * 0.9f)
						{
							throw new EventHandleException("쿨타임이 만료되지 않았습니다. nSkill = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", fElapsedTime = " + fElapsedTime2);
						}
					}
				}
				else
				{
					if (attacker.lastCastSkillId != nSkillId)
					{
						throw new EventHandleException("해당 직업연계스킬을 사용할 수 없습니다. lastCastSkillId = " + attacker.lastCastSkillId + ", nSkillId = " + nSkillId);
					}
					if (nChainSkillId != heroSkill.currentChainSkillId + 1)
					{
						throw new EventHandleException("해당 직업연계스킬을 사용할 수 없습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId);
					}
				}
			}
			else
			{
				if (jobSkill.isSpecialSkill)
				{
					nConsumeLak = Resource.instance.specialSkillMaxLak;
					if (attacker.lak < nConsumeLak)
					{
						throw new EventHandleException("라크가 부족합니다. attackerLak = " + attacker.lak + ", nConsumeLak = " + nConsumeLak);
					}
				}
				else
				{
					float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(heroSkill.castTime, currentTime);
					if (fElapsedTime < jobSkill.coolTime * 0.9f)
					{
						throw new EventHandleException("쿨타임이 만료되지 않았습니다. nSkill = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", fElapsedTime = " + fElapsedTime);
					}
				}
				nChainSkillId = 0;
			}
			if (!currentPlace.Contains(skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 유효하지 않습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			float fCastRange = jobSkill.castRange * 1.1f;
			if (fCastRange > 0f && jobSkill.skillType == 2 && !MathUtil.SphereContains(attacker.position, fCastRange, skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 스킬 시전거리내에 있지 않습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", skillTargetPosition = " + skillTargetPosition);
			}
			if (!currentPlace.Contains(heroTargetPosition))
			{
				throw new EventHandleException("영웅대상위치가 유효하지 않습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", heroTargetPosition = " + heroTargetPosition);
			}
			if (jobSkill.castingMoveType == 1)
			{
				float fMoveRange = 0f;
				int nCastingMoveValue1 = jobSkill.castingMoveValue1;
				if (!MathUtil.SphereContains(fCircleRadius: (nCastingMoveValue1 <= 0) ? fCastRange : ((float)nCastingMoveValue1 / 100f * 1.1f), circleCenter: attacker.position, target: heroTargetPosition))
				{
					throw new EventHandleException("영웅대상위치가 시전 이동가능거리내에 있지 않습니다. nSkillId = " + nSkillId + ", nChainSkillId = " + nChainSkillId + ", heroTargetPosition = " + heroTargetPosition);
				}
			}
			if (jobSkill.formType != 1 || nChainSkillId == 1)
			{
				heroSkill.castTime = currentTime;
			}
			heroSkill.currentChainSkillId = nChainSkillId;
			heroSkill.currentHitId = 0;
			heroSkill.targetHeroId = targetHeroId;
			attacker.lastCastSkillId = nSkillId;
			if (jobSkill.isSpecialSkill)
			{
				attacker.UseLak(nConsumeLak);
			}
			if (jobSkill.castingMoveType == 1)
			{
				currentPlace.ChangeHeroPositionAndRotation(attacker, heroTargetPosition, fRotationY, bSendInterestTargetChangeEvent: true, currentTime);
			}
			attacker.StartBattleMode(currentTime);
			ServerEvent.SendSkillCastResult(m_myHero.account.peer, heroId, nSkillId, nChainSkillId, bIsSucceeded: true, attacker.lak);
			ServerEvent.SendHeroSkillCast(currentPlace.GetDynamicClientPeers(attacker.sector, attacker.id), attacker.id, nSkillId, nChainSkillId, heroTargetPosition, skillTargetPosition);
			ProcessSkillCastAbnormalState(heroSkill, currentTime);
			if (jobSkill.skillType == 2)
			{
				SkillEffect effect = new SkillEffect();
				effect.Init(currentPlace, heroSkill.MakeOffense(), skillTargetPosition, targetHeroId, m_myHero.level);
				currentPlace.AddSkillEffect(effect);
			}
		}
		catch (EventHandleException ex)
		{
			int nLak = attacker?.lak ?? 0;
			ServerEvent.SendSkillCastResult(m_myHero.account.peer, heroId, nSkillId, nChainSkillId, bIsSucceeded: false, nLak);
			throw new EventHandleException(ex.Message);
		}
	}

	private void ProcessSkillCastAbnormalState(HeroSkill heroSkill, DateTimeOffset time)
	{
		AbnormalState abnormalState = heroSkill.skill.buffAbnormalState;
		if (abnormalState != null)
		{
			Hero target = heroSkill.hero;
			AbnormalStateLevel abnormalStateLevel = abnormalState.GetJobAbnormalState(target.jobId).GetAbnormalStateJobSkillLevel(heroSkill.level);
			if (abnormalStateLevel == null)
			{
				SFLogUtil.Error(GetType(), "상태이상레벨이 존재하지 않습니다. AbnoramalStateId = " + abnormalState.id + ", jobId = " + target.jobId + ", level = " + heroSkill.level);
			}
			else
			{
				target.ProcessAbnormalState(target, abnormalStateLevel, time);
			}
		}
	}
}
