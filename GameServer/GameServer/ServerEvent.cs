using System;
using System.Collections.Generic;
using ClientCommon;
using Photon.SocketServer;
using ServerFramework;

namespace GameServer;

public static class ServerEvent
{
	public static void Send(ClientPeer peer, short snEventName, byte[] body)
	{
		peer.SendEvent(snEventName, body, default(SendParameters));
	}

	public static void Send(ClientPeer peer, ServerEventName eventName, SEBServerEventBody body)
	{
		Send(peer, (short)eventName, Body.SerializeRaw(body));
	}

	public static void Send(IEnumerable<ClientPeer> peers, short snEventName, byte[] body)
	{
		SFEvent.Send(peers, snEventName, body);
	}

	public static void Send(IEnumerable<ClientPeer> peers, ServerEventName eventName, SEBServerEventBody body)
	{
		Send(peers, (short)eventName, Body.SerializeRaw(body));
	}

	public static void SendDateChanged(IEnumerable<ClientPeer> peers, DateTimeOffset time)
	{
		SEBDateChangedEventBody body = new SEBDateChangedEventBody();
		body.time = (DateTimeOffset)time;
		Send(peers, ServerEventName.DateChanged, body);
	}

	public static void SendServerMaxLevelUpdated(IEnumerable<ClientPeer> peers, int nServerMaxLevel)
	{
		SEBServerMaxLevelUpdatedEventBody body = new SEBServerMaxLevelUpdatedEventBody();
		body.serverMaxLevel = nServerMaxLevel;
		Send(peers, ServerEventName.ServerMaxLevelUpdated, body);
	}

	public static void SendSecretLetterQuestTargetNationChanged(IEnumerable<ClientPeer> peers, int nTargetNationId)
	{
		SEBSecretLetterQuestTargetNationChangedEventBody body = new SEBSecretLetterQuestTargetNationChangedEventBody();
		body.targetNationId = nTargetNationId;
		Send(peers, ServerEventName.SecretLetterQuestTargetNationChanged, body);
	}

	public static void SendNationFundChanged(IEnumerable<ClientPeer> peers, int nNationId, long lnFund)
	{
		SEBNationFundChangedEventBody body = new SEBNationFundChangedEventBody();
		body.nationId = nNationId;
		body.fund = lnFund;
		Send(peers, ServerEventName.NationFundChanged, body);
	}

	public static void SendAccountLoginDuplicated(ClientPeer peer)
	{
		Send(peer, ServerEventName.AccountLoginDuplicated, null);
	}

	public static void SendHeroEnter(IEnumerable<ClientPeer> peers, PDHero hero, bool bIsRevivalEnter)
	{
		SEBHeroEnterEventBody body = new SEBHeroEnterEventBody();
		body.hero = hero;
		body.isRevivalEnter = bIsRevivalEnter;
		Send(peers, ServerEventName.HeroEnter, body);
	}

