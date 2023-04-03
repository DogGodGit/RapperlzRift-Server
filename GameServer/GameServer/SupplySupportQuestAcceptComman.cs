using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestAcceptCommandHandler : InGameCommandHandler<SupplySupportQuestAcceptCommandBody, SupplySupportQuestAcceptResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_NotEnoughOrderItem = 102;

	public const short kResult_NotEnoughGold = 103;

	public const short kResult_OverflowedStartCount = 104;

	public const short kResult_UnableInteractionPositionWithStartNPC = 105;

	public const short kResult_PlayingCartQuest = 106;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HeroSupplySupportQuest m_heroQuest;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private SupplySupportQuestCartInstance m_cartInst;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nOrderId = m_body.orderId;
		SupplySupportQuest quest = Resource.instance.supplySupportQuest;
		SupplySupportQuestOrder order = quest.GetOrder(nOrderId);
		if (order == null)
		{
			throw new CommandHandleException(1, "지령ID가 유효하지 않습니다. nOrderId = " + nOrderId);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재장소에서는 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.level < quest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅의 레벨이 부족합니다.");
		}
		if (m_myHero.isTransformMonster)
		{
			throw new CommandHandleException(1, "몬스터 변신중에는 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.supplySupportQuest != null)
		{
			throw new CommandHandleException(1, "이미 보급지원퀘스트를 진행하는 중입니다.");
		}
		if (m_myHero.isPlayingCartQuest)
		{
			throw new CommandHandleException(106, "영웅이 카트퀘스트를 진행중입니다.");
		}
		int nOrderItemId = order.orderItemId;
		Item orderItem = Resource.instance.GetItem(nOrderItemId);
		if (m_myHero.GetItemCount(nOrderItemId) <= 0)
		{
			throw new CommandHandleException(102, "지령아이템이 부족합니다.");
		}
		int nGuaranteeGold = quest.GuaranteeGold;
		if (m_myHero.gold < nGuaranteeGold)
		{
			throw new CommandHandleException(103, "보증금에 필요한 골드가 부족합니다.");
		}
		m_myHero.RefreshDailySupplySupportQuestStartCount(currentDate);
		DateValuePair<int> startCount = m_myHero.dailySupplySupportQuestStartCount;
		if (startCount.value >= quest.limitCount)
		{
			throw new CommandHandleException(104, "금일 시작횟수를 모두 사용했습니다.");
		}
		Npc startNpc = quest.startNpc;
		if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "시작 NPC에 있는 장소가 아닙니다.");
		}
		if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(105, "시작 NPC와 상호작용할 수 있는 위치가 아닙니다.");
		}
		m_myHero.UseItem(nOrderItemId, bFisetUseOwn: true, 1, m_changedInventorySlots, out var nUsedOwnCount, out var _);
		m_myHero.UseGold(nGuaranteeGold);
		m_heroQuest = new HeroSupplySupportQuest(m_myHero);
		m_heroQuest.Init(order, m_currentTime);
		m_myHero.supplySupportQuest = m_heroQuest;
		startCount.value++;
		m_cartInst = new SupplySupportQuestCartInstance();
		lock (m_cartInst.syncObject)
		{
			m_cartInst.Init(m_heroQuest, m_currentTime, bFirstCreation: true);
			m_cartInst.SetPositionAndRotation(m_myHero.position, m_myHero.rotationY);
			currentPlace.EnterCart(m_cartInst, m_currentTime, bSendEvent: false);
			m_cartInst.GetOn(m_currentTime);
			SaveToDB();
			SaveToDB_Log(nOrderItemId, nUsedOwnCount > 0, nGuaranteeGold);
			SupplySupportQuestAcceptResponseBody resBody = new SupplySupportQuestAcceptResponseBody();
			resBody.cartInst = (PDSupplySupportQuestCartInstance)m_cartInst.ToPDCartInstance(m_currentTime);
			resBody.remainingTime = m_heroQuest.GetRemainingTime(m_currentTime);
			resBody.date = (DateTime)startCount.date;
			resBody.supplySupportQuestDailyStartCount = startCount.value;
			resBody.changedInventorySlot = m_changedInventorySlots[0].ToPDInventorySlot();
			resBody.gold = m_myHero.gold;
			SendResponseOK(resBody);
			m_myHero.ProcessSeriesMission(9);
			m_myHero.ProcessTodayMission(9, m_currentTime);
			m_myHero.ProcessTodayTask(19, currentDate);
			m_myHero.ProcessRetrievalProgressCount(6, currentDate);
			if (orderItem.grade == 5)
			{
				m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.LegendSupplySupportQuest, 1, m_currentTime);
			}
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		HeroSupplySupportQuest quest = m_myHero.supplySupportQuest;
		dbWork.AddSqlCommand(GameDac.CSC_AddSupplySupportQuest(quest.id, m_myHero.id, quest.cart.id, quest.cartHp, quest.isCartRiding, quest.cartContinentId, quest.cartPosition.x, quest.cartPosition.y, quest.cartPosition.z, quest.cartYRotation, quest.orderId, 0, m_currentTime, m_currentTime));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.Schedule();
	}

	private void SaveToDB_Log(int nOrderItemId, bool bOrderItemOwned, int nGuaranteeGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSupplySupportQuestStartLog(Guid.NewGuid(), m_myHero.id, m_heroQuest.id, nOrderItemId, 1, bOrderItemOwned, nGuaranteeGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
