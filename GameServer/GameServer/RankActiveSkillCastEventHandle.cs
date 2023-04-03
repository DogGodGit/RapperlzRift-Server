using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RankActiveSkillCastEventHandler : InGameEventHandler<CEBRankActiveSkillCastEventBody>
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
		Vector3 skillTargetPosition = m_body.skillTargetPosition;
		PDHitTarget target = m_body.target;
		if (placeInstanceId == Guid.Empty)
		{
			throw new EventHandleException("장소ID 가 유효하지 않습니다.");
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
		HeroRankActiveSkill heroSkill = m_myHero.GetRankActiveSkill(nSkillId);
		if (heroSkill == null)
		{
			throw new EventHandleException("스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		int nSelectedRankActiveSkillId = m_myHero.selectedRankActiveSkillId;
		if (nSkillId != nSelectedRankActiveSkillId)
		{
			throw new EventHandleException("선택된계급액티브스킬이 아닙니다. nSkillId = " + nSkillId + ", nSelectedRankActiveSkillId = " + nSelectedRankActiveSkillId);
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_myHero.GetRankActiveSkillRemainingCoolTime(currentTime) > 0f)
		{
			throw new EventHandleException("쿨타임이 만료되지 않았습니다.");
		}
		RankActiveSkill skill = heroSkill.skill;
		float fCastRange = 0f;
		if (skill.type == 1)
		{
			if (!currentPlace.Contains(skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 유효하지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
			fCastRange = skill.castRange * 1.1f;
			if (!MathUtil.SphereContains(m_myHero.position, fCastRange, skillTargetPosition))
			{
				throw new EventHandleException("스킬대상위치가 스킬 시전거리내에 있지 않습니다. nSkillId = " + nSkillId + ", SkillTargetPosition = " + skillTargetPosition);
			}
		}
		m_myHero.rankActiveSkillCastingTime = currentTime;
		SaveToDB(currentTime);
		m_myHero.StartBattleMode(currentTime);
		ServerEvent.SendHeroRankActiveSkillCast(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, nSkillId, skillTargetPosition);
		if (skill.type == 1)
		{
			if (target.type == 1)
			{
				PDHeroHitTarget heroTarget = (PDHeroHitTarget)target;
				Hero hero = currentPlace.GetHero((Guid)heroTarget.heroId);
				if (hero == null)
				{
					return;
				}
				lock (hero.syncObject)
				{
					if (!hero.isRidingCart && BattleUtil.IsHit(m_myHero.position, fCastRange, hero.position, hero.radius) && currentPlace.IsHeroRankActiveSkillCast_OtherHero(m_myHero, hero))
					{
						ProcessRankActiveSkillAbnormalState(hero, heroSkill, currentTime);
					}
					return;
				}
			}
			if (target.type == 2)
			{
				PDMonsterHitTarget monsterTarget = (PDMonsterHitTarget)target;
				MonsterInstance monsterInst = currentPlace.GetMonster(monsterTarget.monsterInstanceId);
				if (monsterInst != null && BattleUtil.IsHit(m_myHero.position, fCastRange, monsterInst.position, monsterInst.monster.radius))
				{
					ProcessRankActiveSkillAbnormalState(monsterInst, heroSkill, currentTime);
				}
			}
			else if (target.type == 3 && currentPlace is ContinentInstance continentInst)
			{
				PDCartHitTarget cartTarget = (PDCartHitTarget)target;
				CartInstance cartInst = continentInst.GetCartInstance(cartTarget.cartInstanceId);
				if (cartInst != null && BattleUtil.IsHit(m_myHero.position, fCastRange, cartInst.position, cartInst.radius))
				{
					ProcessRankActiveSkillAbnormalState(cartInst, heroSkill, currentTime);
				}
			}
		}
		else if (skill.type == 2)
		{
			ProcessRankActiveSkillAbnormalState(m_myHero, heroSkill, currentTime);
		}
	}

	private void ProcessRankActiveSkillAbnormalState(Unit target, HeroRankActiveSkill heroSkill, DateTimeOffset time)
	{
		AbnormalState abnormalState = heroSkill.skill.abnormalState;
		if (abnormalState == null)
		{
			SFLogUtil.Error(GetType(), "상태이상이 존재하지 않습니다. skillId = " + heroSkill.skillId);
			return;
		}
		AbnormalStateRankSkillLevel abnormalStateLevel = abnormalState.GetAbnormalStateRankSkillLevel(heroSkill.level);
		if (abnormalStateLevel == null)
		{
			SFLogUtil.Error(GetType(), "계급액티브스킬상태이상레벨이 존재하지 않습니다. AbnormalStateId = " + abnormalState.id + ", skillId = " + heroSkill.skillId + ", level = " + heroSkill.level);
		}
		else
		{
			target.ProcessAbnormalState(heroSkill.hero, abnormalStateLevel, time);
		}
	}

	private void SaveToDB(DateTimeOffset time)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RankActiveSkillCastingTime(m_myHero.id, time));
		dbWork.Schedule();
	}
}