	public static void SendHeroExit(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroExitEventBody body = new SEBHeroExitEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroExit, body);
	}

	public static void SendHeroMove(IEnumerable<ClientPeer> peers, Guid heroId, PDVector3 position, float fRotationY)
	{
		SEBHeroMoveEventBody body = new SEBHeroMoveEventBody();
		body.heroId = (Guid)heroId;
		body.position = position;
		body.rotationY = fRotationY;
		Send(peers, ServerEventName.HeroMove, body);
	}

	public static void SendHeroMoveModeChanged(IEnumerable<ClientPeer> peers, Guid heroId, bool bIsWalking)
	{
		SEBHeroMoveModeChangedEventBody body = new SEBHeroMoveModeChangedEventBody();
		body.heroId = (Guid)heroId;
		body.isWalking = bIsWalking;
		Send(peers, ServerEventName.HeroMoveModeChanged, body);
	}

	public static void SendHeroInterestAreaEnter(IEnumerable<ClientPeer> peers, PDHero hero)
	{
		SEBHeroInterestAreaEnterEventBody body = new SEBHeroInterestAreaEnterEventBody();
		body.hero = hero;
		Send(peers, ServerEventName.HeroInterestAreaEnter, body);
	}

	public static void SendHeroInterestAreaExit(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroInterestAreaExitEventBody body = new SEBHeroInterestAreaExitEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroInterestAreaExit, body);
	}

	public static void SendHeroSkillCast(IEnumerable<ClientPeer> peers, Guid heroId, int nSkillId, int nChainSkillId, PDVector3 heroTargetPosition, PDVector3 skillTargetPosition)
	{
		SEBHeroSkillCastEventBody body = new SEBHeroSkillCastEventBody();
		body.heroId = (Guid)heroId;
		body.skillId = nSkillId;
		body.chainSkillId = nChainSkillId;
		body.heroTargetPosition = heroTargetPosition;
		body.skillTargetPosition = skillTargetPosition;
		Send(peers, ServerEventName.HeroSkillCast, body);
	}

	public static void SendHeroHit(IEnumerable<ClientPeer> peers, Guid heroId, PDHitResult hitResult)
	{
		SEBHeroHitEventBody body = new SEBHeroHitEventBody();
		body.heroId = (Guid)heroId;
		body.hitResult = hitResult;
		Send(peers, ServerEventName.HeroHit, body);
	}

	public static void SendHeroRevived(IEnumerable<ClientPeer> peers, Guid heroId, int nHP, PDVector3 position, float fRotationY)
	{
		SEBHeroRevivedEventBody body = new SEBHeroRevivedEventBody();
		body.heroId = (Guid)heroId;
		body.hp = nHP;
		body.position = position;
		body.rotationY = fRotationY;
		Send(peers, ServerEventName.HeroRevived, body);
	}

	public static void SendInterestTargetChange(ClientPeer peer, PDHero[] addedHeroes, PDMonsterInstance[] addedMonsterInsts, PDContinentObjectInstance[] addedContinentObjectInsts, PDCartInstance[] addedCartInsts, Guid[] removedHeroes, long[] removedMonsterInsts, long[] removedContinentObjectInsts, long[] removedCartInsts)
	{
		SEBInterestTargetChangeEventBody body = new SEBInterestTargetChangeEventBody();
		body.addedHeroes = addedHeroes;
		body.addedMonsterInsts = addedMonsterInsts;
		body.addedContinentObjectInsts = addedContinentObjectInsts;
		body.addedCartInsts = addedCartInsts;
		body.removedHeroes = (Guid[])(object)removedHeroes;
		body.removedMonsterInsts = removedMonsterInsts;
		body.removedContinentObjectInsts = removedContinentObjectInsts;
		body.removedCartInsts = removedCartInsts;
		Send(peer, ServerEventName.InterestTargetChange, body);
	}

	public static void SendSkillCastResult(ClientPeer peer, Guid heroId, int nSkillId, int nChainSkillId, bool bIsSucceeded, int nLak)
	{
		SEBSkillCastResultEventBody body = new SEBSkillCastResultEventBody();
		body.heroId = (Guid)heroId;
		body.skillId = nSkillId;
		body.chainSkillId = nChainSkillId;
		body.isSucceeded = bIsSucceeded;
		body.lak = nLak;
		Send(peer, ServerEventName.SkillCastResult, body);
	}

	public static void SendBattleModeStart(ClientPeer peer)
	{
		SEBBattleModeStartEventBody body = new SEBBattleModeStartEventBody();
		Send(peer, ServerEventName.BattleModeStart, body);
	}

	public static void SendBattleModeEnd(ClientPeer peer)
	{
		SEBBattleModeEndEventBody body = new SEBBattleModeEndEventBody();
		Send(peer, ServerEventName.BattleModeEnd, body);
	}

	public static void SendHeroBattleModeStart(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroBattleModeStartEventBody body = new SEBHeroBattleModeStartEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroBattleModeStart, body);
	}

	public static void SendHeroBattleModeEnd(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroBattleModeEndEventBody body = new SEBHeroBattleModeEndEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroBattleModeEnd, body);
	}

	public static void SendLakAcquisition(ClientPeer peer, int nLak)
	{
		SEBLakAcquisitionEventBody body = new SEBLakAcquisitionEventBody();
		body.lak = nLak;
		Send(peer, ServerEventName.LakAcquisition, body);
	}

	public static void SendHeroMaxHpChangedEvent(IEnumerable<ClientPeer> peers, Guid heroId, int nMaxHp, int nHp)
	{
		SEBHeroMaxHpChangedEventBody body = new SEBHeroMaxHpChangedEventBody();
		body.heroId = (Guid)heroId;
		body.maxHp = nMaxHp;
		body.hp = nHp;
		Send(peers, ServerEventName.HeroMaxHpChanged, body);
	}

	public static void SendMaxHpChanged(ClientPeer peer, int nMaxHp, int nHp)
	{
		SEBMaxHpChangedEventBody body = new SEBMaxHpChangedEventBody();
		body.maxHp = nMaxHp;
		body.hp = nHp;
		Send(peer, ServerEventName.MaxHpChanged, body);
	}

	public static void SendHeroAbnormalStateEffectStart(IEnumerable<ClientPeer> peers, Guid heroId, long lnAbnormalStateEffectInstanceId, int nAbnormalStateId, int nSourceJobId, int nLevel, float fRemainingTime, int nDamageAbsorbShieldRemainingAbsorbAmount, long[] removedAbnormalStateEffects)
	{
		SEBHeroAbnormalStateEffectStartEventBody body = new SEBHeroAbnormalStateEffectStartEventBody();
		body.heroId = (Guid)heroId;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		body.abnormalStateId = nAbnormalStateId;
		body.sourceJobId = nSourceJobId;
		body.level = nLevel;
		body.remainingTime = fRemainingTime;
		body.damageAbsorbShieldRemainingAbsorbAmount = nDamageAbsorbShieldRemainingAbsorbAmount;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroAbnormalStateEffectStart, body);
	}

	public static void SendHeroAbnormalStateEffectHit(IEnumerable<ClientPeer> peers, Guid heroId, int nHP, long lnAbnormalStateEffectInstanceId, int nDamage, int nHPDamage, long[] removedAbnormalStateEffects, PDAttacker pdAttacker)
	{
		SEBHeroAbnormalStateEffectHitEventBody body = new SEBHeroAbnormalStateEffectHitEventBody();
		body.heroId = (Guid)heroId;
		body.hp = nHP;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		body.attacker = pdAttacker;
		Send(peers, ServerEventName.HeroAbnormalStateEffectHit, body);
	}

	public static void SendHeroAbnormalStateEffectFinished(IEnumerable<ClientPeer> peers, Guid heroId, long lnAbnormalStateEffectInstanceId)
	{
		SEBHeroAbnormalStateEffectFinishedEventBody body = new SEBHeroAbnormalStateEffectFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		Send(peers, ServerEventName.HeroAbnormalStateEffectFinished, body);
	}

	public static void SendHeroContinentObjectInteractionStart(IEnumerable<ClientPeer> peers, Guid heroId, long lnContinentObjectInstanceId)
	{
		SEBHeroContinentObjectInteractionStartEventBody body = new SEBHeroContinentObjectInteractionStartEventBody();
		body.heroId = (Guid)heroId;
		body.continentObjectInstanceId = lnContinentObjectInstanceId;
		Send(peers, ServerEventName.HeroContinentObjectInteractionStart, body);
	}

	public static void SendHeroContinentObjectInteractionFinished(IEnumerable<ClientPeer> peers, Guid heroId, long lnContinentObjectInstanceId)
	{
		SEBHeroContinentObjectInteractionFinishedEventBody body = new SEBHeroContinentObjectInteractionFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.continentObjectInstanceId = lnContinentObjectInstanceId;
		Send(peers, ServerEventName.HeroContinentObjectInteractionFinished, body);
	}

	public static void SendHeroContinentObjectInteractionCancel(IEnumerable<ClientPeer> peers, Guid heroId, long lnContinentObjectInstanceId)
	{
		SEBHeroContinentObjectInteractionCancelEventBody body = new SEBHeroContinentObjectInteractionCancelEventBody();
		body.heroId = (Guid)heroId;
		body.continentObjectInstanceId = lnContinentObjectInstanceId;
		Send(peers, ServerEventName.HeroContinentObjectInteractionCancel, body);
	}

	public static void SendHeroHpRestored(IEnumerable<ClientPeer> peers, Guid heroId, int nHP)
	{
		SEBHeroHpRestoredEventBody body = new SEBHeroHpRestoredEventBody();
		body.heroId = (Guid)heroId;
		body.hp = nHP;
		Send(peers, ServerEventName.HeroHpRestored, body);
	}

	public static void SendHeroReturnScrollUseStart(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroReturnScrollUseStartEventBody body = new SEBHeroReturnScrollUseStartEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroReturnScrollUseStart, body);
	}

	public static void SendReturnScrollUseFinished(ClientPeer peer, PDInventorySlot pdInventorySlot, int nTargetContinentId, int nTargetNationId)
	{
		SEBReturnScrollUseFinishedEventBody body = new SEBReturnScrollUseFinishedEventBody();
		body.changedInventorySlot = pdInventorySlot;
		body.targetContinentId = nTargetContinentId;
		body.targetNationId = nTargetNationId;
		Send(peer, ServerEventName.ReturnScrollUseFinished, body);
	}

	public static void SendHeroReturnScrollUseFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroReturnScrollUseFinishedEventBody body = new SEBHeroReturnScrollUseFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroReturnScrollUseFinished, body);
	}

	public static void SendReturnScrollUseCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.ReturnScrollUseCancel, null);
	}

	public static void SendHeroReturnScrollUseCancel(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroReturnScrollUseCancelEventBody body = new SEBHeroReturnScrollUseCancelEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroReturnScrollUseCancel, body);
	}

	public static void SendDropObjectLooted(ClientPeer peer, PDFullHeroMainGear[] pdHeroMainGears, int nMaxAcquisitionMainGearGrade, PDInventorySlot[] pdChangedInventorySlots, PDDropObject[] pdLootedDropObjects, PDDropObject[] pdNotLootedDropObjects)
	{
		SEBDropObjectLootedEventBody body = new SEBDropObjectLootedEventBody();
		body.heroMainGears = pdHeroMainGears;
		body.maxAcquisitionMainGearGrade = nMaxAcquisitionMainGearGrade;
		body.changedInventorySlots = pdChangedInventorySlots;
		body.lootedDropObjects = pdLootedDropObjects;
		body.notLootedDropObjects = pdNotLootedDropObjects;
		Send(peer, ServerEventName.DropObjectLooted, body);
	}

	public static void SendMainQuestUpdated(ClientPeer peer, int nMainQuestNo, int nProgressCount)
	{
		SEBMainQuestUpdatedEventBody body = new SEBMainQuestUpdatedEventBody();
		body.mainQuestNo = nMainQuestNo;
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.MainQuestUpdated, body);
	}

	public static void SendMainQuestMonsterTransformationCanceled(ClientPeer peer, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBMainQuestMonsterTransformationCanceledEventBody body = new SEBMainQuestMonsterTransformationCanceledEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.MainQuestMonsterTransformationCanceled, body);
	}

	public static void SendMainQuestMonsterTransformationFinished(ClientPeer peer, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBMainQuestMonsterTransformationFinishedEventBody body = new SEBMainQuestMonsterTransformationFinishedEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.MainQuestMonsterTransformationFinished, body);
	}

	public static void SendHeroMainQuestMonsterTransformationStarted(IEnumerable<ClientPeer> peers, Guid heroId, int nTransformationMonsterId, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBHeroMainQuestMonsterTransformationStartedEventBody body = new SEBHeroMainQuestMonsterTransformationStartedEventBody();
		body.heroId = (Guid)heroId;
		body.transformationMonsterId = nTransformationMonsterId;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroMainQuestMonsterTransformationStarted, body);
	}

	public static void SendHeroMainQuestMonsterTransformationCanceled(IEnumerable<ClientPeer> peers, Guid heroId, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBHeroMainQuestMonsterTransformationCanceledEventBody body = new SEBHeroMainQuestMonsterTransformationCanceledEventBody();
		body.heroId = (Guid)heroId;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroMainQuestMonsterTransformationCanceled, body);
	}

	public static void SendHeroMainQuestMonsterTransformationFinished(IEnumerable<ClientPeer> peers, Guid heroId, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBHeroMainQuestMonsterTransformationFinishedEventBody body = new SEBHeroMainQuestMonsterTransformationFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroMainQuestMonsterTransformationFinished, body);
	}

	public static void SendHeroMainQuestTransformationMonsterSkillCast(IEnumerable<ClientPeer> peers, Guid heroId, int nSkillId, PDVector3 skillTargetPosition)
	{
		SEBHeroMainQuestTransformationMonsterSkillCastEventBody body = new SEBHeroMainQuestTransformationMonsterSkillCastEventBody();
		body.heroId = (Guid)heroId;
		body.skillId = nSkillId;
		body.skillTargetPosition = skillTargetPosition;
		Send(peers, ServerEventName.HeroMainQuestTransformationMonsterSkillCast, body);
	}

	public static void SendMonsterOwnershipChange(IEnumerable<ClientPeer> peers, long lnInstanceId, Guid ownerId, MonsterOwnerType ownerType)
	{
		SEBMonsterOwnershipChangeEventBody body = new SEBMonsterOwnershipChangeEventBody();
		body.instanceId = lnInstanceId;
		body.ownerId = (Guid)ownerId;
		body.ownerType = ownerType;
		Send(peers, ServerEventName.MonsterOwnershipChange, body);
	}

	public static void SendMonsterInterestAreaEnter(IEnumerable<ClientPeer> peers, PDMonsterInstance monsterInst)
	{
		SEBMonsterInterestAreaEnterEventBody body = new SEBMonsterInterestAreaEnterEventBody();
		body.monsterInst = monsterInst;
		Send(peers, ServerEventName.MonsterInterestAreaEnter, body);
	}

	public static void SendMonsterInterestAreaExit(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBMonsterInterestAreaExitEventBody body = new SEBMonsterInterestAreaExitEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.MonsterInterestAreaExit, body);
	}

	public static void SendMonsterMove(IEnumerable<ClientPeer> peers, long lnInstanceId, PDVector3 position, float fRotationY)
	{
		SEBMonsterMoveEventBody body = new SEBMonsterMoveEventBody();
		body.instanceId = lnInstanceId;
		body.position = position;
		body.rotationY = fRotationY;
		Send(peers, ServerEventName.MonsterMove, body);
	}

	public static void SendMonsterSkillCast(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, int nSkillId, PDVector3 skillTargetPosition)
	{
		SEBMonsterSkillCastEventBody body = new SEBMonsterSkillCastEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.skillId = nSkillId;
		body.skillTargetPosition = skillTargetPosition;
		Send(peers, ServerEventName.MonsterSkillCast, body);
	}

	public static void SendMonsterHit(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, PDHitResult hitResult)
	{
		SEBMonsterHitEventBody body = new SEBMonsterHitEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.hitResult = hitResult;
		Send(peers, ServerEventName.MonsterHit, body);
	}

	public static void SendMonsterMentalHit(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, Guid attackerId, Guid tamerId, int nSkillId, int nMentalStrength, int nDamage, int nMentalStrengthDamage, long[] removedAbnormalStateEffects)
	{
		SEBMonsterMentalHitEventBody body = new SEBMonsterMentalHitEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.attackerId = (Guid)attackerId;
		body.tamerId = (Guid)tamerId;
		body.skillId = nSkillId;
		body.mentalStrength = nMentalStrength;
		body.damage = nDamage;
		body.mentalStrengthDamage = nMentalStrengthDamage;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.MonsterMentalHit, body);
	}

	public static void SendMonsterSpawn(IEnumerable<ClientPeer> peers, PDMonsterInstance monsterInst)
	{
		SEBMonsterSpawnEventBody body = new SEBMonsterSpawnEventBody();
		body.monster = monsterInst;
		Send(peers, ServerEventName.MonsterSpawn, body);
	}

	public static void SendMonsterRemoved(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId)
	{
		SEBMonsterRemovedEventBody body = new SEBMonsterRemovedEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		Send(peers, ServerEventName.MonsterRemoved, body);
	}

	public static void SendMonsterAbnormalStateEffectStart(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, long lnAbnormalStateEffectInstanceId, int nAbnormalStateId, int nSourceJobId, int nLevel, float fRemainingTime, int nDamageAbsorbShieldRemainingAbsorbAmount, long[] removedAbnormalStateEffects)
	{
		SEBMonsterAbnormalStateEffectStartEventBody body = new SEBMonsterAbnormalStateEffectStartEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		body.abnormalStateId = nAbnormalStateId;
		body.sourceJobId = nSourceJobId;
		body.level = nLevel;
		body.remainingTime = fRemainingTime;
		body.damageAbsorbShieldRemainingAbsorbAmount = nDamageAbsorbShieldRemainingAbsorbAmount;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.MonsterAbnormalStateEffectStart, body);
	}

	public static void SendMonsterAbnormalStateEffectHit(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, int nHP, long lnAbnormalStateEffectInstanceId, int nDamage, int nHPDamage, long[] removedAbnormalStateEffects, PDAttacker pdAttacker)
	{
		SEBMonsterAbnormalStateEffectHitEventBody body = new SEBMonsterAbnormalStateEffectHitEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.hp = nHP;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		body.attacker = pdAttacker;
		Send(peers, ServerEventName.MonsterAbnormalStateEffectHit, body);
	}

	public static void SendMonsterAbnormalStateEffectFinished(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, long lnAbnormalStateEffectInstanceId)
	{
		SEBMonsterAbnormalStateEffectFinishedEventBody body = new SEBMonsterAbnormalStateEffectFinishedEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		Send(peers, ServerEventName.MonsterAbnormalStateEffectFinished, body);
	}

	public static void SendMonsterHpRestored(IEnumerable<ClientPeer> peers, long lnMonsterInstanceId, int nHP)
	{
		SEBMonsterHpRestoredEventBody body = new SEBMonsterHpRestoredEventBody();
		body.monsterInstanceId = lnMonsterInstanceId;
		body.hp = nHP;
		Send(peers, ServerEventName.MonsterHpRestored, body);
	}

	public static void SendMonsterReturnModeChanged(IEnumerable<ClientPeer> peers, long lnInstanceId, bool bIsReturnMode)
	{
		SEBMonsterReturnModeChangedEventBody body = new SEBMonsterReturnModeChangedEventBody();
		body.instanceId = lnInstanceId;
		body.isReturnMode = bIsReturnMode;
		Send(peers, ServerEventName.MonsterReturnModeChanged, body);
	}

	public static void SendHeroMainGearEquip(IEnumerable<ClientPeer> peers, Guid heroId, PDHeroMainGear heroMainGear)
	{
		SEBHeroMainGearEquipEventBody body = new SEBHeroMainGearEquipEventBody();
		body.heroId = (Guid)heroId;
		body.heroMainGear = heroMainGear;
		Send(peers, ServerEventName.HeroMainGearEquip, body);
	}

	public static void SendHeroMainGearUnequip(IEnumerable<ClientPeer> peers, Guid heroId, Guid heroMainGearId)
	{
		SEBHeroMainGearUnequipEventBody body = new SEBHeroMainGearUnequipEventBody();
		body.heroId = (Guid)heroId;
		body.heroMainGearId = (Guid)heroMainGearId;
		Send(peers, ServerEventName.HeroMainGearUnequip, body);
	}

	public static void SendNewMail(ClientPeer peer, PDMail mail)
	{
		SEBNewMailEventBody body = new SEBNewMailEventBody();
		body.mail = mail;
		Send(peer, ServerEventName.NewMail, body);
	}

	public static void SendHeroLevelUp(IEnumerable<ClientPeer> peers, Guid heroId, int level, int maxHp, int hp)
	{
		SEBHeroLevelUpEventBody body = new SEBHeroLevelUpEventBody();
		body.heroId = (Guid)heroId;
		body.level = level;
		body.maxHp = maxHp;
		body.hp = hp;
		Send(peers, ServerEventName.HeroLevelUp, body);
	}

	public static void SendExpAcquisition(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHp, int nHp)
	{
		SEBExpAcquisitionEventBody body = new SEBExpAcquisitionEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = nMaxHp;
		body.hp = nHp;
		Send(peer, ServerEventName.ExpAcquisition, body);
	}

	public static void SendContinentObjectCreated(IEnumerable<ClientPeer> peers, long lnContinentObjectInstanceId, int nArrangeNo)
	{
		SEBContinentObjectCreatedEventBody body = new SEBContinentObjectCreatedEventBody();
		body.continentObjectInstanceId = lnContinentObjectInstanceId;
		body.arrangeNo = nArrangeNo;
		Send(peers, ServerEventName.ContinentObjectCreated, body);
	}

	public static void SendContinentBanished(ClientPeer peer, int nTargetContinentId, int nTargetNationId)
	{
		SEBContinentBanishedEventBody body = new SEBContinentBanishedEventBody();
		body.targetContinentId = nTargetContinentId;
		body.targetNationId = nTargetNationId;
		Send(peer, ServerEventName.ContinentBanished, body);
	}

	public static void SendPartyInvitationArrived(ClientPeer peer, PDPartyInvitation invitation)
	{
		SEBPartyInvitationArrivedEventBody body = new SEBPartyInvitationArrivedEventBody();
		body.invitation = invitation;
		Send(peer, ServerEventName.PartyInvitationArrived, body);
	}

	public static void SendPartyInvitationCanceled(ClientPeer peer, long lnInvitationNo)
	{
		SEBPartyInvitationCanceledEventBody body = new SEBPartyInvitationCanceledEventBody();
		body.invitationNo = lnInvitationNo;
		Send(peer, ServerEventName.PartyInvitationCanceled, body);
	}

	public static void SendPartyInvitationAccepted(ClientPeer peer, long lnInvitationNo)
	{
		SEBPartyInvitationAcceptedEventBody body = new SEBPartyInvitationAcceptedEventBody();
		body.invitationNo = lnInvitationNo;
		Send(peer, ServerEventName.PartyInvitationAccepted, body);
	}

	public static void SendPartyInvitationRefused(ClientPeer peer, long lnInvitationNo)
	{
		SEBPartyInvitationRefusedEventBody body = new SEBPartyInvitationRefusedEventBody();
		body.invitationNo = lnInvitationNo;
		Send(peer, ServerEventName.PartyInvitationRefused, body);
	}

	public static void SendPartyInvitationLifetimeEnded(ClientPeer peer, long lnInvitationNo)
	{
		SEBPartyInvitationLifetimeEndedEventBody body = new SEBPartyInvitationLifetimeEndedEventBody();
		body.invitationNo = lnInvitationNo;
		Send(peer, ServerEventName.PartyInvitationLifetimeEnded, body);
	}

	public static void SendPartyApplicationArrived(ClientPeer peer, PDPartyApplication app)
	{
		SEBPartyApplicationArrivedEventBody body = new SEBPartyApplicationArrivedEventBody();
		body.app = app;
		Send(peer, ServerEventName.PartyApplicationArrived, body);
	}

	public static void SendPartyApplicationCanceled(ClientPeer peer, long lnApplicationNo)
	{
		SEBPartyApplicationCanceledEventBody body = new SEBPartyApplicationCanceledEventBody();
		body.applicationNo = lnApplicationNo;
		Send(peer, ServerEventName.PartyApplicationCanceled, body);
	}

	public static void SendPartyApplicationAccepted(ClientPeer peer, long lnApplicationNo, PDParty party)
	{
		SEBPartyApplicationAcceptedEventBody body = new SEBPartyApplicationAcceptedEventBody();
		body.applicationNo = lnApplicationNo;
		body.party = party;
		Send(peer, ServerEventName.PartyApplicationAccepted, body);
	}

	public static void SendPartyApplicationRefused(ClientPeer peer, long lnApplicationNo)
	{
		SEBPartyApplicationRefusedEventBody body = new SEBPartyApplicationRefusedEventBody();
		body.applicationNo = lnApplicationNo;
		Send(peer, ServerEventName.PartyApplicationRefused, body);
	}

	public static void SendPartyApplicationLifetimeEnded(ClientPeer peer, long lnApplicationNo)
	{
		SEBPartyApplicationLifetimeEndedEventBody body = new SEBPartyApplicationLifetimeEndedEventBody();
		body.applicationNo = lnApplicationNo;
		Send(peer, ServerEventName.PartyApplicationLifetimeEnded, body);
	}

	public static void SendPartyMemberEnter(IEnumerable<ClientPeer> peers, PDPartyMember member)
	{
		SEBPartyMemberEnterEventBody body = new SEBPartyMemberEnterEventBody();
		body.member = member;
		Send(peers, ServerEventName.PartyMemberEnter, body);
	}

	public static void SendPartyMemberExit(IEnumerable<ClientPeer> peers, Guid memberId, bool bBanished)
	{
		SEBPartyMemberExitEventBody body = new SEBPartyMemberExitEventBody();
		body.memberId = (Guid)memberId;
		body.banished = bBanished;
		Send(peers, ServerEventName.PartyMemberExit, body);
	}

	public static void SendPartyMasterChanged(IEnumerable<ClientPeer> peers, Guid masterId, float fCallRemainingCoolTime)
	{
		SEBPartyMasterChangedEventBody body = new SEBPartyMasterChangedEventBody();
		body.masterId = (Guid)masterId;
		body.callRemainingCoolTime = fCallRemainingCoolTime;
		Send(peers, ServerEventName.PartyMasterChanged, body);
	}

	public static void SendPartyBanished(ClientPeer peer)
	{
		Send(peer, ServerEventName.PartyBanished, null);
	}

	public static void SendPartyDisbanded(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.PartyDisbanded, null);
	}

	public static void SendPartyMembersUpdated(IEnumerable<ClientPeer> peers, PDPartyMember[] members)
	{
		SEBPartyMembersUpdatedEventBody body = new SEBPartyMembersUpdatedEventBody();
		body.members = members;
		Send(peers, ServerEventName.PartyMembersUpdated, body);
	}

	public static void SendPartyCall(IEnumerable<ClientPeer> peers, int nContinentId, int nNationId, PDVector3 position)
	{
		SEBPartyCallEventBody body = new SEBPartyCallEventBody();
		body.continentId = nContinentId;
		body.nationId = nNationId;
		body.position = position;
		Send(peers, ServerEventName.PartyCall, body);
	}

	public static void SendChattingMessageReceived(IEnumerable<ClientPeer> peers, int nType, string[] messages, PDChattingLink link, PDSimpleHero sender, PDSimpleHero target)
	{
		SEBChattingMessageReceivedEventBody body = new SEBChattingMessageReceivedEventBody();
		body.type = nType;
		body.messages = messages;
		body.link = link;
		body.sender = sender;
		body.target = target;
		Send(peers, ServerEventName.ChattingMessageReceived, body);
	}

	public static void SendMainQuestDungeonStepStart(ClientPeer peer, int nStepNo, PDMainQuestDungeonMonsterInstance[] pdMainQuestDungeonMonsterInsts, PDVector3 heroPosition, float fHeroRotationY)
	{
		SEBMainQuestDungeonStepStartEventBody body = new SEBMainQuestDungeonStepStartEventBody();
		body.stepNo = nStepNo;
		body.monsterInsts = pdMainQuestDungeonMonsterInsts;
		body.heroPosition = heroPosition;
		body.heroRotationY = fHeroRotationY;
		Send(peer, ServerEventName.MainQuestDungeonStepStart, body);
	}

	public static void SendMainQuestDungeonStepCompleted(ClientPeer peer, int nStepNo, int nLevel, long lnExp, int nMaxHp, int nHp, long lnGold, long lnMaxGold, long lnAcquiredExp)
	{
		SEBMainQuestDungeonStepCompletedEventBody body = new SEBMainQuestDungeonStepCompletedEventBody();
		body.stepNo = nStepNo;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = nMaxHp;
		body.hp = nHp;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		body.acquiredExp = lnAcquiredExp;
		Send(peer, ServerEventName.MainQuestDungeonStepCompleted, body);
	}

	public static void SendMainQuestDungeonClear(ClientPeer peer)
	{
		Send(peer, ServerEventName.MainQuestDungeonClear, null);
	}

	public static void SendMainQuestDungeonFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.MainQuestDungeonFail, null);
	}

	public static void SendMainQuestDungeonBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHp)
	{
		SEBMainQuestDungeonBanishedEventBody body = new SEBMainQuestDungeonBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHp;
		Send(peer, ServerEventName.MainQuestDungeonBanished, body);
	}

	public static void SendMainQuestDungeonMonsterSummon(ClientPeer peer, long lnSummonerInstanceId, PDMainQuestDungeonSummonMonsterInstance[] summonMonsterInsts)
	{
		SEBMainQuestDungeonMonsterSummonEventBody body = new SEBMainQuestDungeonMonsterSummonEventBody();
		body.summonerInstanceId = lnSummonerInstanceId;
		body.summonMonsterInsts = summonMonsterInsts;
		Send(peer, ServerEventName.MainQuestDungeonMonsterSummon, body);
	}

	public static void SendHeroVipLevelChanged(IEnumerable<ClientPeer> peers, Guid heroId, int nVipLevel)
	{
		SEBHeroVipLevelChangedEventBody body = new SEBHeroVipLevelChangedEventBody();
		body.heroId = (Guid)heroId;
		body.vipLevel = nVipLevel;
		Send(peers, ServerEventName.HeroVipLevelChanged, body);
	}

	public static void SendHeroEquippedWingChanged(IEnumerable<ClientPeer> peers, Guid heroId, int nWingId)
	{
		SEBHeroEquippedWingChangedEventBody body = new SEBHeroEquippedWingChangedEventBody();
		body.heroId = (Guid)heroId;
		body.wingId = nWingId;
		Send(peers, ServerEventName.HeroEquippedWingChanged, body);
	}

	public static void SendAccelerationStarted(ClientPeer peer)
	{
		Send(peer, ServerEventName.AccelerationStarted, null);
	}

	public static void SendHeroAccelerationStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroAccelerationStartedEventBody body = new SEBHeroAccelerationStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroAccelerationStarted, body);
	}

	public static void SendHeroAccelerationEnded(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroAccelerationEndedEventBody body = new SEBHeroAccelerationEndedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroAccelerationEnded, body);
	}

	public static void SendStoryDungeonStepStart(ClientPeer peer, int nStepNo, PDStoryDungeonMonsterInstance[] pdStoryDungeonMonsterInsts)
	{
		SEBStoryDungeonStepStartEventBody body = new SEBStoryDungeonStepStartEventBody();
		body.stepNo = nStepNo;
		body.monsterInsts = pdStoryDungeonMonsterInsts;
		Send(peer, ServerEventName.StoryDungeonStepStart, body);
	}

	public static void SendStoryDungeonClear(ClientPeer peer, PDItemBooty[] booties, PDInventorySlot[] pdChangedInventorySlots)
	{
		SEBStoryDungeonClearEventBody body = new SEBStoryDungeonClearEventBody();
		body.booties = booties;
		body.changedInventorySlots = pdChangedInventorySlots;
		Send(peer, ServerEventName.StoryDungeonClear, body);
	}

	public static void SendStoryDungeonFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.StoryDungeonFail, null);
	}

	public static void SendStoryDungeonBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBStoryDungeonBanishedEventBody body = new SEBStoryDungeonBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.StoryDungeonBanished, body);
	}

	public static void SendStoryDungeonTrapCast(ClientPeer peer, int nTrapId)
	{
		SEBStoryDungeonTrapCastEventBody body = new SEBStoryDungeonTrapCastEventBody();
		body.trapId = nTrapId;
		Send(peer, ServerEventName.StoryDungeonTrapCast, body);
	}

	public static void SendStoryDungeonTrapHit(ClientPeer peer, bool bIsImmortal, int nHP, int nDamage, int nHPDamage, PDAbnormalStateEffectDamageAbsorbShield[] changedAbnormalStateEffectDamageAbsorbShields, long[] removedAbnormalStateEffects)
	{
		SEBStoryDungeonTrapHitEventBody body = new SEBStoryDungeonTrapHitEventBody();
		body.isImmortal = bIsImmortal;
		body.hp = nHP;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.changedAbnormalStateEffectDamageAbsorbShields = changedAbnormalStateEffectDamageAbsorbShields;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.StoryDungeonTrapHit, body);
	}

	public static void SendStaminaAutoRecovery(ClientPeer peer, int nStamina)
	{
		SEBStaminaAutoRecoveryEventBody body = new SEBStaminaAutoRecoveryEventBody();
		body.stamina = nStamina;
		Send(peer, ServerEventName.StaminaAutoRecovery, body);
	}

	public static void SendStaminaScheduleRecovery(ClientPeer peer, int nStamina)
	{
		SEBStaminaScheduleRecoveryEventBody body = new SEBStaminaScheduleRecoveryEventBody();
		body.stamina = nStamina;
		Send(peer, ServerEventName.StaminaScheduleRecovery, body);
	}

	public static void SendHeroMountGetOn(IEnumerable<ClientPeer> peers, Guid heroId, int nMountId, int nLevel)
	{
		SEBHeroMountGetOnEventBody body = new SEBHeroMountGetOnEventBody();
		body.heroId = (Guid)heroId;
		body.mountId = nMountId;
		body.level = nLevel;
		Send(peers, ServerEventName.HeroMountGetOn, body);
	}

	public static void SendHeroMountGetOff(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroMountGetOffEventBody body = new SEBHeroMountGetOffEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroMountGetOff, body);
	}

	public static void SendHeroMountLevelUp(IEnumerable<ClientPeer> peers, Guid heroId, int nMountId, int nMountLevel)
	{
		SEBHeroMountLevelUpEventBody body = new SEBHeroMountLevelUpEventBody();
		body.heroId = (Guid)heroId;
		body.mountId = nMountId;
		body.mountLevel = nMountLevel;
		Send(peers, ServerEventName.HeroMountLevelUp, body);
	}

	public static void SendExpDungeonWaveStart(ClientPeer peer, int nWaveNo, PDExpDungeonMonsterInstance[] monsterInsts, PDExpDungeonLakChargeMonsterInstance lakChargeMonsterInst)
	{
		SEBExpDungeonWaveStartEventBody body = new SEBExpDungeonWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		body.lakChargeMonsterInst = lakChargeMonsterInst;
		Send(peer, ServerEventName.ExpDungeonWaveStart, body);
	}

	public static void SendExpDungeonWaveCompleted(ClientPeer peer)
	{
		Send(peer, ServerEventName.ExpDungeonWaveCompleted, null);
	}

	public static void SendExpDungeonWaveTimeout(ClientPeer peer)
	{
		Send(peer, ServerEventName.ExpDungeonWaveTimeout, null);
	}

	public static void SendExpDungeonClear(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP)
	{
		SEBExpDungeonClearEventBody body = new SEBExpDungeonClearEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.ExpDungeonClear, body);
	}

	public static void SendExpDungeonBanished(ClientPeer peer, int nPreviousContinentId, int nPrevuousNationId, int nHP)
	{
		SEBExpDungeonBanishedEventBody body = new SEBExpDungeonBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPrevuousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.ExpDungeonBanished, body);
	}

	public static void SendTreatOfFarmQuestMissionComplete(ClientPeer peer, int nCompletedMissionCount, PDHeroTreatOfFarmQuestMission nextMission, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, PDInventorySlot[] changedInventorySlots)
	{
		SEBTreatOfFarmQuestMissionCompleteEventBody body = new SEBTreatOfFarmQuestMissionCompleteEventBody();
		body.completedMissionCount = nCompletedMissionCount;
		body.nextMission = nextMission;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.TreatOfFarmQuestMissionComplete, body);
	}

	public static void SendTreatOfFarmQuestMissionFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.TreatOfFarmQuestMissionFail, null);
	}

	public static void SendTreatOfFarmQuestMissionMonsterSpawned(ClientPeer peer, long lnInstanceId, PDVector3 position, float fRemainingLifetime)
	{
		SEBTreatOfFarmQuestMissionMonsterSpawnedEventBody body = new SEBTreatOfFarmQuestMissionMonsterSpawnedEventBody();
		body.instanceId = lnInstanceId;
		body.position = position;
		body.remainingLifetime = fRemainingLifetime;
		Send(peer, ServerEventName.TreatOfFarmQuestMissionMonsterSpawned, body);
	}

	public static void SendCartEnter(IEnumerable<ClientPeer> peers, PDCartInstance cartInst)
	{
		SEBCartEnterEventBody body = new SEBCartEnterEventBody();
		body.cartInst = cartInst;
		Send(peers, ServerEventName.CartEnter, body);
	}

	public static void SendCartExit(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBCartExitEventBody body = new SEBCartExitEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.CartExit, body);
	}

	public static void SendCartGetOn(IEnumerable<ClientPeer> peers, PDCartInstance cartInst, Guid heroId)
	{
		SEBCartGetOnEventBody body = new SEBCartGetOnEventBody();
		body.cartInst = cartInst;
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.CartGetOn, body);
	}

	public static void SendCartGetOff(IEnumerable<ClientPeer> peers, long lnInstanceId, PDHero hero)
	{
		SEBCartGetOffEventBody body = new SEBCartGetOffEventBody();
		body.instanceId = lnInstanceId;
		body.hero = hero;
		Send(peers, ServerEventName.CartGetOff, body);
	}

	public static void SendCartInterestAreaEnter(IEnumerable<ClientPeer> peers, PDCartInstance cartInst)
	{
		SEBCartInterestAreaEnterEventBody body = new SEBCartInterestAreaEnterEventBody();
		body.cartInst = cartInst;
		Send(peers, ServerEventName.CartInterestAreaEnter, body);
	}

	public static void SendCartInterestAreaExit(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBCartInterestAreaExitEventBody body = new SEBCartInterestAreaExitEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.CartInterestAreaExit, body);
	}

	public static void SendCartMove(IEnumerable<ClientPeer> peers, long lnInstanceId, PDVector3 position, float fRotationY)
	{
		SEBCartMoveEventBody body = new SEBCartMoveEventBody();
		body.instanceId = lnInstanceId;
		body.position = position;
		body.rotationY = fRotationY;
		Send(peers, ServerEventName.CartMove, body);
	}

	public static void SendCartHighSpeedStart(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBCartHighSpeedStartEventBody body = new SEBCartHighSpeedStartEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.CartHighSpeedStart, body);
	}

	public static void SendCartHighSpeedEnd(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBCartHighSpeedEndEventBody body = new SEBCartHighSpeedEndEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.CartHighSpeedEnd, body);
	}

	public static void SendMyCartHighSpeedEnd(ClientPeer peer)
	{
		Send(peer, ServerEventName.MyCartHighSpeedEnd, null);
	}

	public static void SendCartChanged(IEnumerable<ClientPeer> peers, long lnInstanceId, int nCartId)
	{
		SEBCartChangedEventBody body = new SEBCartChangedEventBody();
		body.instanceId = lnInstanceId;
		body.cartId = nCartId;
		Send(peers, ServerEventName.CartChanged, body);
	}

	public static void SendCartHit(IEnumerable<ClientPeer> peers, long lnInstanceId, PDHitResult hitResult)
	{
		SEBCartHitEventBody body = new SEBCartHitEventBody();
		body.cartInstanceId = lnInstanceId;
		body.hitResult = hitResult;
		Send(peers, ServerEventName.CartHit, body);
	}

	public static void SendCartAbnormalStateEffectStart(IEnumerable<ClientPeer> peers, long lnCartInstanceId, long lnAbnormalStateEffectInstanceId, int nAbnormalStateId, int nSourceJobId, int nLevel, float fRemainingTime, int nDamageAbsorbShieldRemainingAbsorbAmount, long[] removedAbnormalStateEffects)
	{
		SEBCartAbnormalStateEffectStartEventBody body = new SEBCartAbnormalStateEffectStartEventBody();
		body.cartInstanceId = lnCartInstanceId;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		body.abnormalStateId = nAbnormalStateId;
		body.sourceJobId = nSourceJobId;
		body.level = nLevel;
		body.remainingTime = fRemainingTime;
		body.damageAbsorbShieldRemainingAbsorbAmount = nDamageAbsorbShieldRemainingAbsorbAmount;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.CartAbnormalStateEffectStart, body);
	}

	public static void SendCartAbnormalStateEffectHit(IEnumerable<ClientPeer> peers, long lnCartInstanceId, int nHP, long lnAbnormalStateEffectInstanceId, int nDamage, int nHPDamage, long[] removedAbnormalStateEffects, PDAttacker pdAttacker)
	{
		SEBCartAbnormalStateEffectHitEventBody body = new SEBCartAbnormalStateEffectHitEventBody();
		body.cartInstanceId = lnCartInstanceId;
		body.hp = nHP;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		body.attacker = pdAttacker;
		Send(peers, ServerEventName.CartAbnormalStateEffectHit, body);
	}

	public static void SendCartAbnormalStateEffectFinished(IEnumerable<ClientPeer> peers, long lnCartInstanceId, long lnAbnormalStateEffectInstanceId)
	{
		SEBCartAbnormalStateEffectFinishedEventBody body = new SEBCartAbnormalStateEffectFinishedEventBody();
		body.cartInstanceId = lnCartInstanceId;
		body.abnormalStateEffectInstanceId = lnAbnormalStateEffectInstanceId;
		Send(peers, ServerEventName.CartAbnormalStateEffectFinished, body);
	}

	public static void SendGoldDungeonStepStart(ClientPeer peer, int nStepNo, PDGoldDungeonMonsterInstance[] monsterInsts)
	{
		SEBGoldDungeonStepStartEventBody body = new SEBGoldDungeonStepStartEventBody();
		body.stepNo = nStepNo;
		body.monsterInsts = monsterInsts;
		Send(peer, ServerEventName.GoldDungeonStepStart, body);
	}

	public static void SendGoldDungeonStepCompleted(ClientPeer peer, long lnRewardGold, long lnGold, long lnMaxGold)
	{
		SEBGoldDungeonStepCompletedEventBody body = new SEBGoldDungeonStepCompletedEventBody();
		body.rewardGold = lnRewardGold;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		Send(peer, ServerEventName.GoldDungeonStepCompleted, body);
	}

	public static void SendGoldDungeonWaveStart(ClientPeer peer, int nWaveNo)
	{
		SEBGoldDungeonWaveStartEventBody body = new SEBGoldDungeonWaveStartEventBody();
		body.waveNo = nWaveNo;
		Send(peer, ServerEventName.GoldDungeonWaveStart, body);
	}

	public static void SendGoldDungeonWaveCompleted(ClientPeer peer)
	{
		Send(peer, ServerEventName.GoldDungeonWaveCompleted, null);
	}

	public static void SendGoldDungeonWaveTimeout(ClientPeer peer)
	{
		Send(peer, ServerEventName.GoldDungeonWaveTimeout, null);
	}

	public static void SendGoldDungeonClear(ClientPeer peer, long lnRewardGold, long lnGold, long lnMaxGold)
	{
		SEBGoldDungeonClearEventBody body = new SEBGoldDungeonClearEventBody();
		body.rewardGold = lnRewardGold;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		Send(peer, ServerEventName.GoldDungeonClear, body);
	}

	public static void SendGoldDungeonFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.GoldDungeonFail, null);
	}

	public static void SendGoldDungeonBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBGoldDungeonBanishedEventBody body = new SEBGoldDungeonBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.GoldDungeonBanished, body);
	}

	public static void SendUndergroundMazeBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBUndergroundMazeBanishedEventBody body = new SEBUndergroundMazeBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.UndergroundMazeBanished, body);
	}

	public static void SendArtifactRoomStart(ClientPeer peer, PDArtifactRoomMonsterInstance[] monsterInsts)
	{
		SEBArtifactRoomStartEventBody body = new SEBArtifactRoomStartEventBody();
		body.monsterInsts = monsterInsts;
		Send(peer, ServerEventName.ArtifactRoomStart, body);
	}

	public static void SendArtifactRoomClear(ClientPeer peer, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBArtifactRoomClearEventBody body = new SEBArtifactRoomClearEventBody();
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.ArtifactRoomClear, body);
	}

	public static void SendArtifactRoomBanishedForNextFloorChallenge(ClientPeer peer)
	{
		Send(peer, ServerEventName.ArtifactRoomBanishedForNextFloorChallenge, null);
	}

	public static void SendArtifactRoomFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.ArtifactRoomFail, null);
	}

	public static void SendArtifactRoomBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBArtifactRoomBanishedEventBody body = new SEBArtifactRoomBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.ArtifactRoomBanished, body);
	}

	public static void SendArtifactRoomSweepNextFloorStart(ClientPeer peer, int nProgressFloor)
	{
		SEBArtifactRoomSweepNextFloorStartEventBody body = new SEBArtifactRoomSweepNextFloorStartEventBody();
		body.progressFloor = nProgressFloor;
		Send(peer, ServerEventName.ArtifactRoomSweepNextFloorStart, body);
	}

	public static void SendArtifactRoomSweepCompleted(ClientPeer peer)
	{
		Send(peer, ServerEventName.ArtifactRoomSweepCompleted, null);
	}

	public static void SendBountyHunterQuestUpdated(ClientPeer peer, int nPorgressCount)
	{
		SEBBountyHunterQuestUpdatedEventBody body = new SEBBountyHunterQuestUpdatedEventBody();
		body.progressCount = nPorgressCount;
		Send(peer, ServerEventName.BountyHunterQuestUpdated, body);
	}

	public static void SendFishingCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.FishingCanceled, null);
	}

	public static void SendFishingCastingCompleted(ClientPeer peer, int nCastingCount, long lnAcquiredExp, int nLevel, long lnExp, int maxHp, int hp)
	{
		SEBFishingCastingCompletedEventBody body = new SEBFishingCastingCompletedEventBody();
		body.castingCount = nCastingCount;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = maxHp;
		body.hp = hp;
		Send(peer, ServerEventName.FishingCastingCompleted, body);
	}

	public static void SendHeroFishingStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroFishingStartedEventBody body = new SEBHeroFishingStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroFishingStarted, body);
	}

	public static void SendHeroFishingCompleted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroFishingCompletedEventBody body = new SEBHeroFishingCompletedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroFishingCompleted, body);
	}

	public static void SendHeroFishingCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroFishingCanceledEventBody body = new SEBHeroFishingCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroFishingCanceled, body);
	}

	public static void SendMysteryBoxPickCompleted(ClientPeer peer, int nPickCount, int nPickedBoxGrade)
	{
		SEBMysteryBoxPickCompletedEventBody body = new SEBMysteryBoxPickCompletedEventBody();
		body.pickCount = nPickCount;
		body.pickedBoxGrade = nPickedBoxGrade;
		Send(peer, ServerEventName.MysteryBoxPickCompleted, body);
	}

	public static void SendMysteryBoxPickCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.MysteryBoxPickCanceled, null);
	}

	public static void SendHeroMysteryBoxPickStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroMysteryBoxPickStartedEventBody body = new SEBHeroMysteryBoxPickStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroMysteryBoxPickStarted, body);
	}

	public static void SendHeroMysteryBoxPickCompleted(IEnumerable<ClientPeer> peers, Guid heroId, int nPickedBoxGrade)
	{
		SEBHeroMysteryBoxPickCompletedEventBody body = new SEBHeroMysteryBoxPickCompletedEventBody();
		body.heroId = (Guid)heroId;
		body.pickedBoxGrade = nPickedBoxGrade;
		Send(peers, ServerEventName.HeroMysteryBoxPickCompleted, body);
	}

	public static void SendHeroMysteryBoxPickCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroMysteryBoxPickCanceledEventBody body = new SEBHeroMysteryBoxPickCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroMysteryBoxPickCanceled, body);
	}

	public static void SendHeroMysteryBoxQuestCompleted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroMysteryBoxQuestCompletedEventBody body = new SEBHeroMysteryBoxQuestCompletedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroMysteryBoxQuestCompleted, body);
	}

	public static void SendSecretLetterPickCompleted(ClientPeer peer, int nPickCount, int nPickedLetterGrade)
	{
		SEBSecretLetterPickCompletedEventBody body = new SEBSecretLetterPickCompletedEventBody();
		body.pickCount = nPickCount;
		body.pickedLetterGrade = nPickedLetterGrade;
		Send(peer, ServerEventName.SecretLetterPickCompleted, body);
	}

	public static void SendSecretLetterPickCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.SecretLetterPickCanceled, null);
	}

	public static void SendHeroSecretLetterPickStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroSecretLetterPickStartedEventBody body = new SEBHeroSecretLetterPickStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroSecretLetterPickStarted, body);
	}

	public static void SendHeroSecretLetterPickCompleted(IEnumerable<ClientPeer> peers, Guid heroId, int nPickedLetterGrade)
	{
		SEBHeroSecretLetterPickCompletedEventBody body = new SEBHeroSecretLetterPickCompletedEventBody();
		body.heroId = (Guid)heroId;
		body.pickedLetterGrade = nPickedLetterGrade;
		Send(peers, ServerEventName.HeroSecretLetterPickCompleted, body);
	}

	public static void SendHeroSecretLetterPickCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroSecretLetterPickCanceledEventBody body = new SEBHeroSecretLetterPickCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroSecretLetterPickCanceled, body);
	}

	public static void SendHeroSecretLetterQuestCompleted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroSecretLetterQuestCompletedEventBody body = new SEBHeroSecretLetterQuestCompletedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroSecretLetterQuestCompleted, body);
	}

	public static void SendDimensionRaidInteractionCompleted(ClientPeer peer, int nNextStep)
	{
		SEBDimensionRaidInteractionCompletedEventBody body = new SEBDimensionRaidInteractionCompletedEventBody();
		body.nextStep = nNextStep;
		Send(peer, ServerEventName.DimensionRaidInteractionCompleted, body);
	}

	public static void SendDimensionRaidInteractionCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.DimensionRaidInteractionCanceled, null);
	}

	public static void SendHeroDimensionRaidInteractionStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDimensionRaidInteractionStartedEventBody body = new SEBHeroDimensionRaidInteractionStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDimensionRaidInteractionStarted, body);
	}

	public static void SendHeroDimensionRaidInteractionCompleted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDimensionRaidInteractionCompletedEventBody body = new SEBHeroDimensionRaidInteractionCompletedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDimensionRaidInteractionCompleted, body);
	}

	public static void SendHeroDimensionRaidInteractionCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDimensionRaidInteractionCanceledEventBody body = new SEBHeroDimensionRaidInteractionCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDimensionRaidInteractionCanceled, body);
	}

	public static void SendHolyWarQuestUpdated(ClientPeer peer, int nKillCount)
	{
		SEBHolyWarQuestUpdatedEventBody body = new SEBHolyWarQuestUpdatedEventBody();
		body.killCount = nKillCount;
		Send(peer, ServerEventName.HolyWarQuestUpdated, body);
	}

	public static void SendSeriesMissionUpdated(ClientPeer peer, int nMissionId, int nProgressCount)
	{
		SEBSeriesMissionUpdatedEventBody body = new SEBSeriesMissionUpdatedEventBody();
		body.missionId = nMissionId;
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.SeriesMissionUpdated, body);
	}

	public static void SendTodayMissionUpdated(ClientPeer peer, int nMissionId, int nProgressCount)
	{
		SEBTodayMissionUpdatedEventBody body = new SEBTodayMissionUpdatedEventBody();
		body.missionId = nMissionId;
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.TodayMissionUpdated, body);
	}

	public static void SendTodayMissionListChanged(ClientPeer peer, DateTime date, PDHeroTodayMission[] missions)
	{
		SEBTodayMissionListChangedEventBody body = new SEBTodayMissionListChangedEventBody();
		body.date = (DateTime)date;
		body.missions = missions;
		Send(peer, ServerEventName.TodayMissionListChanged, body);
	}

	public static void SendTodayTaskUpdated(ClientPeer peer, DateTime date, int nTaskId, int nProgressCount, int nAchievementDailyPoint)
	{
		SEBTodayTaskUpdatedEventBody body = new SEBTodayTaskUpdatedEventBody();
		body.date = (DateTime)date;
		body.taskId = nTaskId;
		body.progressCount = nProgressCount;
		body.achievementDailyPoint = nAchievementDailyPoint;
		Send(peer, ServerEventName.TodayTaskUpdated, body);
	}

	public static void SendContinentExitForAncientRelicEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForAncientRelicEnter, null);
	}

	public static void SendAncientRelicMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBAncientRelicMatchingRoomPartyEnterEventBody body = new SEBAncientRelicMatchingRoomPartyEnterEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.AncientRelicMatchingRoomPartyEnter, body);
	}

	public static void SendAncientRelicMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBAncientRelicMatchingRoomBanishedEventBody body = new SEBAncientRelicMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.AncientRelicMatchingRoomBanished, body);
	}

	public static void SendAncientRelicMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBAncientRelicMatchingStatusChangedEventBody body = new SEBAncientRelicMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.AncientRelicMatchingStatusChanged, body);
	}

	public static void SendAncientRelicStepStart(IEnumerable<ClientPeer> peers, int nStepNo, PDVector3 targetPosition, float fTargetRadius, int nRemoveObstacleId)
	{
		SEBAncientRelicStepStartEventBody body = new SEBAncientRelicStepStartEventBody();
		body.stepNo = nStepNo;
		body.targetPosition = targetPosition;
		body.targetRadius = fTargetRadius;
		body.removeObstacleId = nRemoveObstacleId;
		Send(peers, ServerEventName.AncientRelicStepStart, body);
	}

	public static void SendAncientRelicStepCompleted(ClientPeer peer, PDItemBooty[] booties, PDInventorySlot[] changedInventorySlots)
	{
		SEBAncientRelicStepCompletedEventBody body = new SEBAncientRelicStepCompletedEventBody();
		body.booties = booties;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.AncientRelicStepCompleted, body);
	}

	public static void SendAncientRelicWaveStart(IEnumerable<ClientPeer> peers, int nWaveNo, PDAncientRelicMonsterInstance[] monsterInsts)
	{
		SEBAncientRelicWaveStartEventBody body = new SEBAncientRelicWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.AncientRelicWaveStart, body);
	}

	public static void SendAncientRelicPointUpdated(IEnumerable<ClientPeer> peers, int nPoint)
	{
		SEBAncientRelicPointUpdatedEventBody body = new SEBAncientRelicPointUpdatedEventBody();
		body.point = nPoint;
		Send(peers, ServerEventName.AncientRelicPointUpdated, body);
	}

	public static void SendAncientRelicClear(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.AncientRelicClear, null);
	}

	public static void SendAncientRelicFail(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.AncientRelicFail, null);
	}

	public static void SendAncientRelicBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBAncientRelicBanishedEventBody body = new SEBAncientRelicBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.AncientRelicBanished, body);
	}

	public static void SendAncientRelicTrapActivated(IEnumerable<ClientPeer> peers, int nId)
	{
		SEBAncientRelicTrapActivatedEventBody body = new SEBAncientRelicTrapActivatedEventBody();
		body.id = nId;
		Send(peers, ServerEventName.AncientRelicTrapActivated, body);
	}

	public static void SendAncientRelicTrapHit(IEnumerable<ClientPeer> peers, Guid heroId, bool bIsImmortal, int nHP, int nDamage, int nHPDamage, PDAbnormalStateEffectDamageAbsorbShield[] changedAbnormalStateEffectDamageAbsorbShields, long[] removedAbnormalStateEffects)
	{
		SEBAncientRelicTrapHitEventBody body = new SEBAncientRelicTrapHitEventBody();
		body.heroId = (Guid)heroId;
		body.isImmortal = bIsImmortal;
		body.hp = nHP;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.changedAbnormalStateEffectDamageAbsorbShields = changedAbnormalStateEffectDamageAbsorbShields;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.AncientRelicTrapHit, body);
	}

	public static void SendAncientRelicTrapEffectFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBAncientRelicTrapEffectFinishedEventBody body = new SEBAncientRelicTrapEffectFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.AncientRelicTrapEffectFinished, body);
	}

	public static void SendPvpKill(ClientPeer peer, DateTime date, int nKillCount, int nExploitPoint, int nDailyExploitPoint)
	{
		SEBPvpKillEventBody body = new SEBPvpKillEventBody();
		body.date = (DateTime)date;
		body.killCount = nKillCount;
		body.exploitPoint = nExploitPoint;
		body.dailyExploitPoint = nDailyExploitPoint;
		Send(peer, ServerEventName.PvpKill, body);
	}

	public static void SendPvpAssist(ClientPeer peer, DateTime date, int nAssistCount, int nExploitPoint, int nDailyExploitPoint)
	{
		SEBPvpAssistEventBody body = new SEBPvpAssistEventBody();
		body.date = (DateTime)date;
		body.assistCount = nAssistCount;
		body.exploitPoint = nExploitPoint;
		body.dailyExploitPoint = nDailyExploitPoint;
		Send(peer, ServerEventName.PvpAssist, body);
	}

	public static void SendHeroRankAcquired(IEnumerable<ClientPeer> peers, Guid heroId, int nRankNo)
	{
		SEBHeroRankAcquiredEventBody body = new SEBHeroRankAcquiredEventBody();
		body.heroId = (Guid)heroId;
		body.rankNo = nRankNo;
		Send(peers, ServerEventName.HeroRankAcquired, body);
	}

	public static void SendHeroRankActiveSkillCast(IEnumerable<ClientPeer> peers, Guid heroId, int nSkillId, PDVector3 skillTargetPosition)
	{
		SEBHeroRankActiveSkillCastEventBody body = new SEBHeroRankActiveSkillCastEventBody();
		body.heroId = (Guid)heroId;
		body.skillId = nSkillId;
		body.skillTargetPosition = skillTargetPosition;
		Send(peers, ServerEventName.HeroRankActiveSkillCast, body);
	}

	public static void SendHeroDistortionStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDistortionStartedEventBody body = new SEBHeroDistortionStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDistortionStarted, body);
	}

	public static void SendHeroDistortionFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDistortionFinishedEventBody body = new SEBHeroDistortionFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDistortionFinished, body);
	}

	public static void SendHeroDistortionCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDistortionCanceledEventBody body = new SEBHeroDistortionCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDistortionCanceled, body);
	}

	public static void SendDistortionCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.DistortionCanceled, null);
	}

	public static void SendFieldOfHonorStart(ClientPeer peer)
	{
		Send(peer, ServerEventName.FieldOfHonorStart, null);
	}

	public static void SendFieldOfHonorClear(ClientPeer peer, int nMyRanking, int nSuccessiveCount, int nHonorPoint, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP)
	{
		SEBFieldOfHonorClearEventBody body = new SEBFieldOfHonorClearEventBody();
		body.myRanking = nMyRanking;
		body.successiveCount = nSuccessiveCount;
		body.honorPoint = nHonorPoint;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.FieldOfHonorClear, body);
	}

	public static void SendFieldOfHonorFail(ClientPeer peer, int nSuccessiveCount, int nHonorPoint, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP)
	{
		SEBFieldOfHonorFailEventBody body = new SEBFieldOfHonorFailEventBody();
		body.successiveCount = nSuccessiveCount;
		body.honorPoint = nHonorPoint;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.FieldOfHonorFail, body);
	}

	public static void SendFieldOfHonorBanished(ClientPeer peer, int nPreviousContinentId, int nPrevuousNationId, int nHP)
	{
		SEBFieldOfHonorBanishedEventBody body = new SEBFieldOfHonorBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPrevuousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.FieldOfHonorBanished, body);
	}

	public static void SendFieldOfHonorDailyRankingUpdated(ClientPeer peer, int nRankingNo, int nRanking)
	{
		SEBFieldOfHonorDailyRankingUpdatedEventBody body = new SEBFieldOfHonorDailyRankingUpdatedEventBody();
		body.rankingNo = nRankingNo;
		body.ranking = nRanking;
		Send(peer, ServerEventName.FieldOfHonorDailyRankingUpdated, body);
	}

	public static void SendDailyServerLevelRankingUpdated(ClientPeer peer, int nRankingNo, int nRanking)
	{
		SEBDailyServerLevelRankingUpdatedEventBody body = new SEBDailyServerLevelRankingUpdatedEventBody();
		body.rankingNo = nRankingNo;
		body.ranking = nRanking;
		Send(peer, ServerEventName.DailyServerLevelRankingUpdated, body);
	}

	public static void SendGuildApplicationAccepted(ClientPeer peer, Guid applicationId, PDGuild pdGuild, int nMemberGrade, int nMaxHp)
	{
		SEBGuildApplicationAcceptedEventBody body = new SEBGuildApplicationAcceptedEventBody();
		body.applicationId = (Guid)applicationId;
		body.guild = pdGuild;
		body.memberGrade = nMemberGrade;
		body.maxHp = nMaxHp;
		Send(peer, ServerEventName.GuildApplicationAccepted, body);
	}

	public static void SendGuildApplicationRefused(ClientPeer peer, Guid applicationId)
	{
		SEBGuildApplicationRefusedEventBody body = new SEBGuildApplicationRefusedEventBody();
		body.applicationId = (Guid)applicationId;
		Send(peer, ServerEventName.GuildApplicationRefused, body);
	}

	public static void SendGuildApplicationCountUpdated(IEnumerable<ClientPeer> peers, int nCount)
	{
		SEBGuildApplicationCountUpdatedEventBody body = new SEBGuildApplicationCountUpdatedEventBody();
		body.count = nCount;
		Send(peers, ServerEventName.GuildApplicationCountUpdated, body);
	}

	public static void SendGuildMemberEnter(IEnumerable<ClientPeer> peers, Guid heroId, string sName)
	{
		SEBGuildMemberEnterEventBody body = new SEBGuildMemberEnterEventBody();
		body.heroId = (Guid)heroId;
		body.name = sName;
		Send(peers, ServerEventName.GuildMemberEnter, body);
	}

	public static void SendGuildMemberExit(IEnumerable<ClientPeer> peers, Guid heroId, string sName, bool bIsBanished)
	{
		SEBGuildMemberExitEventBody body = new SEBGuildMemberExitEventBody();
		body.heroId = (Guid)heroId;
		body.name = sName;
		body.isBanished = bIsBanished;
		Send(peers, ServerEventName.GuildMemberExit, body);
	}

	public static void SendGuildAppointed(IEnumerable<ClientPeer> peers, Guid appointerId, string sAppointerName, int nAppointerGrade, Guid appointeeId, string sAppointeeName, int nAppointeeGrade)
	{
		SEBGuildAppointedEventBody body = new SEBGuildAppointedEventBody();
		body.appointerId = (Guid)appointerId;
		body.appointerName = sAppointerName;
		body.appointerGrade = nAppointerGrade;
		body.appointeeId = (Guid)appointeeId;
		body.appointeeName = sAppointeeName;
		body.appointeeGrade = nAppointeeGrade;
		Send(peers, ServerEventName.GuildAppointed, body);
	}

	public static void SendGuildMasterTransferred(IEnumerable<ClientPeer> peers, Guid transfererId, string transfererName, Guid transfereeId, string transfereeName)
	{
		SEBGuildMasterTransferredEventBody body = new SEBGuildMasterTransferredEventBody();
		body.transfererId = (Guid)transfererId;
		body.transfererName = transfererName;
		body.transfereeId = (Guid)transfereeId;
		body.transfereeName = transfereeName;
		Send(peers, ServerEventName.GuildMasterTransferred, body);
	}

	public static void SendGuildBanished(ClientPeer peer, int nMaxHp, int nHp, int nPreviousContinentId, int nPreviousNationId)
	{
		SEBGuildBanishedEventBody body = new SEBGuildBanishedEventBody();
		body.maxHp = nMaxHp;
		body.hp = nHp;
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		Send(peer, ServerEventName.GuildBanished, body);
	}

	public static void SendGuildInvitationArrived(ClientPeer peer, PDHeroGuildInvitation invitation)
	{
		SEBGuildInvitationArrivedEventBody body = new SEBGuildInvitationArrivedEventBody();
		body.invitation = invitation;
		Send(peer, ServerEventName.GuildInvitationArrived, body);
	}

	public static void SendGuildInvitationRefused(ClientPeer peer, Guid targetId, string sTargetName)
	{
		SEBGuildInvitationRefusedEventBody body = new SEBGuildInvitationRefusedEventBody();
		body.targetId = (Guid)targetId;
		body.targetName = sTargetName;
		Send(peer, ServerEventName.GuildInvitationRefused, body);
	}

	public static void SendGuildInvitationCanceled(ClientPeer peer, Guid invitationId)
	{
		SEBGuildInvitationCanceledEventBody body = new SEBGuildInvitationCanceledEventBody();
		body.id = (Guid)invitationId;
		Send(peer, ServerEventName.GuildInvitationCanceled, body);
	}

	public static void SendGuildInvitationLifetimeEnded(ClientPeer peer, Guid id, Guid targetId, string sTargetName)
	{
		SEBGuildInvitationLifetimeEndedEventBody body = new SEBGuildInvitationLifetimeEndedEventBody();
		body.invitationId = (Guid)id;
		body.targetId = (Guid)targetId;
		body.targetName = sTargetName;
		Send(peer, ServerEventName.GuildInvitationLifetimeEnded, body);
	}

	public static void SendHeroGuildInfoUpdated(IEnumerable<ClientPeer> peers, Guid heroId, Guid guildId, string guildName, int nGuildMemberGrade)
	{
		SEBHeroGuildInfoUpdatedEventBody body = new SEBHeroGuildInfoUpdatedEventBody();
		body.heroId = (Guid)heroId;
		body.guildId = (Guid)guildId;
		body.guildName = guildName;
		body.guildMemberGrade = nGuildMemberGrade;
		Send(peers, ServerEventName.HeroGuildInfoUpdated, body);
	}

	public static void SendGuildNoticeChanged(IEnumerable<ClientPeer> peers, string sNotice)
	{
		SEBGuildNoticeChangedEventBody body = new SEBGuildNoticeChangedEventBody();
		body.notice = sNotice;
		Send(peers, ServerEventName.GuildNoticeChanged, body);
	}

	public static void SendGuildFundChanged(IEnumerable<ClientPeer> peers, long lnFund)
	{
		SEBGuildFundChangedEventBody body = new SEBGuildFundChangedEventBody();
		body.fund = lnFund;
		Send(peers, ServerEventName.GuildFundChanged, body);
	}

	public static void SendGuildMoralPointChanged(IEnumerable<ClientPeer> peers, DateTime date, int nMoralPoint)
	{
		SEBGuildMoralPointChangedEventBody body = new SEBGuildMoralPointChangedEventBody();
		body.date = (DateTime)date;
		body.moralPoint = nMoralPoint;
		Send(peers, ServerEventName.GuildMoralPointChanged, body);
	}

	public static void SendGuildBuildingPointChanged(IEnumerable<ClientPeer> peers, int nBuildingPoint)
	{
		SEBGuildBuildingPointChangedEventBody body = new SEBGuildBuildingPointChangedEventBody();
		body.buildingPoint = nBuildingPoint;
		Send(peers, ServerEventName.GuildBuildingPointChanged, body);
	}

	public static void SendGuildBuildingLevelUp(IEnumerable<ClientPeer> peers, int nBuildingId, int nBuildingLevel)
	{
		SEBGuildBuildingLevelUpEventBody body = new SEBGuildBuildingLevelUpEventBody();
		body.buildingId = nBuildingId;
		body.buildingLevel = nBuildingLevel;
		Send(peers, ServerEventName.GuildBuildingLevelUp, body);
	}

	public static void SendSupplySupportQuestFail(ClientPeer peer, long lnGold, long lnMaxGold)
	{
		SEBSupplySupportQuestFailEventBody body = new SEBSupplySupportQuestFailEventBody();
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		Send(peer, ServerEventName.SupplySupportQuestFail, body);
	}

	public static void SendSupplySupportQuestCartDestructionReward(ClientPeer peer, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBSupplySupportQuestCartDestructionRewardEventBody body = new SEBSupplySupportQuestCartDestructionRewardEventBody();
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.SupplySupportQuestCartDestructionReward, body);
	}

	public static void SendGuildFarmQuestInteractionCompleted(ClientPeer peer)
	{
		Send(peer, ServerEventName.GuildFarmQuestInteractionCompleted, null);
	}

	public static void SendGuildFarmQuestInteractionCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.GuildFarmQuestInteractionCanceled, null);
	}

	public static void SendHeroGuildFarmQuestInteractionStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGuildFarmQuestInteractionStartedEventBody body = new SEBHeroGuildFarmQuestInteractionStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGuildFarmQuestInteractionStarted, body);
	}

	public static void SendHeroGuildFarmQuestInteractionCompleted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGuildFarmQuestInteractionCompletedEventBody body = new SEBHeroGuildFarmQuestInteractionCompletedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGuildFarmQuestInteractionCompleted, body);
	}

	public static void SendHeroGuildFarmQuestInteractionCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGuildFarmQuestInteractionCanceledEventBody body = new SEBHeroGuildFarmQuestInteractionCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGuildFarmQuestInteractionCanceled, body);
	}

	public static void SendGuildFoodWarehouseCollected(IEnumerable<ClientPeer> peers, Guid collectionId)
	{
		SEBGuildFoodWarehouseCollectedEventBody body = new SEBGuildFoodWarehouseCollectedEventBody();
		body.collectionId = (Guid)collectionId;
		Send(peers, ServerEventName.GuildFoodWarehouseCollected, body);
	}

	public static void SendGuildAltarSpellInjectionMissionCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.GuildAltarSpellInjectionMissionCanceled, null);
	}

	public static void SendGuildAltarSpellInjectionMissionCompleted(ClientPeer peer, DateTime date, int nGuildMoralPoint, int nGIMoralPoint)
	{
		SEBGuildAltarSpellInjectionMissionCompletedEventBody body = new SEBGuildAltarSpellInjectionMissionCompletedEventBody();
		body.date = (DateTime)date;
		body.guildMoralPoint = nGuildMoralPoint;
		body.giMoralPoint = nGIMoralPoint;
		Send(peer, ServerEventName.GuildAltarSpellInjectionMissionCompleted, body);
	}

	public static void SendHeroGuildAltarSpellInjectionMissionStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGuildAltarSpellInjectionMissionStartedEventBody body = new SEBHeroGuildAltarSpellInjectionMissionStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGuildAltarSpellInjectionMissionStarted, body);
	}

	public static void SendHeroGuildAltarSpellInjectionMissionCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGuildAltarSpellInjectionMissionCanceledEventBody body = new SEBHeroGuildAltarSpellInjectionMissionCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGuildAltarSpellInjectionMissionCanceled, body);
	}

	public static void SendHeroGuildAltarSpellInjectionMissionCompleted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGuildAltarSpellInjectionMissionCompletedEventBody body = new SEBHeroGuildAltarSpellInjectionMissionCompletedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGuildAltarSpellInjectionMissionCompleted, body);
	}

	public static void SendGuildAltarDefenseMissionFailed(ClientPeer peer)
	{
		Send(peer, ServerEventName.GuildAltarDefenseMissionFailed, null);
	}

	public static void SendGuildAltarDefenseMissionCompleted(ClientPeer peer, DateTime date, int nGuildMoralPoint, int nGIMoralPoint)
	{
		SEBGuildAltarDefenseMissionCompletedEventBody body = new SEBGuildAltarDefenseMissionCompletedEventBody();
		body.date = (DateTime)date;
		body.guildMoralPoint = nGuildMoralPoint;
		body.giMoralPoint = nGIMoralPoint;
		Send(peer, ServerEventName.GuildAltarDefenseMissionCompleted, body);
	}

	public static void SendGuildAltarCompleted(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHp, int nHp, int nContributionPoint, int nTotalContributionPoint, long lnGiFund, int nGiBuildingPoint)
	{
		SEBGuildAltarCompletedEventBody body = new SEBGuildAltarCompletedEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = nMaxHp;
		body.hp = nHp;
		body.contributionPoint = nContributionPoint;
		body.totalContributionPoint = nTotalContributionPoint;
		body.giFund = lnGiFund;
		body.giBuildingPoint = nGiBuildingPoint;
		Send(peer, ServerEventName.GuildAltarCompleted, body);
	}

	public static void SendGuildMissionUpdated(ClientPeer peer, int nProgressCount)
	{
		SEBGuildMissionUpdatedEventBody body = new SEBGuildMissionUpdatedEventBody();
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.GuildMissionUpdated, body);
	}

	public static void SendGuildMissionFailed(ClientPeer peer)
	{
		Send(peer, ServerEventName.GuildMissionFailed, null);
	}

	public static void SendGuildMissionCompleted(ClientPeer peer, DateTime date, PDHeroGuildMission nextMission, int nCompletedMissionCount, long lnAcquiredExp, int nMaxHP, int nHP, long lnExp, int nLevel, int nTotalGuildContributionPoint, int nGuildContributionPoint, PDInventorySlot[] changedInventorySlots, long giFund, int giBuildingPoint)
	{
		SEBGuildMissionCompletedEventBody body = new SEBGuildMissionCompletedEventBody();
		body.date = (DateTime)date;
		body.nextMission = nextMission;
		body.completedMissionCount = nCompletedMissionCount;
		body.acquiredExp = lnAcquiredExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.exp = lnExp;
		body.level = nLevel;
		body.totalGuildContributionPoint = nTotalGuildContributionPoint;
		body.guildContributionPoint = nGuildContributionPoint;
		body.changedInventorySlots = changedInventorySlots;
		body.giFund = giFund;
		body.giBuildingPoint = giBuildingPoint;
		Send(peer, ServerEventName.GuildMissionCompleted, body);
	}

	public static void SendGuildSpiritAnnounced(IEnumerable<ClientPeer> peers, Guid guildId, string sGuildName, Guid heroId, string sHeroName, int nContinentId)
	{
		SEBGuildSpiritAnnouncedEventBody body = new SEBGuildSpiritAnnouncedEventBody();
		body.guildId = (Guid)guildId;
		body.guildName = sGuildName;
		body.heroId = (Guid)heroId;
		body.heroName = sHeroName;
		body.continentId = nContinentId;
		Send(peers, ServerEventName.GuildSpiritAnnounced, body);
	}

	public static void SendGuildCall(IEnumerable<ClientPeer> peers, PDGuildCall call)
	{
		SEBGuildCallEventBody body = new SEBGuildCallEventBody();
		body.call = call;
		Send(peers, ServerEventName.GuildCall, body);
	}

	public static void SendGuildSupplySupportQuestStarted(IEnumerable<ClientPeer> peers, DateTime date, int nDailyGuildSupplySupportQuestStartCount)
	{
		SEBGuildSupplySupportQuestStartedEventBody body = new SEBGuildSupplySupportQuestStartedEventBody();
		body.date = (DateTime)date;
		body.dailyGuildSupplySupportQuestStartCount = nDailyGuildSupplySupportQuestStartCount;
		Send(peers, ServerEventName.GuildSupplySupportQuestStarted, body);
	}

	public static void SendGuildSupplySupportQuestCompleted(ClientPeer peer, long lnacquiredExp, int nMaxHP, int nHP, long lnExp, int nLevel, int nTotalGuildContributionPoint, int nGuildContributionPoint)
	{
		SEBGuildSupplySupportQuestCompletedEventBody body = new SEBGuildSupplySupportQuestCompletedEventBody();
		body.acquiredExp = lnacquiredExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.exp = lnExp;
		body.level = nLevel;
		body.totalGuildContributionPoint = nTotalGuildContributionPoint;
		body.guildContributionPoint = nGuildContributionPoint;
		Send(peer, ServerEventName.GuildSupplySupportQuestCompleted, body);
	}

	public static void SendGuildSupplySupportQuestFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.GuildSupplySupportQuestFail, null);
	}

	public static void SendGuildHuntingQuestUpdated(ClientPeer peer, int nProgressCount)
	{
		SEBGuildHuntingQuestUpdatedEventBody body = new SEBGuildHuntingQuestUpdatedEventBody();
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.GuildHuntingQuestUpdated, body);
	}

	public static void SendGuildHuntingDonationCountUpdated(IEnumerable<ClientPeer> peers, int nDonationCount)
	{
		SEBGuildHuntingDonationCountUpdatedEventBody body = new SEBGuildHuntingDonationCountUpdatedEventBody();
		body.donationCount = nDonationCount;
		Send(peers, ServerEventName.GuildHuntingDonationCountUpdated, body);
	}

	public static void SendGuildDailyObjectiveSet(IEnumerable<ClientPeer> peers, DateTime date, int nContentId)
	{
		SEBGuildDailyObjectiveSetEventBody body = new SEBGuildDailyObjectiveSetEventBody();
		body.date = (DateTime)date;
		body.contentId = nContentId;
		Send(peers, ServerEventName.GuildDailyObjectiveSet, body);
	}

	public static void SendGuildDailyObjectiveNotice(IEnumerable<ClientPeer> peers, PDSimpleHero hero, int nContentId)
	{
		SEBGuildDailyObjectiveNoticeEventBody body = new SEBGuildDailyObjectiveNoticeEventBody();
		body.hero = hero;
		body.contentId = nContentId;
		Send(peers, ServerEventName.GuildDailyObjectiveNotice, body);
	}

	public static void SendGuildDailyObjectiveCompletionMemberCountUpdated(IEnumerable<ClientPeer> peers, int nCount)
	{
		SEBGuildDailyObjectiveCompletionMemberCountUpdatedEventBody body = new SEBGuildDailyObjectiveCompletionMemberCountUpdatedEventBody();
		body.count = nCount;
		Send(peers, ServerEventName.GuildDailyObjectiveCompletionMemberCountUpdated, body);
	}

	public static void SendGuildWeeklyObjectiveSet(IEnumerable<ClientPeer> peers, DateTime date, int nObjectiveId)
	{
		SEBGuildWeeklyObjectiveSetEventBody body = new SEBGuildWeeklyObjectiveSetEventBody();
		body.date = (DateTime)date;
		body.objectiveId = nObjectiveId;
		Send(peers, ServerEventName.GuildWeeklyObjectiveSet, body);
	}

	public static void SendGuildWeeklyObjectiveCompletionMemberCountUpdated(IEnumerable<ClientPeer> peers, int nCount)
	{
		SEBGuildWeeklyObjectiveCompletionMemberCountUpdatedEventBody body = new SEBGuildWeeklyObjectiveCompletionMemberCountUpdatedEventBody();
		body.count = nCount;
		Send(peers, ServerEventName.GuildWeeklyObjectiveCompletionMemberCountUpdated, body);
	}

	public static void SendWingAcquisition(ClientPeer peer, PDHeroWing wing)
	{
		SEBWingAcquisitionEventBody body = new SEBWingAcquisitionEventBody();
		body.wing = wing;
		Send(peer, ServerEventName.WingAcquisition, body);
	}

	public static void SendNationNoblesseAppointment(IEnumerable<ClientPeer> peers, int nNationId, int nNoblesseId, Guid heroId, string sHeroName, int nHeroJobId, DateTime appointmentDate)
	{
		SEBNationNoblesseAppointmentEventBody body = new SEBNationNoblesseAppointmentEventBody();
		body.nationId = nNationId;
		body.noblesseId = nNoblesseId;
		body.heroId = (Guid)heroId;
		body.heroName = sHeroName;
		body.heroJobId = nHeroJobId;
		body.appointmentDate = (DateTime)appointmentDate;
		Send(peers, ServerEventName.NationNoblesseAppointment, body);
	}

	public static void SendNationNoblesseDismissal(IEnumerable<ClientPeer> peers, int nNationId, int nNoblesseId)
	{
		SEBNationNoblesseDismissalEventBody body = new SEBNationNoblesseDismissalEventBody();
		body.nationId = nNationId;
		body.noblesseId = nNoblesseId;
		Send(peers, ServerEventName.NationNoblesseDismissal, body);
	}

	public static void SendNationWarDeclaration(IEnumerable<ClientPeer> peers, PDNationWarDeclaration nationWarDeclaration)
	{
		SEBNationWarDeclarationEventBody body = new SEBNationWarDeclarationEventBody();
		body.nationWarDeclaration = nationWarDeclaration;
		Send(peers, ServerEventName.NationWarDeclaration, body);
	}

	public static void SendNationWarStart(IEnumerable<ClientPeer> peers, Guid declarationId)
	{
		SEBNationWarStartEventBody body = new SEBNationWarStartEventBody();
		body.declarationId = (Guid)declarationId;
		Send(peers, ServerEventName.NationWarStart, body);
	}

	public static void SendNationWarCall(IEnumerable<ClientPeer> peers, Guid callerId, string sCallerName, int nCallerNoblesseId, DateTime date, int nDailyNationWarCallCount, float fNationWarCallRemainingCoolTime)
	{
		SEBNationWarCallEventBody body = new SEBNationWarCallEventBody();
		body.callerId = (Guid)callerId;
		body.callerName = sCallerName;
		body.callerNoblesseId = nCallerNoblesseId;
		body.date = (DateTime)date;
		body.dailyNationWarCallCount = nDailyNationWarCallCount;
		body.nationWarCallRemainingCoolTime = fNationWarCallRemainingCoolTime;
		Send(peers, ServerEventName.NationWarCall, body);
	}

	public static void SendNationWarConvergingAttack(IEnumerable<ClientPeer> peers, int nTargetMonsterArrangeId, DateTime date, int nDailyNationWarConvergingAttackCount, float fNationWarConvergingAttackReminaingCoolTime)
	{
		SEBNationWarConvergingAttackEventBody body = new SEBNationWarConvergingAttackEventBody();
		body.targetMonsterArrangeId = nTargetMonsterArrangeId;
		body.date = (DateTime)date;
		body.dailyNationWarConvergingAttackCount = nDailyNationWarConvergingAttackCount;
		body.nationWarConvergingAttackReminaingCoolTime = fNationWarConvergingAttackReminaingCoolTime;
		Send(peers, ServerEventName.NationWarConvergingAttack, body);
	}

	public static void SendNationWarConvergingAttackFinished(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.NationWarConvergingAttackFinished, null);
	}

	public static void SendNationWarKillCountUpdated(ClientPeer peer, int nKillCount, int nAccKillCount)
	{
		SEBNationWarKillCountUpdatedEventBody body = new SEBNationWarKillCountUpdatedEventBody();
		body.killCount = nKillCount;
		body.accKillCount = nAccKillCount;
		Send(peer, ServerEventName.NationWarKillCountUpdated, body);
	}

	public static void SendNationWarMultiKill(IEnumerable<ClientPeer> peers, Guid heroId, string sHeroName, int nNationId, int nKillCount)
	{
		SEBNationWarMultiKillEventBody body = new SEBNationWarMultiKillEventBody();
		body.heroId = (Guid)heroId;
		body.heroName = sHeroName;
		body.nationId = nNationId;
		body.killCount = nKillCount;
		Send(peers, ServerEventName.NationWarMultiKill, body);
	}

	public static void SendNationWarNoblesseKill(IEnumerable<ClientPeer> peers, Guid killerId, string sKillerName, int nKillerNationId, Guid deadHeroId, string sDeadHeroName, int nDeadNoblesseId, int nDeadNationId)
	{
		SEBNationWarNoblesseKillEventBody body = new SEBNationWarNoblesseKillEventBody();
		body.killerId = (Guid)killerId;
		body.killerName = sKillerName;
		body.killerNationId = nKillerNationId;
		body.deadHeroId = (Guid)deadHeroId;
		body.deadHeroName = sDeadHeroName;
		body.deadNoblesseId = nDeadNoblesseId;
		body.deadNationId = nDeadNationId;
		Send(peers, ServerEventName.NationWarNoblesseKill, body);
	}

	public static void SendNationWarAssistCountUpdated(ClientPeer peer, int nAssistCount)
	{
		SEBNationWarAssistCountUpdatedEventBody body = new SEBNationWarAssistCountUpdatedEventBody();
		body.assistCount = nAssistCount;
		Send(peer, ServerEventName.NationWarAssistCountUpdated, body);
	}

	public static void SendNationWarDeadCountUpdated(ClientPeer peer, int nDeadCount)
	{
		SEBNationWarDeadCountUpdatedEventBody body = new SEBNationWarDeadCountUpdatedEventBody();
		body.deadCount = nDeadCount;
		Send(peer, ServerEventName.NationWarDeadCountUpdated, body);
	}

	public static void SendNationWarImmediateRevivalCountUpdated(ClientPeer peer, int nImmediateRevivalCount, int nAccImmediateRevivalCount)
	{
		SEBNationWarImmediateRevivalCountUpdatedEventBody body = new SEBNationWarImmediateRevivalCountUpdatedEventBody();
		body.immediateRevivalCount = nImmediateRevivalCount;
		body.accImmediateRevivalCount = nAccImmediateRevivalCount;
		Send(peer, ServerEventName.NationWarImmediateRevivalCountUpdated, body);
	}

	public static void SendNationWarWin(ClientPeer peer, PDItemBooty[] joinBooties, long lnJoinAcquiredExp, int nJoinAcquiredExploitPoint, int nObjectiveAchievemnetOwnDia, int nObjectiveAchievementAcquiredExploitPoint, PDItemBooty rankingBooty, PDItemBooty luckyBooty, int nLevel, long lnExp, int nExploitPoint, int nMaxHp, int nHp, int nOwnDia, DateTime date, int nDailyExploitPoint, PDInventorySlot[] changedInventorySlots, int nAccNationWarWinCount)
	{
		SEBNationWarWinEventBody body = new SEBNationWarWinEventBody();
		body.joinBooties = joinBooties;
		body.joinAcquiredExp = lnJoinAcquiredExp;
		body.joinAcquiredExploitPoint = nJoinAcquiredExploitPoint;
		body.objectiveAchievementOwnDia = nObjectiveAchievemnetOwnDia;
		body.objectiveAchievementAcquiredExploitPoint = nObjectiveAchievementAcquiredExploitPoint;
		body.rankingBooty = rankingBooty;
		body.luckyBooty = luckyBooty;
		body.level = nLevel;
		body.exp = lnExp;
		body.exploitPoint = nExploitPoint;
		body.maxHp = nMaxHp;
		body.hp = nHp;
		body.ownDia = nOwnDia;
		body.date = (DateTime)date;
		body.dailyExploitPoint = nDailyExploitPoint;
		body.changedInventorySlots = changedInventorySlots;
		body.accNationWarWinCount = nAccNationWarWinCount;
		Send(peer, ServerEventName.NationWarWin, body);
	}

	public static void SendNationWarLose(ClientPeer peer, PDItemBooty[] joinBooties, long lnJoinAcquiredExp, int nJoinAcquiredExploitPoint, int nObjectiveAchievemnetOwnDia, int nObjectiveAchievementAcquiredExploitPoint, PDItemBooty rankingBooty, PDItemBooty luckyBooty, int nLevel, long lnExp, int nExploitPoint, int nMaxHp, int nHp, int nOwnDia, DateTime date, int nDailyExploitPoint, PDInventorySlot[] changedInventorySlots)
	{
		SEBNationWarLoseEventBody body = new SEBNationWarLoseEventBody();
		body.joinBooties = joinBooties;
		body.joinAcquiredExp = lnJoinAcquiredExp;
		body.joinAcquiredExploitPoint = nJoinAcquiredExploitPoint;
		body.objectiveAchievementOwnDia = nObjectiveAchievemnetOwnDia;
		body.objectiveAchievementAcquiredExploitPoint = nObjectiveAchievementAcquiredExploitPoint;
		body.rankingBooty = rankingBooty;
		body.luckyBooty = luckyBooty;
		body.level = nLevel;
		body.exp = lnExp;
		body.exploitPoint = nExploitPoint;
		body.maxHp = nMaxHp;
		body.hp = nHp;
		body.ownDia = nOwnDia;
		body.date = (DateTime)date;
		body.dailyExploitPoint = nDailyExploitPoint;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.NationWarLose, body);
	}

	public static void SendNationWarAllianceNationReward(ClientPeer peer, bool bIsWin, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBNationWarAllianceNationRewardEventBody body = new SEBNationWarAllianceNationRewardEventBody();
		body.isWin = bIsWin;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.NationWarAllianceNationReward, body);
	}

	public static void SendNationWarFinished(IEnumerable<ClientPeer> peers, Guid declarationId, int nWinNationId)
	{
		SEBNationWarFinishedEventBody body = new SEBNationWarFinishedEventBody();
		body.declarationId = (Guid)declarationId;
		body.winNationId = nWinNationId;
		Send(peers, ServerEventName.NationWarFinished, body);
	}

	public static void SendNationWarMonsterBattleModeStart(IEnumerable<ClientPeer> peers, int nMonsterArrangeId)
	{
		SEBNationWarMonsterBattleModeStartEventBody body = new SEBNationWarMonsterBattleModeStartEventBody();
		body.monsterArrangeId = nMonsterArrangeId;
		Send(peers, ServerEventName.NationWarMonsterBattleModeStart, body);
	}

	public static void SendNationWarMonsterBattleModeEnd(IEnumerable<ClientPeer> peers, int nMonsterArrangeId)
	{
		SEBNationWarMonsterBattleModeEndEventBody body = new SEBNationWarMonsterBattleModeEndEventBody();
		body.monsterArrangeId = nMonsterArrangeId;
		Send(peers, ServerEventName.NationWarMonsterBattleModeEnd, body);
	}

	public static void SendNationWarMonsterEmergency(IEnumerable<ClientPeer> peers, int nMonsterArrangeId)
	{
		SEBNationWarMonsterEmergencyEventBody body = new SEBNationWarMonsterEmergencyEventBody();
		body.monsterArrangeId = nMonsterArrangeId;
		Send(peers, ServerEventName.NationWarMonsterEmergency, body);
	}

	public static void SendNationWarMonsterDead(IEnumerable<ClientPeer> peers, int nMonsterArrangeId)
	{
		SEBNationWarMonsterDeadEventBody body = new SEBNationWarMonsterDeadEventBody();
		body.monsterArrangeId = nMonsterArrangeId;
		Send(peers, ServerEventName.NationWarMonsterDead, body);
	}

	public static void SendNationWarMonsterSpawn(IEnumerable<ClientPeer> peers, int nMonsterArrangeId, int nNationId)
	{
		SEBNationWarMonsterSpawnEventBody body = new SEBNationWarMonsterSpawnEventBody();
		body.monsterArrangeId = nMonsterArrangeId;
		body.nationId = nNationId;
		Send(peers, ServerEventName.NationWarMonsterSpawn, body);
	}

	public static void SendNationCall(IEnumerable<ClientPeer> peers, PDNationCall call)
	{
		SEBNationCallEventBody body = new SEBNationCallEventBody();
		body.call = call;
		Send(peers, ServerEventName.NationCall, body);
	}

	public static void SendContinentExitForSoulCoveterEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForSoulCoveterEnter, null);
	}

	public static void SendSoulCoveterMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nDifficulty, int nMatchingStatus, float fRemainingTime)
	{
		SEBSoulCoveterMatchingRoomPartyEnterEventBody body = new SEBSoulCoveterMatchingRoomPartyEnterEventBody();
		body.difficulty = nDifficulty;
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.SoulCoveterMatchingRoomPartyEnter, body);
	}

	public static void SendSoulCoveterMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBSoulCoveterMatchingRoomBanishedEventBody body = new SEBSoulCoveterMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.SoulCoveterMatchingRoomBanished, body);
	}

	public static void SendSoulCoveterMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBAncientRelicMatchingStatusChangedEventBody body = new SEBAncientRelicMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.SoulCoveterMatchingStatusChanged, body);
	}

	public static void SendSoulCoveterWaveStart(IEnumerable<ClientPeer> peers, int nWaveNo, PDSoulCoveterMonsterInstance[] monsterInsts)
	{
		SEBSoulCoveterWaveStartEventBody body = new SEBSoulCoveterWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.SoulCoveterWaveStart, body);
	}

	public static void SendSoulCoveterClear(ClientPeer peer, PDItemBooty[] booties, PDInventorySlot[] changedInventorySlots)
	{
		SEBSoulCoveterClearEventBody body = new SEBSoulCoveterClearEventBody();
		body.booties = booties;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.SoulCoveterClear, body);
	}

	public static void SendSoulCoveterFail(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.SoulCoveterFail, null);
	}

	public static void SendSoulCoveterBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBSoulCoveterBanishedEventBody body = new SEBSoulCoveterBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.SoulCoveterBanished, body);
	}

	public static void SendSceneryQuestCanceled(ClientPeer peer, int nQuestId)
	{
		SEBSceneryQuestCanceledEventBody body = new SEBSceneryQuestCanceledEventBody();
		body.questId = nQuestId;
		Send(peer, ServerEventName.SceneryQuestCanceled, body);
	}

	public static void SendSceneryQuestCompleted(ClientPeer peer, int nQuestId, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBSceneryQuestCompletedEventBody body = new SEBSceneryQuestCompletedEventBody();
		body.questId = nQuestId;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.SceneryQuestCompleted, body);
	}

	public static void SendAccNationWarCommanderKillCountUpdated(ClientPeer peer, int nCount)
	{
		SEBAccNationWarCommanderKillCountUpdatedEventBody body = new SEBAccNationWarCommanderKillCountUpdatedEventBody();
		body.count = nCount;
		Send(peer, ServerEventName.AccNationWarCommanderKillCountUpdated, body);
	}

	public static void SendAccMonsterKillCountUpdated(ClientPeer peer, int nCount)
	{
		SEBAccMonsterKillCountUpdatedEventBody body = new SEBAccMonsterKillCountUpdatedEventBody();
		body.count = nCount;
		Send(peer, ServerEventName.AccMonsterKillCountUpdated, body);
	}

	public static void SendTitleLifetimeEnded(ClientPeer peer, int nTitleId)
	{
		SEBTitleLifetimeEndedEventBody body = new SEBTitleLifetimeEndedEventBody();
		body.titleId = nTitleId;
		Send(peer, ServerEventName.TitleLifetimeEnded, body);
	}

	public static void SendHeroDisplayTitleChanged(IEnumerable<ClientPeer> peers, Guid heroId, int nTitleId)
	{
		SEBHeroDisplayTitleChangedEventBody body = new SEBHeroDisplayTitleChangedEventBody();
		body.heroId = (Guid)heroId;
		body.titleId = nTitleId;
		Send(peers, ServerEventName.HeroDisplayTitleChanged, body);
	}

	public static void SendCreatureCardShopRefreshed(ClientPeer peer, PDHeroCreatureCardShopRandomProduct[] randomProducts)
	{
		SEBCreatureCardShopRefreshedEventBody body = new SEBCreatureCardShopRefreshedEventBody();
		body.randomProducts = randomProducts;
		Send(peer, ServerEventName.CreatureCardShopRefreshed, body);
	}

	public static void SendEliteMonsterKillCountUpdated(ClientPeer peer, int nEliteMonsterId, int nKillCount, int nMaxHP)
	{
		SEBEliteMonsterKillCountUpdatedEventBody body = new SEBEliteMonsterKillCountUpdatedEventBody();
		body.eliteMonsterId = nEliteMonsterId;
		body.killCount = nKillCount;
		body.maxHP = nMaxHP;
		Send(peer, ServerEventName.EliteMonsterKillCountUpdated, body);
	}

	public static void SendEliteMonsterSpawn(IEnumerable<ClientPeer> peers, int nEliteMonsterId)
	{
		SEBEliteMonsterSpawnEventBody body = new SEBEliteMonsterSpawnEventBody();
		body.eliteMonsterId = nEliteMonsterId;
		Send(peers, ServerEventName.EliteMonsterSpawn, body);
	}

	public static void SendEliteMonsterRemoved(IEnumerable<ClientPeer> peers, int nEliteMonsterId)
	{
		SEBEliteMonsterRemovedEventBody body = new SEBEliteMonsterRemovedEventBody();
		body.eliteMonsterId = nEliteMonsterId;
		Send(peers, ServerEventName.EliteMonsterRemoved, body);
	}

	public static void SendEliteDungeonStart(ClientPeer peer)
	{
		Send(peer, ServerEventName.EliteDungeonStart, null);
	}

	public static void SendEliteDungeonClear(ClientPeer peer)
	{
		Send(peer, ServerEventName.EliteDungeonClear, null);
	}

	public static void SendEliteDungeonFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.EliteDungeonFail, null);
	}

	public static void SendEliteDungeonBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBEliteDungeonBanishedEventBody body = new SEBEliteDungeonBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.EliteDungeonBanished, body);
	}

	public static void SendProofOfValorRefreshed(ClientPeer peer, PDHeroProofOfValorInstance heroProofOfValorInst, int nProofOfValorPaidRefreshCount)
	{
		SEBProofOfValorRefreshedEventBody body = new SEBProofOfValorRefreshedEventBody();
		body.heroProofOfValorInst = heroProofOfValorInst;
		body.proofOfValorPaidRefreshCount = nProofOfValorPaidRefreshCount;
		Send(peer, ServerEventName.ProofOfValorRefreshed, body);
	}

	public static void SendProofOfValorStart(ClientPeer peer)
	{
		Send(peer, ServerEventName.ProofOfValorStart, null);
	}

	public static void SendProofOfValorClear(ClientPeer peer, int nClearGrade, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, int nSoulPowder, PDHeroCreatureCard changedCreatureCard, PDHeroProofOfValorInstance heroProofOfValorInst, int nProofOfValorPaidRefreshCount)
	{
		SEBProofOfValorClearEventBody body = new SEBProofOfValorClearEventBody();
		body.clearGrade = nClearGrade;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.soulPowder = nSoulPowder;
		body.changedCreatureCard = changedCreatureCard;
		body.heroProofOfValorInst = heroProofOfValorInst;
		body.proofOfValorPaidRefreshCount = nProofOfValorPaidRefreshCount;
		Send(peer, ServerEventName.ProofOfValorClear, body);
	}

	public static void SendProofOfValorFail(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, int nSoulPowder, PDHeroProofOfValorInstance heroProofOfValorInst, int nProofOfValorPaidRefreshCount)
	{
		SEBProofOfValorFailEventBody body = new SEBProofOfValorFailEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.soulPowder = nSoulPowder;
		body.heroProofOfValorInst = heroProofOfValorInst;
		body.proofOfValorPaidRefreshCount = nProofOfValorPaidRefreshCount;
		Send(peer, ServerEventName.ProofOfValorFail, body);
	}

	public static void SendProofOfValorBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBProofOfValorBanishedEventBody body = new SEBProofOfValorBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.ProofOfValorBanished, body);
	}

	public static void SendProofOfValorBuffBoxCreated(ClientPeer peer, PDProofOfValorBuffBoxInstance[] buffBoxInsts)
	{
		SEBProofOfValorBuffBoxCreatedEventBody body = new SEBProofOfValorBuffBoxCreatedEventBody();
		body.buffBoxInsts = buffBoxInsts;
		Send(peer, ServerEventName.ProofOfValorBuffBoxCreated, body);
	}

	public static void SendProofOfValorBuffBoxLifetimeEnded(ClientPeer peer)
	{
		Send(peer, ServerEventName.ProofOfValorBuffBoxLifetimeEnded, null);
	}

	public static void SendProofOfValorBuffFinished(ClientPeer peer)
	{
		Send(peer, ServerEventName.ProofOfValorBuffFinished, null);
	}

	public static void SendNotice(IEnumerable<ClientPeer> peers, string content)
	{
		SEBNoticeEventBody body = new SEBNoticeEventBody();
		body.content = content;
		Send(peers, ServerEventName.Notice, body);
	}

	public static void SendHeroJobCommonSkillCast(IEnumerable<ClientPeer> peers, Guid heroId, int nSkillId, PDVector3 skillTargetPosition)
	{
		SEBHeroJobCommonSkillCastEventBody body = new SEBHeroJobCommonSkillCastEventBody();
		body.heroId = (Guid)heroId;
		body.skillId = nSkillId;
		body.skillTargetPosition = skillTargetPosition;
		Send(peers, ServerEventName.HeroJobCommonSkillCast, body);
	}

	public static void SendGroggyMonsterItemStealFinished(ClientPeer peer, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBGroggyMonsterItemStealFinishedEventBody body = new SEBGroggyMonsterItemStealFinishedEventBody();
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.GroggyMonsterItemStealFinished, body);
	}

	public static void SendGroggyMonsterItemStealCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.GroggyMonsterItemStealCancel, null);
	}

	public static void SendHeroGroggyMonsterItemStealStart(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGroggyMonsterItemStealStartEventBody body = new SEBHeroGroggyMonsterItemStealStartEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGroggyMonsterItemStealStart, body);
	}

	public static void SendHeroGroggyMonsterItemStealFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGroggyMonsterItemStealFinishedEventBody body = new SEBHeroGroggyMonsterItemStealFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGroggyMonsterItemStealFinished, body);
	}

	public static void SendHeroGroggyMonsterItemStealCancel(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroGroggyMonsterItemStealCancelEventBody body = new SEBHeroGroggyMonsterItemStealCancelEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroGroggyMonsterItemStealCancel, body);
	}

	public static void SendHeroDailyQuestProgressCountUpdated(ClientPeer peer, PDHeroDailyQuestProgressCount[] progressCounts)
	{
		SEBHeroDailyQuestProgressCountUpdatedEventBody body = new SEBHeroDailyQuestProgressCountUpdatedEventBody();
		body.progressCounts = progressCounts;
		Send(peer, ServerEventName.HeroDailyQuestProgressCountUpdated, body);
	}

	public static void SendHeroDailyQuestCreated(ClientPeer peer, PDHeroDailyQuest[] quests)
	{
		SEBHeroDailyQuestCreatedEventBody body = new SEBHeroDailyQuestCreatedEventBody();
		body.quests = quests;
		Send(peer, ServerEventName.HeroDailyQuestCreated, body);
	}

	public static void SendWeeklyQuestCreated(ClientPeer peer, PDHeroWeeklyQuest quest)
	{
		SEBWeeklyQuestCreatedEventBody body = new SEBWeeklyQuestCreatedEventBody();
		body.quest = quest;
		Send(peer, ServerEventName.WeeklyQuestCreated, body);
	}

	public static void SendWeeklyQuestRoundProgressCountUpdated(ClientPeer peer, Guid roundId, int nRoundProgressCount)
	{
		SEBWeeklyQuestRoundProgressCountUpdatedEventBody body = new SEBWeeklyQuestRoundProgressCountUpdatedEventBody();
		body.roundId = (Guid)roundId;
		body.roundProgressCount = nRoundProgressCount;
		Send(peer, ServerEventName.WeeklyQuestRoundProgressCountUpdated, body);
	}

	public static void SendWeeklyQuestRoundCompletedEventBody(ClientPeer peer, long lnGold, long lnMaxGold, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, PDInventorySlot[] changedInventorySlots, int nNextRoundNo, Guid nextRoundId, int nNextRoundMissionId)
	{
		SEBWeeklyQuestRoundCompletedEventBody body = new SEBWeeklyQuestRoundCompletedEventBody();
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.changedInventorySlots = changedInventorySlots;
		body.nextRoundNo = nNextRoundNo;
		body.nextRoundId = (Guid)nextRoundId;
		body.nextRoundMissionId = nNextRoundMissionId;
		Send(peer, ServerEventName.WeeklyQuestRoundCompleted, body);
	}

	public static void SendWisdomTempleStepStart(ClientPeer peer, int nStepNo, int nPuzzleId, int nQuizNo, PDWisdomTempleMonsterInstance[] monsterInsts, PDWisdomTempleColorMatchingObjectInstance[] colorMatchingObjectInsts)
	{
		SEBWisdomTempleStepStartEventBody body = new SEBWisdomTempleStepStartEventBody();
		body.stepNo = nStepNo;
		body.puzzleId = nPuzzleId;
		body.quizNo = nQuizNo;
		body.monsterInsts = monsterInsts;
		body.colorMatchingObjectInsts = colorMatchingObjectInsts;
		Send(peer, ServerEventName.WisdomTempleStepStart, body);
	}

	public static void SendWisdomTempleStepCompleted(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBWisdomTempleStepCompletedEventBody body = new SEBWisdomTempleStepCompletedEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.WisdomTempleStepCompleted, body);
	}

	public static void SendWisdomTempleColorMatchingObjectInteractionFinished(ClientPeer peer, PDWisdomTempleColorMatchingObjectInstance colorMatchingObjectInst)
	{
		SEBWisdomTempleColorMatchingObjectInteractionFinishedEventBody body = new SEBWisdomTempleColorMatchingObjectInteractionFinishedEventBody();
		body.colorMatchingObjectInst = colorMatchingObjectInst;
		Send(peer, ServerEventName.WisdomTempleColorMatchingObjectInteractionFinished, body);
	}

	public static void SendWisdomTempleColorMatchingObjectInteractionCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.WisdomTempleColorMatchingObjectInteractionCancel, null);
	}

	public static void SendWisdomTempleColorMatchingMonsterCreated(ClientPeer peer, PDWisdomTempleColorMatchingMonsterInstance monsterInst)
	{
		SEBWisdomTempleColorMatchingMonsterCreatedEventBody body = new SEBWisdomTempleColorMatchingMonsterCreatedEventBody();
		body.monsterInst = monsterInst;
		Send(peer, ServerEventName.WisdomTempleColorMatchingMonsterCreated, body);
	}

	public static void SendWisdomTempleColorMatchingMonsterkill(ClientPeer peer, int nColorMatchingPoint, PDWisdomTempleColorMatchingObjectInstance[] createdColorMatchingObjectInst)
	{
		SEBWisdomTempleColorMatchingMonsterKillEventBody body = new SEBWisdomTempleColorMatchingMonsterKillEventBody();
		body.colorMatchingPoint = nColorMatchingPoint;
		body.createdColorMatchingObjectInsts = createdColorMatchingObjectInst;
		Send(peer, ServerEventName.WisdomTempleColorMatchingMonsterKill, body);
	}

	public static void SendWisdomTempleFakeTreasureBoxKill(ClientPeer peer, int nRow, int nCol, bool bExistAroundRealTreasureBox)
	{
		SEBWisdomTempleFakeTreasureBoxKillEventBody body = new SEBWisdomTempleFakeTreasureBoxKillEventBody();
		body.row = nRow;
		body.col = nCol;
		body.existAroundRealTreasureBox = bExistAroundRealTreasureBox;
		Send(peer, ServerEventName.WisdomTempleFakeTreasureBoxKill, body);
	}

	public static void SendWisdomTemplePuzzleCompleted(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, PDWisdomTemplePuzzleRewardObjectInstance[] puzzleRewardObjectInsts)
	{
		SEBWisdomTemplePuzzleCompletedEventBody body = new SEBWisdomTemplePuzzleCompletedEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.puzzleRewardObjectInsts = puzzleRewardObjectInsts;
		Send(peer, ServerEventName.WisdomTemplePuzzleCompleted, body);
	}

	public static void SendWisdomTemplePuzzleRewardObjectInteractionFinished(ClientPeer peer, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBWisdomTemplePuzzleRewardObjectInteractionFinishedEventBody body = new SEBWisdomTemplePuzzleRewardObjectInteractionFinishedEventBody();
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.WisdomTemplePuzzleRewardObjectInteractionFinished, body);
	}

	public static void SendWisdomTemplePuzzleRewardObjectInteractionCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.WisdomTemplePuzzleRewardObjectInteractionCancel, null);
	}

	public static void SendWisdomTempleQuizFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.WisdomTempleQuizFail, null);
	}

	public static void SendWisdomTempleBossMonsterCreated(ClientPeer peer, PDWisdomTempleBossMonsterInstance monsterInst)
	{
		SEBWisdomTempleBossMonsterCreatedEventBody body = new SEBWisdomTempleBossMonsterCreatedEventBody();
		body.monsterInst = monsterInst;
		Send(peer, ServerEventName.WisdomTempleBossMonsterCreated, body);
	}

	public static void SendWisdomTempleBossMonsterKill(ClientPeer peer, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBWisdomTempleBossMonsterKillEventBody body = new SEBWisdomTempleBossMonsterKillEventBody();
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.WisdomTempleBossMonsterKill, body);
	}

	public static void SendWisdomTempleClear(ClientPeer peer)
	{
		Send(peer, ServerEventName.WisdomTempleClear, null);
	}

	public static void SendWisdomTempleFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.WisdomTempleFail, null);
	}

	public static void SendWisdomTempleBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBWisdomTempleBanishedEventBody body = new SEBWisdomTempleBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.WisdomTempleBanished, body);
	}

	public static void SendOpen7DayEventProgressCountUpdated(ClientPeer peer, PDHeroOpen7DayEventProgressCount progressCount)
	{
		SEBOpen7DayEventProgressCountUpdatedEventBody body = new SEBOpen7DayEventProgressCountUpdatedEventBody();
		body.progressCount = progressCount;
		Send(peer, ServerEventName.Open7DayEventProgressCountUpdated, body);
	}

	public static void SendRetrievalProgressCountUpdated(ClientPeer peer, PDHeroRetrievalProgressCount progressCount)
	{
		SEBRetrievalProgressCountUpdatedEventBody body = new SEBRetrievalProgressCountUpdatedEventBody();
		body.progressCount = progressCount;
		Send(peer, ServerEventName.RetrievalProgressCountUpdated, body);
	}

	public static void SendContinentExitForRuinsReclaimEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForRuinsReclaimEnter, null);
	}

	public static void SendRuinsReclaimMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBRuinsReclaimMatchingRoomPartyEnterEventBody body = new SEBRuinsReclaimMatchingRoomPartyEnterEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.RuinsReclaimMatchingRoomPartyEnter, body);
	}

	public static void SendRuinsReclaimMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBRuinsReclaimMatchingStatusChangedEventBody body = new SEBRuinsReclaimMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.RuinsReclaimMatchingStatusChanged, body);
	}

	public static void SendRuinsReclaimMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBRuinsReclaimMatchingRoomBanishedEventBody body = new SEBRuinsReclaimMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.RuinsReclaimMatchingRoomBanished, body);
	}

	public static void SendRuinsReclaimStepStart(IEnumerable<ClientPeer> peers, int nStepNo, PDRuinsReclaimRewardObjectInstance[] objectInsts)
	{
		SEBRuinsReclaimStepStartEventBody body = new SEBRuinsReclaimStepStartEventBody();
		body.stepNo = nStepNo;
		body.objectInsts = objectInsts;
		Send(peers, ServerEventName.RuinsReclaimStepStart, body);
	}

	public static void SendRuinsReclaimStepCompleted(ClientPeer peer, PDItemBooty[] booties, PDInventorySlot[] changedInventorySlot)
	{
		SEBRuinsReclaimStepCompletedEventBody body = new SEBRuinsReclaimStepCompletedEventBody();
		body.booties = booties;
		body.changedInventorySlots = changedInventorySlot;
		Send(peer, ServerEventName.RuinsReclaimStepCompleted, body);
	}

	public static void SendRuinsReclaimRewardObjectInteractionCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.RuinsReclaimRewardObjectInteractionCancel, null);
	}

	public static void SendRuinsReclaimRewardObjectInteractionFinished(ClientPeer peer, long lnGold, long lnMaxGold, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBRuinsReclaimRewardObjectInteractionFinishedEventBody body = new SEBRuinsReclaimRewardObjectInteractionFinishedEventBody();
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.RuinsReclaimRewardObjectInteractionFinished, body);
	}

	public static void SendHeroRuinsReclaimRewardObjectInteractionStart(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroRuinsReclaimRewardObjectInteractionStartEventBody body = new SEBHeroRuinsReclaimRewardObjectInteractionStartEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroRuinsReclaimRewardObjectInteractionStart, body);
	}

	public static void SendHeroRuinsReclaimRewardObjectInteractionCancel(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroRuinsReclaimRewardObjectInteractionCancelEventBody body = new SEBHeroRuinsReclaimRewardObjectInteractionCancelEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroRuinsReclaimRewardObjectInteractionCancel, body);
	}

	public static void SendHeroRuinsReclaimRewardObjectInteractionFinished(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroRuinsReclaimRewardObjectInteractionFinishedEventBody body = new SEBHeroRuinsReclaimRewardObjectInteractionFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroRuinsReclaimRewardObjectInteractionFinished, body);
	}

	public static void SendRuinsReclaimWaveStart(IEnumerable<ClientPeer> peers, int nWaveNo, PDRuinsReclaimMonsterInstance[] monsterInsts)
	{
		SEBRuinsReclaimWaveStartEventBody body = new SEBRuinsReclaimWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.RuinsReclaimWaveStart, body);
	}

	public static void SendRuinsReclaimWaveComplete(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.RuinsReclaimWaveCompleted, null);
	}

	public static void SendRuinsReclaimStepWaveSkillCast(IEnumerable<ClientPeer> peers, PDVector3 targetPosition, PDRuinsReclaimMonsterTransformationCancelObjectInstance[] createdObjectInsts)
	{
		SEBRuinsReclaimStepWaveSkillCastEventBody body = new SEBRuinsReclaimStepWaveSkillCastEventBody();
		body.targetPosition = targetPosition;
		body.createdObjectInsts = createdObjectInsts;
		Send(peers, ServerEventName.RuinsReclaimStepWaveSkillCast, body);
	}

	public static void SendRuinsReclaimMonsterTransformationStart(ClientPeer peer, int nTransformationMonsterId, long[] removedAbnormalStateEffects)
	{
		SEBRuinsReclaimMonsterTransformationStartEventBody body = new SEBRuinsReclaimMonsterTransformationStartEventBody();
		body.transformationMonsterId = nTransformationMonsterId;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.RuinsReclaimMonsterTransformationStart, body);
	}

	public static void SendRuinsReclaimMonsterTransformationFinished(ClientPeer peer)
	{
		Send(peer, ServerEventName.RuinsReclaimMonsterTransformationFinished, null);
	}

	public static void SendHeroRuinsReclaimMonsterTransformationStart(IEnumerable<ClientPeer> peers, Guid heroId, int nTransformationMonsterId, long[] removedAbnormalStateEffects)
	{
		SEBHeroRuinsReclaimMonsterTransformationStartEventBody body = new SEBHeroRuinsReclaimMonsterTransformationStartEventBody();
		body.heroId = (Guid)heroId;
		body.transformationMonsterId = nTransformationMonsterId;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroRuinsReclaimMonsterTransformationStart, body);
	}

	public static void SendHeroRuinsReclaimMonsterTransformationFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroRuinsReclaimMonsterTransformationFinishedEventBody body = new SEBHeroRuinsReclaimMonsterTransformationFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroRuinsReclaimMonsterTransformationFinished, body);
	}

	public static void SendRuinsReclaimMonsterTransformationCancelObjectLifetimeEnded(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBRuinsReclaimMonsterTransformationCancelObjectLifetimeEndedEventBody body = new SEBRuinsReclaimMonsterTransformationCancelObjectLifetimeEndedEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.RuinsReclaimMonsterTransformationCancelObjectLifetimeEnded, body);
	}

	public static void SendRuinsReclaimMonsterTransformationCancelObjectInteractionCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.RuinsReclaimMonsterTransformationCancelObjectInteractionCancel, null);
	}

	public static void SendRuinsReclaimMonsterTransformationCancelObjectInteractionFinished(ClientPeer peer)
	{
		Send(peer, ServerEventName.RuinsReclaimMonsterTransformationCancelObjectInteractionFinished, null);
	}

	public static void SendHeroRuinsReclaimMonsterTransformationCancelObjectInteractionStart(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionStartEventBody body = new SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionStartEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroRuinsReclaimMonsterTransformationCancelObjectInteractionStart, body);
	}

	public static void SendHeroRuinsReclaimMonsterTransformationCancelObjectInteractionCancel(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionCancelEventBody body = new SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionCancelEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroRuinsReclaimMonsterTransformationCancelObjectInteractionCancel, body);
	}

	public static void SendHeroRuinsReclaimMonsterTransformationCancelObjectInteractionFinished(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionFinishedEventBody body = new SEBHeroRuinsReclaimMonsterTransformationCancelObjectInteractionFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroRuinsReclaimMonsterTransformationCancelObjectInteractionFinished, body);
	}

	public static void SendRuinsReclaimMonsterSummon(IEnumerable<ClientPeer> peers, PDRuinsReclaimSummonMonsterInstance[] monsterInsts)
	{
		SEBRuinsReclaimMonsterSummonEventBody body = new SEBRuinsReclaimMonsterSummonEventBody();
		body.monsterInst = monsterInsts;
		Send(peers, ServerEventName.RuinsReclaimMonsterSummon, body);
	}

	public static void SendRuinsReclaimTrapHit(IEnumerable<ClientPeer> peers, Guid heroId, bool bIsImmortal, int nHP, int nDamage, int nHPDamage, PDAbnormalStateEffectDamageAbsorbShield[] changedAbnormalStateEffectDamageAbsorbShields, long[] removedAbnormalStateEffects)
	{
		SEBRuinsReclaimTrapHitEventBody body = new SEBRuinsReclaimTrapHitEventBody();
		body.heroId = (Guid)heroId;
		body.isImmortal = bIsImmortal;
		body.hp = nHP;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.changedAbnormalStateEffectDamageAbsorbShields = changedAbnormalStateEffectDamageAbsorbShields;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.RuinsReclaimTrapHit, body);
	}

	public static void SendRuinsReclaimDebuffEffectStart(ClientPeer peer)
	{
		Send(peer, ServerEventName.RuinsReclaimDebuffEffectStart, null);
	}

	public static void SendRuinsReclaimDebuffEffectStop(ClientPeer peer)
	{
		Send(peer, ServerEventName.RuinsReclaimDebuffEffectStop, null);
	}

	public static void SendHeroRuinsReclaimPortalEnter(IEnumerable<ClientPeer> peers, Guid heroId, PDVector3 position, float fRotationY)
	{
		SEBHeroRuinsReclaimPortalEnterEventBody body = new SEBHeroRuinsReclaimPortalEnterEventBody();
		body.heroId = (Guid)heroId;
		body.position = position;
		body.rotationY = fRotationY;
		Send(peers, ServerEventName.HeroRuinsReclaimPortalEnter, body);
	}

	public static void SendRuinsReclaimClear(ClientPeer peer, Guid monsterTerminatorHeroId, string sMonsterTerminatorHeroName, PDItemBooty monsterTerminatorBooty, Guid ultimateAttackKingHeroId, string sUltimateAttackKingHeroName, PDItemBooty ultimateAttackKingBooty, Guid partyVolunteerHeroId, string sPartyVolunteerHeroName, PDItemBooty partyVolunteerBooty, PDItemBooty randomBooty, PDItemBooty[] booties, PDInventorySlot[] changedInventorySlots)
	{
		SEBRuinsReclaimClearEventBody body = new SEBRuinsReclaimClearEventBody();
		body.monsterTerminatorHeroId = (Guid)monsterTerminatorHeroId;
		body.monsterTerminatorHeroName = sMonsterTerminatorHeroName;
		body.monsterTerminatorBooty = monsterTerminatorBooty;
		body.ultimateAttackKingHeroId = (Guid)ultimateAttackKingHeroId;
		body.ultimateAttackKingHeroName = sUltimateAttackKingHeroName;
		body.ultimateAttackKingBooty = ultimateAttackKingBooty;
		body.partyVolunteerHeroId = (Guid)partyVolunteerHeroId;
		body.partyVolunteerHeroName = sPartyVolunteerHeroName;
		body.partyVolunteerBooty = partyVolunteerBooty;
		body.randomBooty = randomBooty;
		body.booties = booties;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.RuinsReclaimClear, body);
	}

	public static void SendRuinsReclaimFail(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.RuinsReclaimFail, null);
	}

	public static void SendRuinsReclaimBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBRuinsReclaimBanishedEventBody body = new SEBRuinsReclaimBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.RuinsReclaimBanished, body);
	}

	public static void SendTrueHeroQuestTaunted(IEnumerable<ClientPeer> peers, int nNationId, Guid heroId, string sHeroName, int nContinentId, PDVector3 position)
	{
		SEBTrueHeroQuestTauntedEventBody body = new SEBTrueHeroQuestTauntedEventBody();
		body.nationId = nNationId;
		body.heroId = (Guid)heroId;
		body.heroName = sHeroName;
		body.continentId = nContinentId;
		body.position = position;
		Send(peers, ServerEventName.TrueHeroQuestTaunted, body);
	}

	public static void SendHeroTrueHeroQuestStepInteractionStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroTrueHeroQuestStepInteractionStartedEventBody body = new SEBHeroTrueHeroQuestStepInteractionStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroTrueHeroQuestStepInteractionStarted, body);
	}

	public static void SendHeroTrueHeroQuestStepInteractionCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroTrueHeroQuestStepInteractionCanceledEventBody body = new SEBHeroTrueHeroQuestStepInteractionCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroTrueHeroQuestStepInteractionCanceled, body);
	}

	public static void SendHeroTrueHeroQuestStepInteractionFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroTrueHeroQuestStepInteractionFinishedEventBody body = new SEBHeroTrueHeroQuestStepInteractionFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroTrueHeroQuestStepInteractionFinished, body);
	}

	public static void SendTrueHeroQuestStepInteractionFinished(ClientPeer peer)
	{
		Send(peer, ServerEventName.TrueHeroQuestStepInteractionFinished, null);
	}

	public static void SendTrueHeroQuestStepCompledted(ClientPeer peer, int nNextStepNo, PDInventorySlot[] changedInventorySlots)
	{
		SEBTrueHeroQuestStepCompletedEventBody body = new SEBTrueHeroQuestStepCompletedEventBody();
		body.nextStepNo = nNextStepNo;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.TrueHeroQuestStepCompleted, body);
	}

	public static void SendTrueHeroQuestStepInteractionCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.TrueHeroQuestStepInteractionCanceled, null);
	}

	public static void SendTrueHeroQuestStepWaitingCanceled(ClientPeer peer)
	{
		Send(peer, ServerEventName.TrueHeroQuestStepWaitingCanceled, null);
	}

	public static void SendContinentExitForInfiniteWarEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForInfiniteWarEnter, null);
	}

	public static void SendInfiniteWarMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBInfiniteWarMatchingStatusChangedEventBody body = new SEBInfiniteWarMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.InfiniteWarMatchingStatusChanged, body);
	}

	public static void SendInfiniteWarMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBInfiniteWarMatchingRoomBanishedEventBody body = new SEBInfiniteWarMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.InfiniteWarMatchingRoomBanished, body);
	}

	public static void SendInfiniteWarStart(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.InfiniteWarStart, null);
	}

	public static void SendInfiniteWarMonsterSpawn(IEnumerable<ClientPeer> peers, PDInfiniteWarMonsterInstance[] monsterInsts)
	{
		SEBInfiniteWarMonsterSpawnEventBody body = new SEBInfiniteWarMonsterSpawnEventBody();
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.InfiniteWarMonsterSpawn, body);
	}

	public static void SendInfiniteWarBuffBoxCreated(IEnumerable<ClientPeer> peers, PDInfiniteWarBuffBoxInstance[] buffBoxInsts)
	{
		SEBInfiniteWarBuffBoxCreatedEventBody body = new SEBInfiniteWarBuffBoxCreatedEventBody();
		body.buffBoxInsts = buffBoxInsts;
		Send(peers, ServerEventName.InfiniteWarBuffBoxCreated, body);
	}

	public static void SendInfiniteWarBuffBoxLifetimeEnded(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBInfiniteWarBuffBoxLifetimeEndedEventBody body = new SEBInfiniteWarBuffBoxLifetimeEndedEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.InfiniteWarBuffBoxLifetimeEnded, body);
	}

	public static void SendHeroInfiniteWarBuffBoxAcquisition(IEnumerable<ClientPeer> peers, Guid heroId, int nHP, long lnInstanceId)
	{
		SEBHeroInfiniteWarBuffBoxAcquisitionEventBody body = new SEBHeroInfiniteWarBuffBoxAcquisitionEventBody();
		body.heroId = (Guid)heroId;
		body.hp = nHP;
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.HeroInfiniteWarBuffBoxAcquisition, body);
	}

	public static void SendInfiniteWarBuffFinished(ClientPeer peer)
	{
		Send(peer, ServerEventName.InfiniteWarBuffFinished, null);
	}

	public static void SendInfiniteWarPointAcquisition(ClientPeer peer, int nPoint, long lnPointUpdatedTimeTicks)
	{
		SEBInfiniteWarPointAcquisitionEventBody body = new SEBInfiniteWarPointAcquisitionEventBody();
		body.point = nPoint;
		body.pointUpdatedTimeTicks = lnPointUpdatedTimeTicks;
		Send(peer, ServerEventName.InfiniteWarPointAcquisition, body);
	}

	public static void SendHeroInfiniteWarPointAcquisition(IEnumerable<ClientPeer> peers, Guid heroId, int nPoint, long lnPointUpdatedTimeTicks)
	{
		SEBHeroInfiniteWarPointAcquisitionEventBody body = new SEBHeroInfiniteWarPointAcquisitionEventBody();
		body.heroId = (Guid)heroId;
		body.point = nPoint;
		body.pointUpdatedTimeTicks = lnPointUpdatedTimeTicks;
		Send(peers, ServerEventName.HeroInfiniteWarPointAcquisition, body);
	}

	public static void SendInfiniteWarClear(ClientPeer peer, PDInfiniteWarRanking[] rankings, PDItemBooty[] booties, PDItemBooty[] rankingBooties, PDInventorySlot[] changedInventorySlots)
	{
		SEBInfiniteWarClearEventBody body = new SEBInfiniteWarClearEventBody();
		body.rankings = rankings;
		body.booties = booties;
		body.rankingBooties = rankingBooties;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.InfiniteWarClear, body);
	}

	public static void SendInfiniteWarBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBInfiniteWarBanishedEventBody body = new SEBInfiniteWarBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.InfiniteWarBanished, body);
	}

	public static void SendFieldBossEventStarted(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.FieldBossEventStarted, null);
	}

	public static void SendFieldBossEventEnded(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.FieldBossEventEnded, null);
	}

	public static void SendFieldBossDead(IEnumerable<ClientPeer> peers, int nFieldBossId)
	{
		SEBFieldBossDeadEventBody body = new SEBFieldBossDeadEventBody();
		body.fieldBossId = nFieldBossId;
		Send(peers, ServerEventName.FieldBossDead, body);
	}

	public static void SendFieldBossRewardLooted(ClientPeer peer, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBFieldBossRewardLootedEventBody body = new SEBFieldBossRewardLootedEventBody();
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.FieldBossRewardLooted, body);
	}

	public static void SendContinentExitForFearAltarEnter(IEnumerable<ClientPeer> peers, int nStageId)
	{
		SEBContinentExitForFearAltarEnterEventBody body = new SEBContinentExitForFearAltarEnterEventBody();
		body.stageId = nStageId;
		Send(peers, ServerEventName.ContinentExitForFearAltarEnter, body);
	}

	public static void SendFearAltarMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBFearAltarMatchingRoomPartyEnterEventBody body = new SEBFearAltarMatchingRoomPartyEnterEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.FearAltarMatchingRoomPartyEnter, body);
	}

	public static void SendFearAltarMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBFearAltarMatchingRoomBanishedEventBody body = new SEBFearAltarMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.FearAltarMatchingRoomBanished, body);
	}

	public static void SendFearAltarMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBFearAltarMatchingStatusChangedEventBody body = new SEBFearAltarMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.FearAltarMatchingStatusChanged, body);
	}

	public static void SendFearAltarWaveStart(IEnumerable<ClientPeer> peers, int nWaveNo, PDFearAltarMonsterInstance[] monsterInsts, PDFearAltarHalidomMonsterInstance halidomMonsterInst)
	{
		SEBFearAltarWaveStartEventBody body = new SEBFearAltarWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		body.halidomMonsterInst = halidomMonsterInst;
		Send(peers, ServerEventName.FearAltarWaveStart, body);
	}

	public static void SendFearAltarHalidomAcquisition(ClientPeer peer, DateTime weekDateTime, int nHalidomId)
	{
		SEBFearAltarHalidomAcquisitionEventBody body = new SEBFearAltarHalidomAcquisitionEventBody();
		body.weekDateTime = (DateTime)weekDateTime;
		body.halidomId = nHalidomId;
		Send(peer, ServerEventName.FearAltarHalidomAcquisition, body);
	}

	public static void SendFearAltarClear(ClientPeer peer, PDFearAltarHero[] clearedHeroes, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP)
	{
		SEBFearAltarClearEventBody body = new SEBFearAltarClearEventBody();
		body.clearedHeroes = clearedHeroes;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.FearAltarClear, body);
	}

	public static void SendFearAltarFail(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.FearAltarFail, null);
	}

	public static void SendFearAltarBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBFearAltarBanishedEventBody body = new SEBFearAltarBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.FearAltarBanished, body);
	}

	public static void SendSubQuestsAccepted(ClientPeer peer, PDHeroSubQuest[] subQuests)
	{
		SEBSubQuestsAcceptedEventBody body = new SEBSubQuestsAcceptedEventBody();
		body.subQuests = subQuests;
		Send(peer, ServerEventName.SubQuestsAccepted, body);
	}

	public static void SendSubQuestProgressCountsUpdated(ClientPeer peer, PDHeroSubQuestProgressCount[] progressCounts)
	{
		SEBSubQuestProgressCountsUpdatedEventBody body = new SEBSubQuestProgressCountsUpdatedEventBody();
		body.progressCounts = progressCounts;
		Send(peer, ServerEventName.SubQuestProgressCountsUpdated, body);
	}

	public static void SendContinentExitForWarMemoryEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForWarMemoryEnter, null);
	}

	public static void SendWarMemoryMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBWarMemoryMatchingRoomPartyEnterEventBody body = new SEBWarMemoryMatchingRoomPartyEnterEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.WarMemoryMatchingRoomPartyEnter, body);
	}

	public static void SendWarMemoryMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBWarMemoryMatchingRoomBanishedEventBody body = new SEBWarMemoryMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.WarMemoryMatchingRoomBanished, body);
	}

	public static void SendWarMemoryMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBWarMemoryMatchingStatusChangedEventBody body = new SEBWarMemoryMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.WarMemoryMatchingStatusChanged, body);
	}

	public static void SendWarMemoryWaveStart(IEnumerable<ClientPeer> peers, int nWaveNo, PDWarMemoryMonsterInstance[] monsterInsts, PDWarMemoryTransformationObjectInstance[] objectInsts)
	{
		SEBWarMemoryWaveStartEventBody body = new SEBWarMemoryWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		body.objectInsts = objectInsts;
		Send(peers, ServerEventName.WarMemoryWaveStart, body);
	}

	public static void SendWarMemoryWaveCompleted(IEnumerable<ClientPeer> peers, PDWarMemoryPoint[] points)
	{
		SEBWarMemoryWaveCompletedEventBody body = new SEBWarMemoryWaveCompletedEventBody();
		body.points = points;
		Send(peers, ServerEventName.WarMemoryWaveCompleted, body);
	}

	public static void SendWarMemoryTransformationObjectLifetimeEnded(IEnumerable<ClientPeer> peers, long lnInstanceId)
	{
		SEBWarMemoryTransformationObjectLifetimeEndedEventBody body = new SEBWarMemoryTransformationObjectLifetimeEndedEventBody();
		body.instanceId = lnInstanceId;
		Send(peers, ServerEventName.WarMemoryTransformationObjectLifetimeEnded, body);
	}

	public static void SendWarMemoryTransformationObjectInteractionCancel(ClientPeer peer)
	{
		Send(peer, ServerEventName.WarMemoryTransformationObjectInteractionCancel, null);
	}

	public static void SendWarMemoryTransformationObjectInteractionFinished(ClientPeer peer, int nPoint, long lnPointUpdatedTimeTicks, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBWarMemoryTransformationObjectInteractionFinishedEventBody body = new SEBWarMemoryTransformationObjectInteractionFinishedEventBody();
		body.point = nPoint;
		body.pointUpdatedTimeTicks = lnPointUpdatedTimeTicks;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.WarMemoryTransformationObjectInteractionFinished, body);
	}

	public static void SendHeroWarMemoryTransformationObjectInteractionStart(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroWarMemoryTransformationObjectInteractionStartEventBody body = new SEBHeroWarMemoryTransformationObjectInteractionStartEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroWarMemoryTransformationObjectInteractionStart, body);
	}

	public static void SendHeroWarMemoryTransformationObjectInteractionCancel(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId)
	{
		SEBHeroWarMemoryTransformationObjectInteractionCancelEventBody body = new SEBHeroWarMemoryTransformationObjectInteractionCancelEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		Send(peers, ServerEventName.HeroWarMemoryTransformationObjectInteractionCancel, body);
	}

	public static void SendHeroWarMemoryTransformationObjectInteractionFinished(IEnumerable<ClientPeer> peers, Guid heroId, long lnObjectInstanceId, int nPoint, long lnPointUpdatedTimeTicks, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBHeroWarMemoryTransformationObjectInteractionFinishedEventBody body = new SEBHeroWarMemoryTransformationObjectInteractionFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.objectInstanceId = lnObjectInstanceId;
		body.point = nPoint;
		body.pointUpdatedTimeTicks = lnPointUpdatedTimeTicks;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroWarMemoryTransformationObjectInteractionFinished, body);
	}

	public static void SendWarMemoryMonsterTransformationCancel(ClientPeer peer, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBWarMemoryMonsterTransformationCancelEventBody body = new SEBWarMemoryMonsterTransformationCancelEventBody();
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.WarMemoryMonsterTransformationCancel, body);
	}

	public static void SendWarMemoryMonsterTransformationFinished(ClientPeer peer, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBWarMemoryMonsterTransformationFinishedEventBody body = new SEBWarMemoryMonsterTransformationFinishedEventBody();
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peer, ServerEventName.WarMemoryMonsterTransformationFinished, body);
	}

	public static void SendHeroWarMemoryMonsterTransformationCancel(IEnumerable<ClientPeer> peers, Guid heroId, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBHeroWarMemoryMonsterTransformationCancelEventBody body = new SEBHeroWarMemoryMonsterTransformationCancelEventBody();
		body.heroId = (Guid)heroId;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroWarMemoryMonsterTransformationCancel, body);
	}

	public static void SendHeroWarMemoryMonsterTransformationFinished(IEnumerable<ClientPeer> peers, Guid heroId, int nMaxHP, int nHP, long[] removedAbnormalStateEffects)
	{
		SEBHeroWarMemoryMonsterTransformationFinishedEventBody body = new SEBHeroWarMemoryMonsterTransformationFinishedEventBody();
		body.heroId = (Guid)heroId;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroWarMemoryMonsterTransformationFinished, body);
	}

	public static void SendWarMemoryMonsterSummon(IEnumerable<ClientPeer> peers, PDWarMemorySummonMonsterInstance[] monsterInsts)
	{
		SEBWarMemoryMonsterSummonEventBody body = new SEBWarMemoryMonsterSummonEventBody();
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.WarMemoryMonsterSummon, body);
	}

	public static void SendWarMemoryPointAcquisition(ClientPeer peer, int nPoint, long lnPointUpdatedTimeTicks)
	{
		SEBWarMemoryPointAcquisitionEventBody body = new SEBWarMemoryPointAcquisitionEventBody();
		body.point = nPoint;
		body.pointUpdatedTimeTicks = lnPointUpdatedTimeTicks;
		Send(peer, ServerEventName.WarMemoryPointAcquisition, body);
	}

	public static void SendHeroWarMemoryPointAcquisition(IEnumerable<ClientPeer> peers, Guid heroId, int nPoint, long lnPointUpdatedTimeTicks)
	{
		SEBHeroWarMemoryPointAcquisitionEventBody body = new SEBHeroWarMemoryPointAcquisitionEventBody();
		body.heroId = (Guid)heroId;
		body.point = nPoint;
		body.pointUpdatedTimeTicks = lnPointUpdatedTimeTicks;
		Send(peers, ServerEventName.HeroWarMemoryPointAcquisition, body);
	}

	public static void SendHeroWarMemoryTransformationMonsterSkillCast(IEnumerable<ClientPeer> peers, Guid heroId, int nSkillId, PDVector3 skillTargetPosition)
	{
		SEBHeroWarMemoryTransformationMonsterSkillCastEventBody body = new SEBHeroWarMemoryTransformationMonsterSkillCastEventBody();
		body.heroId = (Guid)heroId;
		body.skillId = nSkillId;
		body.skillTargetPosition = skillTargetPosition;
		Send(peers, ServerEventName.HeroWarMemoryTransformationMonsterSkillCast, body);
	}

	public static void SendWarMemoryClear(ClientPeer peer, PDWarMemoryRanking[] rankings, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, PDItemBooty[] rankingBooties, PDInventorySlot[] changedInventorySlots)
	{
		SEBWarMemoryClearEventBody body = new SEBWarMemoryClearEventBody();
		body.rankings = rankings;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHp = nMaxHP;
		body.hp = nHP;
		body.rankingBooties = rankingBooties;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.WarMemoryClear, body);
	}

	public static void SendWarMemoryFail(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.WarMemoryFail, null);
	}

	public static void SendWarMemoryBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBWarMemoryBanishedEventBody body = new SEBWarMemoryBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.WarMemoryBanished, body);
	}

	public static void SendOrdealQuestAccepted(ClientPeer peer, PDHeroOrdealQuest quest)
	{
		SEBOrdealQuestAcceptedEventBody body = new SEBOrdealQuestAcceptedEventBody();
		body.quest = quest;
		Send(peer, ServerEventName.OrdealQuestAccepted, body);
	}

	public static void SendOrdealQuestSlotProgressCountsUpdated(ClientPeer peer, PDHeroOrdealQuestSlotProgressCount[] progressCounts)
	{
		SEBOrdealQuestSlotProgressCountsUpdatedEventBody body = new SEBOrdealQuestSlotProgressCountsUpdatedEventBody();
		body.progressCounts = progressCounts;
		Send(peer, ServerEventName.OrdealQuestSlotProgressCountsUpdated, body);
	}

	public static void SendOsirisRoomWaveStart(ClientPeer peer, int nWaveNo, int nMonsterCount)
	{
		SEBOsirisRoomWaveStartEventBody body = new SEBOsirisRoomWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterCount = nMonsterCount;
		Send(peer, ServerEventName.OsirisRoomWaveStart, body);
	}

	public static void SendOsirisRoomMonsterSpawn(ClientPeer peer, PDOsirisRoomMonsterInstance monsterInst)
	{
		SEBOsirisRoomMonsterSpawnEventBody body = new SEBOsirisRoomMonsterSpawnEventBody();
		body.monsterInst = monsterInst;
		Send(peer, ServerEventName.OsirisRoomMonsterSpawn, body);
	}

	public static void SendOsirisRoomRewardGoldAcquisition(ClientPeer peer, long lnGold, long lnMaxGold)
	{
		SEBOsirisRoomRewardGoldAcquisitionEventBody body = new SEBOsirisRoomRewardGoldAcquisitionEventBody();
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		Send(peer, ServerEventName.OsirisRoomRewardGoldAcquisition, body);
	}

	public static void SendOsirisRoomMoneyBuffFinished(ClientPeer peer, int nMaxHP, int nHP)
	{
		SEBOsirisRoomMoneyBuffFinishedEventBody body = new SEBOsirisRoomMoneyBuffFinishedEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.OsirisRoomMoneyBuffFinished, body);
	}

	public static void SendOsirisRoomMoneyBuffCancel(ClientPeer peer, int nMaxHP, int nHP)
	{
		SEBOsirisRoomMoneyBuffCancelEventBody body = new SEBOsirisRoomMoneyBuffCancelEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.OsirisRoomMoneyBuffCancel, body);
	}

	public static void SendOsirisRoomClear(ClientPeer peer)
	{
		Send(peer, ServerEventName.OsirisRoomClear, null);
	}

	public static void SendOsirisRoomFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.OsirisRoomFail, null);
	}

	public static void SendOsirisRoomBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBOsirisRoomBanishedEventBody body = new SEBOsirisRoomBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.OsirisRoomBanished, body);
	}

	public static void SendFriendApplicationReceived(ClientPeer peer, PDFriendApplication app)
	{
		SEBFriendApplicationReceivedEventBody body = new SEBFriendApplicationReceivedEventBody();
		body.app = app;
		Send(peer, ServerEventName.FriendApplicationReceived, body);
	}

	public static void SendFriendApplicationAccepted(ClientPeer peer, long lnApplicationNo, PDFriend newFriend)
	{
		SEBFriendApplicationAcceptedEventBody body = new SEBFriendApplicationAcceptedEventBody();
		body.applicationNo = lnApplicationNo;
		body.newFriend = newFriend;
		Send(peer, ServerEventName.FriendApplicationAccepted, body);
	}

	public static void SendFriendApplicationCanceled(ClientPeer peer, long lnApplicationNo)
	{
		SEBFriendApplicationCanceledEventBody body = new SEBFriendApplicationCanceledEventBody();
		body.applicationNo = lnApplicationNo;
		Send(peer, ServerEventName.FriendApplicationCanceled, body);
	}

	public static void SendFriendApplicationRefused(ClientPeer peer, long lnApplicationNo)
	{
		SEBFriendApplicationRefusedEventBody body = new SEBFriendApplicationRefusedEventBody();
		body.applicationNo = lnApplicationNo;
		Send(peer, ServerEventName.FriendApplicationRefused, body);
	}

	public static void SendTempFriendAdded(ClientPeer peer, PDTempFriend tempFriend, Guid removedTempFriendId)
	{
		SEBTempFriendAddedEventBody body = new SEBTempFriendAddedEventBody();
		body.tempFriend = tempFriend;
		body.removedTempFriendId = (Guid)removedTempFriendId;
		Send(peer, ServerEventName.TempFriendAdded, body);
	}

	public static void SendDeadRecordAdded(ClientPeer peer, PDDeadRecord record, Guid removedRecordId)
	{
		SEBDeadRecordAddedEventBody body = new SEBDeadRecordAddedEventBody();
		body.record = record;
		body.removedRecordId = (Guid)removedRecordId;
		Send(peer, ServerEventName.DeadRecordAdded, body);
	}

	public static void SendBiographyQuestProgressCountsUpdated(ClientPeer peer, PDHeroBiographyQuestProgressCount[] progressCounts)
	{
		SEBBiographyQuestProgressCountsUpdatedEventBody body = new SEBBiographyQuestProgressCountsUpdatedEventBody();
		body.progressCounts = progressCounts;
		Send(peer, ServerEventName.BiographyQuestProgressCountsUpdated, body);
	}

	public static void SendBiographyQuestDungeonWaveStart(ClientPeer peer, int nWaveNo, PDBiographyQuestDungeonMonsterInstance[] monsterInsts)
	{
		SEBBiographyQuestDungeonWaveStartEventBody body = new SEBBiographyQuestDungeonWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		Send(peer, ServerEventName.BiographyQuestDungeonWaveStart, body);
	}

	public static void SendBiographyQuestDungeonWaveCompleted(ClientPeer peer, int nWaveNo)
	{
		SEBBiographyQuestDungeonWaveCompletedEventBody body = new SEBBiographyQuestDungeonWaveCompletedEventBody();
		body.waveNo = nWaveNo;
		Send(peer, ServerEventName.BiographyQuestDungeonWaveCompleted, body);
	}

	public static void SendBiographyQuestDungeonClear(ClientPeer peer)
	{
		Send(peer, ServerEventName.BiographyQuestDungeonClear, null);
	}

	public static void SendBiographyQuestDungeonFail(ClientPeer peer)
	{
		Send(peer, ServerEventName.BiographyQuestDungeonFail, null);
	}

	public static void SendBiographyQuestDungeonBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHp)
	{
		SEBBiographyQuestDungeonBanishedEventBody body = new SEBBiographyQuestDungeonBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHp;
		Send(peer, ServerEventName.BiographyQuestDungeonBanished, body);
	}

	public static void SendBlessingQuestStarted(ClientPeer peer, PDHeroBlessingQuest quest)
	{
		SEBBlessingQuestStartedEventBody body = new SEBBlessingQuestStartedEventBody();
		body.quest = quest;
		Send(peer, ServerEventName.BlessingQuestStarted, body);
	}

	public static void SendBlessingReceived(ClientPeer peer, PDHeroBlessing blessing)
	{
		SEBBlessingReceivedEventBody body = new SEBBlessingReceivedEventBody();
		body.blessing = blessing;
		Send(peer, ServerEventName.BlessingReceived, body);
	}

	public static void SendBlessingThanksMessageReceived(ClientPeer peer, Guid senderId, string sSenderName)
	{
		SEBBlessingThanksMessageReceivedEventBody body = new SEBBlessingThanksMessageReceivedEventBody();
		body.senderId = (Guid)senderId;
		body.senderName = sSenderName;
		Send(peer, ServerEventName.BlessingThanksMessageReceived, body);
	}

	public static void SendOwnerProspectQuestCompleted(ClientPeer peer, Guid instanceId)
	{
		SEBOwnerProspectQuestCompletedEventBody body = new SEBOwnerProspectQuestCompletedEventBody();
		body.instanceId = (Guid)instanceId;
		Send(peer, ServerEventName.OwnerProspectQuestCompleted, body);
	}

	public static void SendOwnerProspectQuestFailed(ClientPeer peer, Guid instanceId)
	{
		SEBOwnerProspectQuestFailedEventBody body = new SEBOwnerProspectQuestFailedEventBody();
		body.instanceId = (Guid)instanceId;
		Send(peer, ServerEventName.OwnerProspectQuestFailed, body);
	}

	public static void SendOwnerProspectQuestTargetLevelUpdated(ClientPeer peer, Guid instanceId, int nTargetLevel)
	{
		SEBOwnerProspectQuestTargetLevelUpdatedEventBody body = new SEBOwnerProspectQuestTargetLevelUpdatedEventBody();
		body.instanceId = (Guid)instanceId;
		body.targetLevel = nTargetLevel;
		Send(peer, ServerEventName.OwnerProspectQuestTargetLevelUpdated, body);
	}

	public static void SendTargetProspectQuestStarted(ClientPeer peer, PDHeroProspectQuest quest)
	{
		SEBTargetProspectQuestStartedEventBody body = new SEBTargetProspectQuestStartedEventBody();
		body.quest = quest;
		Send(peer, ServerEventName.TargetProspectQuestStarted, body);
	}

	public static void SendTargetProspectQuestCompleted(ClientPeer peer, Guid instanceId)
	{
		SEBTargetProspectQuestCompletedEventBody body = new SEBTargetProspectQuestCompletedEventBody();
		body.instanceId = (Guid)instanceId;
		Send(peer, ServerEventName.TargetProspectQuestCompleted, body);
	}

	public static void SendTargetProspectQuestFailed(ClientPeer peer, Guid instanceId)
	{
		SEBTargetProspectQuestFailedEventBody body = new SEBTargetProspectQuestFailedEventBody();
		body.instanceId = (Guid)instanceId;
		Send(peer, ServerEventName.TargetProspectQuestFailed, body);
	}

	public static void SendContinentExitForDragonNestEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForDragonNestEnter, null);
	}

	public static void SendDragonNestMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBDragonNestMatchingRoomPartyEnterEventBody body = new SEBDragonNestMatchingRoomPartyEnterEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.DragonNestMatchingRoomPartyEnter, body);
	}

	public static void SendDragonNestMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBDragonNestMatchingRoomBanishedEventBody body = new SEBDragonNestMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.DragonNestMatchingRoomBanished, body);
	}

	public static void SendDragonNestMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBDragonNestMatchingStatusChangedEventBody body = new SEBDragonNestMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.DragonNestMatchingStatusChanged, body);
	}

	public static void SendDragonNestStepStart(IEnumerable<ClientPeer> peers, int nStepNo, PDDragonNestMonsterInstance[] monsterInsts)
	{
		SEBDragonNestStepStartEventBody body = new SEBDragonNestStepStartEventBody();
		body.stepNo = nStepNo;
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.DragonNestStepStart, body);
	}

	public static void SendDragonNestStepCompleted(ClientPeer peer, PDItemBooty[] booties, PDInventorySlot[] changedInventorySlots)
	{
		SEBDragonNestStepCompletedEventBody body = new SEBDragonNestStepCompletedEventBody();
		body.booties = booties;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.DragonNestStepCompleted, body);
	}

	public static void SendHeroDragonNestTrapHit(IEnumerable<ClientPeer> peers, Guid heroId, bool bIsImmortal, int nHP, int nDamage, int nHPDamage, PDAbnormalStateEffectDamageAbsorbShield[] changedAbnormalStateEffectDamageAbsorbShields, long[] removedAbnormalStateEffects)
	{
		SEBHeroDragonNestTrapHitEventBody body = new SEBHeroDragonNestTrapHitEventBody();
		body.heroId = (Guid)heroId;
		body.isImmortal = bIsImmortal;
		body.hp = nHP;
		body.damage = nDamage;
		body.hpDamage = nHPDamage;
		body.changedAbnormalStateEffectDamageAbsorbShields = changedAbnormalStateEffectDamageAbsorbShields;
		body.removedAbnormalStateEffects = removedAbnormalStateEffects;
		Send(peers, ServerEventName.HeroDragonNestTrapHit, body);
	}

	public static void SendDragonNestTrapEffectFinished(ClientPeer peer)
	{
		Send(peer, ServerEventName.DragonNestTrapEffectFinished, null);
	}

	public static void SendHeroDragonNestTrapEffectFinished(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroDragonNestTrapEffectFinishedEventBody body = new SEBHeroDragonNestTrapEffectFinishedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroDragonNestTrapEffectFinished, body);
	}

	public static void SendDragonNestClear(IEnumerable<ClientPeer> peers, PDSimpleHero[] clearedHeroes)
	{
		SEBDragonNestClearEventBody body = new SEBDragonNestClearEventBody();
		body.clearedHeroes = clearedHeroes;
		Send(peers, ServerEventName.DragonNestClear, body);
	}

	public static void SendDragonNestFail(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.DragonNestFail, null);
	}

	public static void SendDragonNestBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBDragonNestBanishedEventBody body = new SEBDragonNestBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.DragonNestBanished, body);
	}

	public static void SendHeroCreatureParticipated(IEnumerable<ClientPeer> peers, Guid heroId, int nCreatureId)
	{
		SEBHeroCreatureParticipatedEventBody body = new SEBHeroCreatureParticipatedEventBody();
		body.heroId = (Guid)heroId;
		body.creatureId = nCreatureId;
		Send(peers, ServerEventName.HeroCreatureParticipated, body);
	}

	public static void SendHeroCreatureParticipationCanceled(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroCreatureParticipationCanceledEventBody body = new SEBHeroCreatureParticipationCanceledEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroCreatureParticipationCanceled, body);
	}

	public static void SendPresentReceived(ClientPeer peer, Guid senderId, string sSenderName, int nSenderNationId, int nPresentId, DateTime weekStartDate, int nWeeklyPresentPopularityPoint)
	{
		SEBPresentReceivedEventBody body = new SEBPresentReceivedEventBody();
		body.senderId = (Guid)senderId;
		body.senderName = sSenderName;
		body.senderNationId = nSenderNationId;
		body.presentId = nPresentId;
		body.weekStartDate = (DateTime)weekStartDate;
		body.weeklyPresentPopularityPoint = nWeeklyPresentPopularityPoint;
		Send(peer, ServerEventName.PresentReceived, body);
	}

	public static void SendHeroPresent(IEnumerable<ClientPeer> peers, Guid senderId, string sSenderName, int nSenderNationId, Guid targetId, string sTargetName, int nTargetNationId, int nPresentId)
	{
		SEBHeroPresentEventBody body = new SEBHeroPresentEventBody();
		body.senderId = (Guid)senderId;
		body.senderName = sSenderName;
		body.senderNationId = nSenderNationId;
		body.targetId = (Guid)targetId;
		body.targetName = sTargetName;
		body.targetNationId = nTargetNationId;
		body.presentId = nPresentId;
		Send(peers, ServerEventName.HeroPresent, body);
	}

	public static void SendPresentReplyReceived(ClientPeer peer, Guid senderId, string sSenderName, int nSenderNationId)
	{
		SEBPresentReplyReceivedEventBody body = new SEBPresentReplyReceivedEventBody();
		body.senderId = (Guid)senderId;
		body.senderName = sSenderName;
		body.senderNationId = nSenderNationId;
		Send(peer, ServerEventName.PresentReplyReceived, body);
	}

	public static void SendNationWeeklyPresentPopularityPointRankingUpdated(ClientPeer peer, int nRankingNo, int nRanking)
	{
		SEBNationWeeklyPresentPopularityPointRankingUpdatedEventBody body = new SEBNationWeeklyPresentPopularityPointRankingUpdatedEventBody();
		body.rankingNo = nRankingNo;
		body.ranking = nRanking;
		Send(peer, ServerEventName.NationWeeklyPresentPopularityPointRankingUpdated, body);
	}

	public static void SendNationWeeklyPresentContributionPointRankingUpdated(ClientPeer peer, int nRankingNo, int nRanking)
	{
		SEBNationWeeklyPresentContributionPointRankingUpdatedEventBody body = new SEBNationWeeklyPresentContributionPointRankingUpdatedEventBody();
		body.rankingNo = nRankingNo;
		body.ranking = nRanking;
		Send(peer, ServerEventName.NationWeeklyPresentContributionPointRankingUpdated, body);
	}

	public static void SendCostumePeriodExpired(ClientPeer peer, int nCostumeId)
	{
		SEBCostumePeriodExpiredEventBody body = new SEBCostumePeriodExpiredEventBody();
		body.costumeId = nCostumeId;
		Send(peer, ServerEventName.CostumePeriodExpired, body);
	}

	public static void SendHeroCostumeEquipped(IEnumerable<ClientPeer> peers, Guid heroId, int nCostumeId, int nCostumeEffectId)
	{
		SEBHeroCostumeEquippedEventBody body = new SEBHeroCostumeEquippedEventBody();
		body.heroId = (Guid)heroId;
		body.costumeId = nCostumeId;
		body.costumeEffectId = nCostumeEffectId;
		Send(peers, ServerEventName.HeroCostumeEquipped, body);
	}

	public static void SendHeroCostumeUnequipped(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroCostumeUnequippedEventBody body = new SEBHeroCostumeUnequippedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroCostumeUnequipped, body);
	}

	public static void SendHeroCostumeEffectApplied(IEnumerable<ClientPeer> peers, Guid heroId, int nCostumeEffectId)
	{
		SEBHeroCostumeEffectAppliedEventBody body = new SEBHeroCostumeEffectAppliedEventBody();
		body.heroId = (Guid)heroId;
		body.costumeEffectId = nCostumeEffectId;
		Send(peers, ServerEventName.HeroCostumeEffectApplied, body);
	}

	public static void SendCreatureFarmQuestProgressCountUpdated(ClientPeer peer, int nMissionNo, int nProgressCount)
	{
		SEBCreatureFarmQuestMissionProgressCountUpdatedEventBody body = new SEBCreatureFarmQuestMissionProgressCountUpdatedEventBody();
		body.missionNo = nMissionNo;
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.CreatureFarmQuestMissionProgressCountUpdated, body);
	}

	public static void SendCreatureFarmQuestMissionCompleted(ClientPeer peer, long lnAcquiredExp, int nMaxHP, int nHp, int nLevel, long lnExp, int nNextMissionNo)
	{
		SEBCreatureFarmQuestMissionCompletedEventBody body = new SEBCreatureFarmQuestMissionCompletedEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.maxHP = nMaxHP;
		body.hp = nHp;
		body.level = nLevel;
		body.exp = lnExp;
		body.nextMissionNo = nNextMissionNo;
		Send(peer, ServerEventName.CreatureFarmQuestMissionCompleted, body);
	}

	public static void SendCreatureFarmQuestMissionMonsterSpawned(ClientPeer peer, long lnInstanceId, Vector3 position, float fRemainingLifetime)
	{
		SEBCreatureFarmQuestMissionMonsterSpawnedEventBody body = new SEBCreatureFarmQuestMissionMonsterSpawnedEventBody();
		body.instanceId = lnInstanceId;
		body.position = position;
		body.remainingLifetime = fRemainingLifetime;
		Send(peer, ServerEventName.CreatureFarmQuestMissionMonsterSpawned, body);
	}

	public static void SendSafeModeStarted(ClientPeer peer)
	{
		Send(peer, ServerEventName.SafeModeStarted, null);
	}

	public static void SendSafeModeEnded(ClientPeer peer)
	{
		Send(peer, ServerEventName.SafeModeEnded, null);
	}

	public static void SendHeroSafeModeStarted(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroSafeModeStartedEventBody body = new SEBHeroSafeModeStartedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroSafeModeStarted, body);
	}

	public static void SendHeroSafeModeEnded(IEnumerable<ClientPeer> peers, Guid heroId)
	{
		SEBHeroSafeModeEndedEventBody body = new SEBHeroSafeModeEndedEventBody();
		body.heroId = (Guid)heroId;
		Send(peers, ServerEventName.HeroSafeModeEnded, body);
	}

	public static void SendGuildBlessingBuffStarted(IEnumerable<ClientPeer> peers, DateTime blessingBuffStartDate)
	{
		SEBGuildBlessingBuffStartedEventBody body = new SEBGuildBlessingBuffStartedEventBody();
		body.blessingBuffStartDate = (DateTime)blessingBuffStartDate;
		Send(peers, ServerEventName.GuildBlessingBuffStarted, body);
	}

	public static void SendGuildBlessingBuffEnded(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.GuildBlessingBuffEnded, null);
	}

	public static void SendDailyServerNationPowerRankingUpdated(IEnumerable<ClientPeer> peers, PDNationPowerRanking[] rankings)
	{
		SEBDailyServerNationPowerRankingUpdatedEventBody body = new SEBDailyServerNationPowerRankingUpdatedEventBody();
		body.rankings = rankings;
		Send(peers, ServerEventName.DailyServerNationPowerRankingUpdated, body);
	}

	public static void SendNationAllianceApplied(IEnumerable<ClientPeer> peers, PDNationAllianceApplication application)
	{
		SEBNationAllianceAppliedEventBody body = new SEBNationAllianceAppliedEventBody();
		body.application = application;
		Send(peers, ServerEventName.NationAllianceApplied, body);
	}

	public static void SendNationAllianceConcluded(IEnumerable<ClientPeer> peers, PDNationAlliance nationAlliance)
	{
		SEBNationAllianceConcludedEventBody body = new SEBNationAllianceConcludedEventBody();
		body.nationAlliance = nationAlliance;
		Send(peers, ServerEventName.NationAllianceConcluded, body);
	}

	public static void SendNationAllianceApplicationAccepted(IEnumerable<ClientPeer> peers, Guid applicationId)
	{
		SEBNationAllianceApplicationAcceptedEventBody body = new SEBNationAllianceApplicationAcceptedEventBody();
		body.applicationId = (Guid)applicationId;
		Send(peers, ServerEventName.NationAllianceApplicationAccepted, body);
	}

	public static void SendNationAllianceApplicationCanceled(IEnumerable<ClientPeer> peers, Guid applicationId)
	{
		SEBNationAllianceApplicationCanceledEventBody body = new SEBNationAllianceApplicationCanceledEventBody();
		body.applicationId = (Guid)applicationId;
		Send(peers, ServerEventName.NationAllianceApplicationCanceled, body);
	}

	public static void SendNationAllianceApplicationRejected(IEnumerable<ClientPeer> peers, Guid applicationId)
	{
		SEBNationAllianceApplicationRejectedEventBody body = new SEBNationAllianceApplicationRejectedEventBody();
		body.applicationId = (Guid)applicationId;
		Send(peers, ServerEventName.NationAllianceApplicationRejected, body);
	}

	public static void SendNationAllianceBroken(IEnumerable<ClientPeer> peers, Guid nationAllianceId)
	{
		SEBNationAllianceBrokenEventBody body = new SEBNationAllianceBrokenEventBody();
		body.nationAllianceId = (Guid)nationAllianceId;
		Send(peers, ServerEventName.NationAllianceBroken, body);
	}

	public static void SendFirstChargeEventObjectiveCompleted(ClientPeer peer)
	{
		Send(peer, ServerEventName.FirstChargeEventObjectiveCompleted, null);
	}

	public static void SendRechargeEventProgress(ClientPeer peer, int nAccUnOwnDia)
	{
		SEBRechargeEventProgressEventBody body = new SEBRechargeEventProgressEventBody();
		body.accUnOwnDia = nAccUnOwnDia;
		Send(peer, ServerEventName.RechargeEventProgress, body);
	}

	public static void SendChargeEventProgress(ClientPeer peer, int nEventId, int nAccUnOwnDia)
	{
		SEBChargeEventProgressEventBody body = new SEBChargeEventProgressEventBody();
		body.eventId = nEventId;
		body.accUnOwnDia = nAccUnOwnDia;
		Send(peer, ServerEventName.ChargeEventProgress, body);
	}

	public static void SendDailyChargeEventProgress(ClientPeer peer, DateTime date, int nAccUnOwnDia)
	{
		SEBDailyChargeEventProgressEventBody body = new SEBDailyChargeEventProgressEventBody();
		body.date = (DateTime)date;
		body.accUnOwnDia = nAccUnOwnDia;
		Send(peer, ServerEventName.DailyChargeEventProgress, body);
	}

	public static void SendConsumeEventProgress(ClientPeer peer, int nEventId, int nAccDia)
	{
		SEBConsumeEventProgressEventBody body = new SEBConsumeEventProgressEventBody();
		body.eventId = nEventId;
		body.accDia = nAccDia;
		Send(peer, ServerEventName.ConsumeEventProgress, body);
	}

	public static void SendDailyConsumeEventProgress(ClientPeer peer, DateTime date, int nAccDia)
	{
		SEBDailyConsumeEventProgressEventBody body = new SEBDailyConsumeEventProgressEventBody();
		body.date = (DateTime)date;
		body.accDia = nAccDia;
		Send(peer, ServerEventName.DailyConsumeEventProgress, body);
	}

	public static void SendJobChangeQuestProgressCountUpdated(ClientPeer peer, Guid instanceId, int nProgressCount)
	{
		SEBJobChangeQuestProgressCountUpdatedEventBody body = new SEBJobChangeQuestProgressCountUpdatedEventBody();
		body.instanceId = (Guid)instanceId;
		body.progressCount = nProgressCount;
		Send(peer, ServerEventName.JobChangeQuestProgressCountUpdated, body);
	}

	public static void SendJobChangeQuestMonsterSpawned(ClientPeer peer, long lnMonsterInstanceId, Vector3 monsterPosition, float fRemainingTime)
	{
		SEBJobChangeQuestMonsterSpawnedEventBody body = new SEBJobChangeQuestMonsterSpawnedEventBody();
		body.instanceId = lnMonsterInstanceId;
		body.position = monsterPosition;
		body.remainingLifetime = fRemainingTime;
		Send(peer, ServerEventName.JobChangeQuestMonsterSpawned, body);
	}

	public static void SendJobChangeQuestFailed(ClientPeer peer, Guid instanceId)
	{
		SEBJobChangeQuestFailedEventBody body = new SEBJobChangeQuestFailedEventBody();
		body.instanceId = (Guid)instanceId;
		Send(peer, ServerEventName.JobChangeQuestFailed, body);
	}

	public static void SendHeroJobChanged(IEnumerable<ClientPeer> peers, Guid heroId, int nJobId)
	{
		SEBHeroJobChangedEventBody body = new SEBHeroJobChangedEventBody();
		body.heroId = (Guid)heroId;
		body.jobId = nJobId;
		Send(peers, ServerEventName.HeroJobChanged, body);
	}

	public static void SendContinentExitForAnkouTombEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForAnkouTombEnter, null);
	}

	public static void SendAnkouTombMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nDifficulty, int nMatchingStatus, float fRemainingTime)
	{
		SEBAnkouTombMatchingRoomPartyEnterEventBody body = new SEBAnkouTombMatchingRoomPartyEnterEventBody();
		body.difficulty = nDifficulty;
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.AnkouTombMatchingRoomPartyEnter, body);
	}

	public static void SendAnkouTombMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBAnkouTombMatchingRoomBanishedEventBody body = new SEBAnkouTombMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.AnkouTombMatchingRoomBanished, body);
	}

	public static void SendAnkouTombMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBAnkouTombMatchingStatusChangedEventBody body = new SEBAnkouTombMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.AnkouTombMatchingStatusChanged, body);
	}

	public static void SendAnkouTombWaveStart(IEnumerable<ClientPeer> peers, int nWaveNo, PDAnkouTombMonsterInstance[] monsterInsts)
	{
		SEBAnkouTombWaveStartEventBody body = new SEBAnkouTombWaveStartEventBody();
		body.waveNo = nWaveNo;
		body.monsterInsts = monsterInsts;
		Send(peers, ServerEventName.AnkouTombWaveStart, body);
	}

	public static void SendAnkouTombPointAcquisition(ClientPeer peer, int nPoint)
	{
		SEBAnkouTombPointAcquisitionEventBody body = new SEBAnkouTombPointAcquisitionEventBody();
		body.point = nPoint;
		Send(peer, ServerEventName.AnkouTombPointAcquisition, body);
	}

	public static void SendAnkouTombMoneyBuffFinished(ClientPeer peer, int nMaxHP, int nHP)
	{
		SEBAnkouTombMoneyBuffFinishedEventBody body = new SEBAnkouTombMoneyBuffFinishedEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.AnkouTombMoneyBuffFinished, body);
	}

	public static void SendAnkouTombMoneyBuffCancel(ClientPeer peer, int nMaxHP, int nHP)
	{
		SEBAnkouTombMoneyBuffCancelEventBody body = new SEBAnkouTombMoneyBuffCancelEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.AnkouTombMoneyBuffCancel, body);
	}

	public static void SendAnkouTombClear(ClientPeer peer, int nPoint, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, long lnGold, long lnMaxGold, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBAnkouTombClearEventBody body = new SEBAnkouTombClearEventBody();
		body.point = nPoint;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.AnkouTombClear, body);
	}

	public static void SendAnkouTombFail(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, long lnGold, long lnMaxGold)
	{
		SEBAnkouTombFailEventBody body = new SEBAnkouTombFailEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		Send(peer, ServerEventName.AnkouTombFail, body);
	}

	public static void SendAnkouTombBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBAnkouTombBanishedEventBody body = new SEBAnkouTombBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.AnkouTombBanished, body);
	}

	public static void SendAnkouTombServerBestRecordUpdated(IEnumerable<ClientPeer> peers, PDHeroAnkouTombBestRecord record)
	{
		SEBAnkouTombServerBestRecordUpdatedEventBody body = new SEBAnkouTombServerBestRecordUpdatedEventBody();
		body.record = record;
		Send(peers, ServerEventName.AnkouTombServerBestRecordUpdated, body);
	}

	public static void SendArtifactOpened(ClientPeer peer, int nArtifactNo, int nArtifactLevel, int nArtifactExp, int nEquippedArtifactNo, int nMaxHP, int nHP)
	{
		SEBArtifactOpenedEventBody body = new SEBArtifactOpenedEventBody();
		body.artifactNo = nArtifactNo;
		body.artifactLevel = nArtifactLevel;
		body.artifactExp = nArtifactExp;
		body.equippedArtifactNo = nEquippedArtifactNo;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.ArtifactOpened, body);
	}

	public static void SendHeroEquippedArtifactChanged(IEnumerable<ClientPeer> peers, Guid heroId, int nEquippedArtifactNo)
	{
		SEBHeroEquippedArtifactChangedEventBody body = new SEBHeroEquippedArtifactChangedEventBody();
		body.heroId = (Guid)heroId;
		body.equippedArtifactNo = nEquippedArtifactNo;
		Send(peers, ServerEventName.HeroEquippedArtifactChanged, body);
	}

	public static void SendConstellationOpened(ClientPeer peer, PDHeroConstellation[] constellations)
	{
		SEBConstellationOpenedEventBody body = new SEBConstellationOpenedEventBody();
		body.constellations = constellations;
		Send(peer, ServerEventName.ConstellationOpened, body);
	}

	public static void SendContinentExitForTradeShipEnter(IEnumerable<ClientPeer> peers)
	{
		Send(peers, ServerEventName.ContinentExitForTradeShipEnter, null);
	}

	public static void SendTradeShipMatchingRoomPartyEnter(IEnumerable<ClientPeer> peers, int nDifficulty, int nMatchingStatus, float fRemainingTime)
	{
		SEBTradeShipMatchingRoomPartyEnterEventBody body = new SEBTradeShipMatchingRoomPartyEnterEventBody();
		body.difficulty = nDifficulty;
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.TradeShipMatchingRoomPartyEnter, body);
	}

	public static void SendTradeShipMatchingRoomBanished(ClientPeer peer, int nBanishType)
	{
		SEBTradeShipMatchingRoomBanishedEventBody body = new SEBTradeShipMatchingRoomBanishedEventBody();
		body.banishType = nBanishType;
		Send(peer, ServerEventName.TradeShipMatchingRoomBanished, body);
	}

	public static void SendTradeShipMatchingStatusChanged(IEnumerable<ClientPeer> peers, int nMatchingStatus, float fRemainingTime)
	{
		SEBTradeShipMatchingStatusChangedEventBody body = new SEBTradeShipMatchingStatusChangedEventBody();
		body.matchingStatus = nMatchingStatus;
		body.remainingTime = fRemainingTime;
		Send(peers, ServerEventName.TradeShipMatchingStatusChanged, body);
	}

	public static void SendTradeShipStepStart(IEnumerable<ClientPeer> peers, int nStepNo, PDTradeShipMonsterInstance[] monsterInsts, PDTradeShipAdditionalMonsterInstance[] additionalMonsterInsts, PDTradeShipObjectInstance[] objectInsts)
	{
		SEBTradeShipStepStartEventBody body = new SEBTradeShipStepStartEventBody();
		body.stepNo = nStepNo;
		body.monsterInsts = monsterInsts;
		body.additionalMonsterInsts = additionalMonsterInsts;
		body.objectInsts = objectInsts;
		Send(peers, ServerEventName.TradeShipStepStart, body);
	}

	public static void SendTradeShipPointAcquisition(ClientPeer peer, int nPoint)
	{
		SEBTradeShipPointAcquisitionEventBody body = new SEBTradeShipPointAcquisitionEventBody();
		body.point = nPoint;
		Send(peer, ServerEventName.TradeShipPointAcquisition, body);
	}

	public static void SendTradeShipObjectDestructionReward(ClientPeer peer, int nPoint, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBTradeShipObjectDestructionRewardEventBody body = new SEBTradeShipObjectDestructionRewardEventBody();
		body.point = nPoint;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.TradeShipObjectDestructionReward, body);
	}

	public static void SendTradeShipMoneyBuffFinished(ClientPeer peer, int nMaxHP, int nHP)
	{
		SEBTradeShipMoneyBuffFinishedEventBody body = new SEBTradeShipMoneyBuffFinishedEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.TradeShipMoneyBuffFinished, body);
	}

	public static void SendTradeShipMoneyBuffCancel(ClientPeer peer, int nMaxHP, int nHP)
	{
		SEBTradeShipMoneyBuffCancelEventBody body = new SEBTradeShipMoneyBuffCancelEventBody();
		body.maxHP = nMaxHP;
		body.hp = nHP;
		Send(peer, ServerEventName.TradeShipMoneyBuffCancel, body);
	}

	public static void SendTradeShipClear(ClientPeer peer, int nPoint, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, long lnGold, long lnMaxGold, PDItemBooty booty, PDInventorySlot[] changedInventorySlots)
	{
		SEBTradeShipClearEventBody body = new SEBTradeShipClearEventBody();
		body.point = nPoint;
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		body.booty = booty;
		body.changedInventorySlots = changedInventorySlots;
		Send(peer, ServerEventName.TradeShipClear, body);
	}

	public static void SendTradeShipFail(ClientPeer peer, long lnAcquiredExp, int nLevel, long lnExp, int nMaxHP, int nHP, long lnGold, long lnMaxGold)
	{
		SEBTradeShipFailEventBody body = new SEBTradeShipFailEventBody();
		body.acquiredExp = lnAcquiredExp;
		body.level = nLevel;
		body.exp = lnExp;
		body.maxHP = nMaxHP;
		body.hp = nHP;
		body.gold = lnGold;
		body.maxGold = lnMaxGold;
		Send(peer, ServerEventName.TradeShipFail, body);
	}

	public static void SendTradeShipBanished(ClientPeer peer, int nPreviousContinentId, int nPreviousNationId, int nHP)
	{
		SEBTradeShipBanishedEventBody body = new SEBTradeShipBanishedEventBody();
		body.previousContinentId = nPreviousContinentId;
		body.previousNationId = nPreviousNationId;
		body.hp = nHP;
		Send(peer, ServerEventName.TradeShipBanished, body);
	}

	public static void SendTradeShipServerBestRecordUpdated(IEnumerable<ClientPeer> peers, PDHeroTradeShipBestRecord record)
	{
		SEBTradeShipServerBestRecordUpdatedEventBody body = new SEBTradeShipServerBestRecordUpdatedEventBody();
		body.record = record;
		Send(peers, ServerEventName.TradeShipServerBestRecordUpdated, body);
	}

	public static void SendSystemMessage(IEnumerable<ClientPeer> peers, PDSystemMessage systemMessage)
	{
		SEBSystemMessageEventBody body = new SEBSystemMessageEventBody();
		body.systemMessage = systemMessage;
		Send(peers, ServerEventName.SystemMessage, body);
	}
}
