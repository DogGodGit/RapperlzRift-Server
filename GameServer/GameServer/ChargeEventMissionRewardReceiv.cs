using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ChargeEventMissionRewardReceiveCommandHandler : InGameCommandHandler<ChargeEventMissionRewardReceiveCommandBody, ChargeEventMissionRewardReceiveResponseBody>
{
	public const short kResult_EventNotExist = 101;

	public const short kResult_NotEventTime = 102;

	public const short kResult_ObjectiveNotCompleted = 103;

	public const short kResult_AlreadyReceived = 104;

	public const short kResult_NotEnoughInventory = 105;

	private int m_nEventId;

	private int m_nMissionNo;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private ChargeEvent m_evt;

	private ChargeEventMission m_mission;

	private AccountChargeEvent m_accountChargeEvent;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private ResultItemCollection m_rewardItemCollection = new ResultItemCollection();

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nEventId = m_body.eventId;
		if (m_nEventId <= 0)
		{
			throw new CommandHandleException(1, "이벤트ID가 유효하지 않습니다. m_nEventId = " + m_nEventId);
		}
		m_nMissionNo = m_body.missionNo;
		if (m_nMissionNo <= 0)
		{
			throw new CommandHandleException(1, "미션번호가 유효하지 않습니다. m_nMissionNo = " + m_nMissionNo);
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_evt = Resource.instance.GetChargeEvent(m_nEventId);
		if (m_evt == null)
		{
			throw new CommandHandleException(101, "이벤트가 존재하지 않습니다. m_nEventId = " + m_nEventId);
		}
		if (!m_evt.IsEventTime(m_currentTime.DateTime))
		{
			throw new CommandHandleException(102, "이벤트시간이 아닙니다. m_nEventId = " + m_nEventId);
		}
		m_mission = m_evt.GetMission(m_nMissionNo);
		if (m_mission == null)
		{
			throw new CommandHandleException(1, "미션이 존재하지 않습니다. m_nEventId = " + m_nEventId + ", m_nMissionNo = " + m_nMissionNo);
		}
		m_accountChargeEvent = m_myAccount.GetOrCreateChargeEvent(m_nEventId);
		if (m_accountChargeEvent.accUnOwnDia < m_mission.requiredUnOwnDia)
		{
			throw new CommandHandleException(103, "목표액을 충전하지 않았습니다.");
		}
		if (m_accountChargeEvent.IsRewardedMission(m_nMissionNo))
		{
			throw new CommandHandleException(104, "이미 보상을 받았습니다.");
		}
		foreach (ChargeEventMissionReward reward in m_mission.rewards.Values)
		{
			ItemReward itemReward = reward.itemReward;
			m_rewardItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
		}
		if (!m_myHero.IsAvailableInventory(m_rewardItemCollection))
		{
			throw new CommandHandleException(105, "인벤토리가 부족합니다.");
		}
		foreach (ResultItem result in m_rewardItemCollection.resultItems)
		{
			m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
		}
		m_accountChargeEvent.AddRewardedMission(m_nMissionNo);
		SaveToDB();
		SaveToGameLogDB();
		ChargeEventMissionRewardReceiveResponseBody resBody = new ChargeEventMissionRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddAccountChargeEventMissionReward(m_myAccount.id, m_nEventId, m_nMissionNo));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroChargeEventMissionRewardLog(logId, m_myAccount.id, m_myHero.id, m_nEventId, m_nMissionNo, m_currentTime));
			foreach (ResultItem rewardItem in m_rewardItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroChargeEventMissionRewardDetailLog(Guid.NewGuid(), logId, rewardItem.item.id, rewardItem.owned, rewardItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
