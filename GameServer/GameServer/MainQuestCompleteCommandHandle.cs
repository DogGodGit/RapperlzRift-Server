using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MainQuestCompleteCommandHandler : InGameCommandHandler<MainQuestCompleteCommandBody, MainQuestCompleteResponseBody>
{
	public const short kResult_AlreadyCompletedMainQuest = 101;

	public const short kResult_IncompleteObjective = 102;

	public const short kResult_UnableInteractionPositionWithCompletionNPC = 103;

	public const short kResult_NotEnoughInventory = 104;

	private HeroMainQuest m_heroMainQuest;

	private int m_nOldMainQuestNo;

	private long m_lnRewardExp;

	private List<HeroMainGear> m_rewardMainGears = new List<HeroMainGear>();

	private List<HeroSubGear> m_rewardSubGears = new List<HeroSubGear>();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<HeroMount> m_rewardMounts = new List<HeroMount>();

	private List<HeroCreatureCard> m_changedCreatureCards = new List<HeroCreatureCard>();

	private bool m_bIsRankOpened;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMainQuestNo = m_body.mainQuestNo;
		if (nMainQuestNo <= 0)
		{
			throw new CommandHandleException(1, "메인퀘스트 번호가 유효하지 않습니다. nMainQuestNo = " + nMainQuestNo);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		m_heroMainQuest = m_myHero.currentHeroMainQuest;
		if (m_heroMainQuest == null)
		{
			throw new CommandHandleException(1, "영웅메인퀘스트가 존재하지 않습니다.");
		}
		MainQuest mainQuest = m_heroMainQuest.mainQuest;
		if (mainQuest.no != nMainQuestNo)
		{
			throw new CommandHandleException(1, "현재 메인퀘스트 번호가 아닙니다. nMainQuestNo = " + nMainQuestNo);
		}
		if (m_heroMainQuest.completed)
		{
			throw new CommandHandleException(101, "이미 완료된 메인퀘스트입니다.");
		}
		if (!m_heroMainQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(102, "메인퀘스트 목표가 완료되지 않았습니다.");
		}
		m_nOldMainQuestNo = mainQuest.no;
		Npc completionNpc = mainQuest.completionNpc;
		if (completionNpc != null)
		{
			if (!currentPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
			{
				throw new CommandHandleException(1, "완료 NPC가 있는 장소가 아닙니다.");
			}
			if (!completionNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
			{
				throw new CommandHandleException(103, "완료NPC와 상호작용할 수 있는 위치가 아닙니다.");
			}
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		int nRemainingEmptyInventoryCount = m_myHero.emptyInventorySlotCount;
		foreach (MainQuestReward reward2 in mainQuest.rewards)
		{
			if (reward2.jobId > 0 && reward2.jobId != m_myHero.baseJobId)
			{
				continue;
			}
			switch (reward2.type)
			{
			case MainQuestRewardType.MainGear:
				if (nRemainingEmptyInventoryCount < 1)
				{
					throw new CommandHandleException(104, "인벤토리가 부족합니다.");
				}
				nRemainingEmptyInventoryCount--;
				break;
			case MainQuestRewardType.SubGear:
				if (!m_myHero.ContainsSubGear(reward2.subGear.id))
				{
					if (nRemainingEmptyInventoryCount < 1)
					{
						throw new CommandHandleException(104, "인벤토리가 부족합니다.");
					}
					nRemainingEmptyInventoryCount--;
				}
				break;
			case MainQuestRewardType.Item:
			{
				ItemReward itemReward2 = reward2.itemReward;
				resultItemCollection.AddResultItemCount(itemReward2.item, itemReward2.owned, itemReward2.count);
				break;
			}
			default:
				throw new CommandHandleException(1, "타입이 유효하지 않습니다.");
			case MainQuestRewardType.Mount:
			case MainQuestRewardType.CreatureCard:
				break;
			}
		}
		if (!m_myHero.IsAvailableInventory(resultItemCollection, nRemainingEmptyInventoryCount))
		{
			throw new CommandHandleException(104, "인벤토리가 부족합니다.");
		}
		foreach (MainQuestReward reward in mainQuest.rewards)
		{
			if (reward.jobId > 0 && reward.jobId != m_myHero.baseJobId)
			{
				continue;
			}
			switch (reward.type)
			{
			case MainQuestRewardType.MainGear:
			{
				int nEnchantLevel = 0;
				bool bOwned = reward.mainGearOwned;
				HeroMainGear heroMainGear = new HeroMainGear(m_myHero);
				heroMainGear.Init(reward.mainGear, nEnchantLevel, bOwned, m_currentTime);
				m_myHero.AddMainGear(heroMainGear, bInit: false, m_currentTime);
				InventorySlot emptySlot = m_myHero.GetEmptyInventorySlot();
				emptySlot.Place(heroMainGear);
				m_changedInventorySlots.Add(emptySlot);
				m_rewardMainGears.Add(heroMainGear);
				break;
			}
			case MainQuestRewardType.SubGear:
			{
				SubGear rewardSubGear = reward.subGear;
				int nLevel = 1;
				if (!m_myHero.ContainsSubGear(rewardSubGear.id))
				{
					HeroSubGear heroSubGear = new HeroSubGear(m_myHero);
					heroSubGear.Init(rewardSubGear, nLevel, m_currentTime);
					m_myHero.AddSubGear(heroSubGear);
					InventorySlot emptySlot2 = m_myHero.GetEmptyInventorySlot();
					emptySlot2.Place(heroSubGear);
					m_changedInventorySlots.Add(emptySlot2);
					m_rewardSubGears.Add(heroSubGear);
				}
				break;
			}
			case MainQuestRewardType.Item:
			{
				ItemReward itemReward = reward.itemReward;
				m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
				break;
			}
			case MainQuestRewardType.Mount:
			{
				Mount mount = reward.mount;
				int nMountLevel = 1;
				int nMountSatiety = 0;
				if (!m_myHero.ContainsMount(mount.id))
				{
					HeroMount heroMount = new HeroMount(m_myHero, mount, nMountLevel, nMountSatiety);
					m_myHero.AddMount(heroMount);
					heroMount.RefreshAttrTotalValues();
					m_rewardMounts.Add(heroMount);
				}
				break;
			}
			case MainQuestRewardType.CreatureCard:
			{
				CreatureCard creatureCard = reward.creatureCard;
				HeroCreatureCard heroCreatureCard = m_myHero.IncreaseCreatureCardCount(creatureCard);
				m_changedCreatureCards.Add(heroCreatureCard);
				break;
			}
			}
		}
		long lnGold = mainQuest.goldRewardValue;
		m_lnRewardExp = mainQuest.expRewardValue;
		if (lnGold > 0)
		{
			m_myHero.AddGold(lnGold);
		}
		if (m_lnRewardExp > 0)
		{
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		m_heroMainQuest.completed = true;
		if (nMainQuestNo == Resource.instance.rankOpenRequiredMainQuestNo)
		{
			m_bIsRankOpened = true;
			m_myHero.SetRank(Resource.instance.GetRank(1));
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendHeroRankAcquired(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, m_myHero.rank.no);
		}
		if (mainQuest.type == 6)
		{
			ProcessCompletionForTransport();
		}
		else
		{
			Finish();
		}
	}

	private void ProcessCompletionForTransport()
	{
		CartSynchronizer.Exec(m_heroMainQuest.cartInst, new SFAction(ProcessCompletionForTransport_CartRemove));
	}

	private void ProcessCompletionForTransport_CartRemove()
	{
		MainQuestCartInstance cartInst = m_heroMainQuest.cartInst;
		if (cartInst.isRiding)
		{
			cartInst.GetOff(m_currentTime, bSendEvent: false);
			ServerEvent.SendHeroEnter(m_myHero.currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.ToPDHero(m_currentTime), bIsRevivalEnter: false);
		}
		((ContinentInstance)cartInst.currentPlace)?.ExitCart(cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(cartInst);
		m_heroMainQuest.cartInst = null;
		Finish();
	}

	private void Finish()
	{
		List<long> removedAbnormalStateEffects = new List<long>();
		if (m_heroMainQuest.mainQuest.transformationRestored)
		{
			m_myHero.CancelMainQuestTransformationMonsterEffect(bResetHP: true, bSendEventMyself: false, bSendEventToOthers: true, removedAbnormalStateEffects);
		}
		SaveToDB();
		MainQuestCompleteResponseBody resBody = new MainQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.rankNo = m_myHero.rankNo;
		resBody.rewardMainGears = HeroMainGear.ToPDFullHeroMainGears(m_rewardMainGears).ToArray();
		resBody.maxAcquisitionMainGearGrade = m_myHero.maxAcquisitionMainGearGrade;
		resBody.rewardSubGears = HeroSubGear.ToPDFullHeroSubGears(m_rewardSubGears).ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.rewardMounts = HeroMount.ToPDHeroMounts(m_rewardMounts).ToArray();
		resBody.changedCreatureCards = HeroCreatureCard.ToPDHeroCreatureCards(m_changedCreatureCards).ToArray();
		resBody.removedAbnormalStateEffects = removedAbnormalStateEffects.ToArray();
		SendResponseOK(resBody);
		m_myHero.ProcessAutoAcceptedSubQuestsForMainQuest(m_nOldMainQuestNo, m_currentTime, bSendEvent: true);
		m_myHero.ProcessArtifactOpenForMainQuest(bSendEvent: true);
		m_myHero.ProcessConstellationOpenForMainQuest(bSendEvent: true);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		if (m_bIsRankOpened)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Rank(m_myHero));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainQuest_Complete(m_myHero.id, m_myHero.currentHeroMainQuest.mainQuest.no, m_currentTime));
		foreach (HeroMainGear mainGear in m_rewardMainGears)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroMainGear(mainGear));
		}
		foreach (HeroSubGear subGear in m_rewardSubGears)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroSubGear(subGear.hero.id, subGear.subGearId, subGear.level, subGear.quality, subGear.equipped, subGear.regTime));
		}
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		foreach (HeroMount heroMount in m_rewardMounts)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroMount(heroMount.hero.id, heroMount.mount.id, heroMount.level, heroMount.satiety));
		}
		foreach (HeroCreatureCard heroCreatureCard in m_changedCreatureCards)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(heroCreatureCard));
		}
		dbWork.Schedule();
	}
}
