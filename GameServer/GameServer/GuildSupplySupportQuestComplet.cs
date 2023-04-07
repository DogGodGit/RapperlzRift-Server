using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildSupplySupportQuestCompleteCommandHandler : InGameCommandHandler<GuildSupplySupportQuestCompleteCommandBody, GuildSupplySupportQuestCompleteResponseBody>
{
	public const short kResult_NotGuildMember = 101;

	public const short kResult_NotProgressingQuest = 102;

	public const short kResult_UnableQuestCompletePositionWithCompletionNPC_Hero = 103;

	public const short kResult_UnableQuestCompletePositionWithCompletionNPC_Cart = 104;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private ContinentInstance m_currentPlace;

	private GuildSupplySupportQuestPlay m_guildQuest;

	private GuildSupplySupportQuest m_quest;

	private Guild m_guild;

	private int m_nRewardGuildBuildingPoint;

	private int m_nRewardGuildFund;

	private int m_nRewardGuildContributionPoint;

	private long m_lnRewardExp;

	private GuildSupplySupportQuestCartInstance m_cartInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_quest = Resource.instance.guildSupplySupportQuest;
		m_currentTime = DateTimeUtil.currentTime;
		m_currentPlace = m_myHero.currentPlace as ContinentInstance;
		if (m_currentPlace == null)
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입하지 않았습니다.");
		}
		m_guild = guildMember.guild;
		m_guildQuest = m_guild.guildSupplySupportQuestPlay;
		if (m_guildQuest == null)
		{
			throw new CommandHandleException(102, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		Npc completionNpc = m_quest.completionNpc;
		if (!m_currentPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "영웅이 퀘스트완료NPC와 다른장소에 있습니다.");
		}
		if (!completionNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(103, "영웅이 퀘스트완료NPC 상호작용 범위에 있지 않습니다.");
		}
		m_cartInst = m_guildQuest.cartInst;
		lock (m_cartInst.syncObject)
		{
			ValidationCheckCart();
		}
	}

	private void ValidationCheckCart()
	{
		Npc completionNpc = m_quest.completionNpc;
		ContinentInstance cartPlace = (ContinentInstance)m_cartInst.currentPlace;
		if (!cartPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "카트가 퀘스트완료NPC와 다른장소에 있습니다.");
		}
		if (!completionNpc.IsInteractionEnabledPosition(m_cartInst.position, m_myHero.radius))
		{
			throw new CommandHandleException(104, "카트가 퀘스트완료NPC 주위에 있지 않습니다");
		}
		Reward();
	}

	private void Reward()
	{
		GuildBuildingPointReward guildBuildingPointReward = m_quest.guildBuildingPointReward;
		if (guildBuildingPointReward != null)
		{
			m_nRewardGuildBuildingPoint = guildBuildingPointReward.value;
			m_guild.AddBuildingPoint(m_nRewardGuildBuildingPoint, m_myHero.id);
		}
		GuildFundReward guildFundReward = m_quest.guildFundReward;
		if (guildFundReward != null)
		{
			m_nRewardGuildFund = guildFundReward.value;
			m_guild.AddFund(m_nRewardGuildFund, m_myHero.id);
		}
		GuildContributionPointReward guildContributionReward = m_quest.completionGuildContributionPointReward;
		if (guildContributionReward != null)
		{
			m_nRewardGuildContributionPoint = guildContributionReward.value;
			m_myHero.AddGuildContributionPoint(m_nRewardGuildContributionPoint);
		}
		GuildSupplySupportQuestReward guildSupplySupporQuestReward = m_quest.GetReward(m_myHero.level);
		if (guildSupplySupporQuestReward != null)
		{
			ExpReward expReward2 = guildSupplySupporQuestReward.expReward;
			if (expReward2 != null)
			{
				m_lnRewardExp = expReward2.value;
				m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
				m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
		}
		foreach (GuildMember guildMember in m_guild.members.Values)
		{
			Hero hero = m_currentPlace.GetHero(guildMember.id);
			if (hero == null || hero.id == m_myHero.id)
			{
				continue;
			}
			lock (hero.syncObject)
			{
				if (!MathUtil.CircleContains(m_myHero.position, m_quest.completionRewardableRadius, hero.position))
				{
					continue;
				}
				if (m_nRewardGuildContributionPoint > 0)
				{
					hero.AddGuildContributionPoint(m_nRewardGuildContributionPoint);
				}
				GuildSupplySupportQuestReward reward = m_quest.GetReward(hero.level);
				long lnRewardExp = 0L;
				if (reward != null)
				{
					ExpReward expReward = reward.expReward;
					if (expReward != null)
					{
						lnRewardExp = expReward.value;
						lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(hero.level));
						hero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
					}
				}
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildContributionPoint(hero.id, hero.totalGuildContributionPoint, hero.guildContributionPoint));
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(hero));
				dbWork.Schedule();
				try
				{
					SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
					logWork.AddSqlCommand(GameLogDac.CSC_AddGuildSupplySupportQuestExtraRewardLog(Guid.NewGuid(), m_guild.id, hero.id, m_guildQuest.id, m_nRewardGuildContributionPoint, lnRewardExp, m_currentTime));
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					LogError(null, ex, bStackTrace: true);
				}
				ServerEvent.SendGuildSupplySupportQuestCompleted(hero.account.peer, lnRewardExp, hero.realMaxHP, hero.hp, hero.exp, hero.level, hero.totalGuildContributionPoint, hero.guildContributionPoint);
				if (m_guildQuest.startTime.Date == m_currentTime.Date)
				{
					hero.ProcessTodayTask(22, m_currentTime.Date);
				}
			}
		}
		m_guild.RemoveGuildSupplySupportQuest();
		RemoveCart();
	}

	private void RemoveCart()
	{
		if (m_cartInst.isRiding)
		{
			m_cartInst.GetOff(m_currentTime, bSendEvent: false);
			ServerEvent.SendHeroEnter(m_currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.ToPDHero(m_currentTime), bIsRevivalEnter: false);
		}
		((ContinentInstance)m_cartInst.currentPlace)?.ExitCart(m_cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(m_cartInst);
		m_guildQuest.cartInst = null;
		Finish();
	}

	private void Finish()
	{
		SaveToDB();
		SaveToDB_Log();
		GuildSupplySupportQuestCompleteResponseBody resBody = new GuildSupplySupportQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnRewardExp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.exp = m_myHero.exp;
		resBody.level = m_myHero.level;
		resBody.totalGuildContributionPoint = m_myHero.totalGuildContributionPoint;
		resBody.guildContributionPoint = m_myHero.guildContributionPoint;
		resBody.giFund = m_guild.fund;
		resBody.giBuildingPoint = m_guild.buildingPoint;
		SendResponseOK(resBody);
		if (m_guildQuest.startTime.Date == m_currentTime.Date)
		{
			m_myHero.ProcessTodayTask(22, m_currentTime.Date);
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuildSupplySupportQuest_Status(m_guildQuest.id, 1, m_currentTime));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_BuildingPoint(m_guild.id, m_guild.buildingPoint));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Fund(m_guild.id, m_guild.fund));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildContributionPoint(m_myHero.id, m_myHero.totalGuildContributionPoint, m_myHero.guildContributionPoint));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.Schedule();
	}

	private void SaveToDB_Log()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildSupplySupportQuestRewardLog(Guid.NewGuid(), m_guild.id, m_myHero.id, m_guildQuest.id, m_nRewardGuildBuildingPoint, m_nRewardGuildFund, m_currentTime));
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildSupplySupportQuestExtraRewardLog(Guid.NewGuid(), m_guild.id, m_myHero.id, m_guildQuest.id, m_nRewardGuildContributionPoint, m_lnRewardExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
