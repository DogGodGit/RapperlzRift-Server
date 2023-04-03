using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ClientEventHandlerFactory : SFHandlerFactory<short, IEventHandler>
{
	private static ClientEventHandlerFactory s_instance = new ClientEventHandlerFactory();

	public static ClientEventHandlerFactory instance => s_instance;

	private void AddHandlerType<T>(ClientEventName eventName) where T : IEventHandler
	{
		AddHandlerType<T>((short)eventName);
	}

	public void Init()
	{
		AddHandlerType<MoveStartEventHandler>(ClientEventName.MoveStart);
		AddHandlerType<MoveEndEventHandler>(ClientEventName.MoveEnd);
		AddHandlerType<MoveEventHandler>(ClientEventName.Move);
		AddHandlerType<MoveModeChangedEventHandler>(ClientEventName.MoveModeChanged);
		AddHandlerType<SkillCastEventHandler>(ClientEventName.SkillCast);
		AddHandlerType<SkillHitEventHandler>(ClientEventName.SkillHit);
		AddHandlerType<TamingMonsterSkillCastEventHandler>(ClientEventName.TamingMonsterSkillCast);
		AddHandlerType<TamingMonsterSkillhitEventHandler>(ClientEventName.TamingMonsterSkillHit);
		AddHandlerType<WarMemoryTransformationMonsterSkillCastEventHandler>(ClientEventName.WarMemoryTransformationMonsterSkillCast);
		AddHandlerType<WarMemoryTransformationMonsterSkillHitEventHandler>(ClientEventName.WarMemoryTransformationMonsterSkillHit);
		AddHandlerType<MainQuestTransformationMonsterSkillCastEventHandler>(ClientEventName.MainQuestTransformationMonsterSkillCast);
		AddHandlerType<MainQuestTransformationMonsterSkillHitEventHandler>(ClientEventName.MainQuestTransformationMonsterSkillHit);
		AddHandlerType<ContinentObjectInteractionCancelEventHandler>(ClientEventName.ContinentObjectInteractionCancel);
		AddHandlerType<ReturnScrollUseCancelEventHandler>(ClientEventName.ReturnScrollUseCancel);
		AddHandlerType<MountGetOffEventHandler>(ClientEventName.MountGetOff);
		AddHandlerType<MonsterMoveEventHandler>(ClientEventName.MonsterMove);
		AddHandlerType<FishingCancelEventHandler>(ClientEventName.FishingCancel);
		AddHandlerType<MysteryBoxPickCancelEventHandler>(ClientEventName.MysteryBoxPickCancel);
		AddHandlerType<SecretLetterPickCancelEventHandler>(ClientEventName.SecretLetterPickCancel);
		AddHandlerType<DimensionRaidInteractionCancelEventHandler>(ClientEventName.DimensionRaidInteractionCancel);
		AddHandlerType<CartMoveStartEventHandler>(ClientEventName.CartMoveStart);
		AddHandlerType<CartMoveEndEventHandler>(ClientEventName.CartMoveEnd);
		AddHandlerType<CartMoveEventHandler>(ClientEventName.CartMove);
		AddHandlerType<GuildFarmQuestInteractionCancelEventHandler>(ClientEventName.GuildFarmQuestInteractionCancel);
		AddHandlerType<GuildAltarSpellInjectionMissionCancelEventHandler>(ClientEventName.GuildAltarSpellInjectionMissionCancel);
		AddHandlerType<JobCommonSkillCastEventHandler>(ClientEventName.JobCommonSkillCast);
		AddHandlerType<JobCommonSkillHitEventHandler>(ClientEventName.JobCommonSkillHit);
		AddHandlerType<GroggyMonsterItemStealCancelEventHandler>(ClientEventName.GroggyMonsterItemStealCancel);
		AddHandlerType<RankActiveSkillCastEventHandler>(ClientEventName.RankActiveSkillCast);
		AddHandlerType<WisdomTempleObjectInteractionCancelEventHandler>(ClientEventName.WisdomTempleObjectInteractionCancel);
		AddHandlerType<RuinsReclaimObjectInteractionCancelEventHandler>(ClientEventName.RuinsReclaimObjectInteractionCancel);
		AddHandlerType<TrueHeroQuestStepInteractionCancelEventHandler>(ClientEventName.TrueHeroQuestStepInteractionCancel);
		AddHandlerType<WarMemoryObjectInteractionCancelEventHandler>(ClientEventName.WarMemoryObjectInteractionCancel);
		AddHandlerType<AutoHuntStartEventHandler>(ClientEventName.AutoHuntStart);
		AddHandlerType<AutoHuntEndEventHandler>(ClientEventName.AutoHuntEnd);
	}
}
