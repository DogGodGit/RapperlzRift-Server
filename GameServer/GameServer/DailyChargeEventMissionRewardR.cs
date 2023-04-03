using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class DailyChargeEventMissionRewardReceiveCommandHandler : InGameCommandHandler<DailyChargeEventMissionRewardReceiveCommandBody, DailyChargeEventMissionRewardReceiveResponseBody>
{
	public const short kResult_EventNotExist = 101;

	public const short kResult_NotRewardableDate = 102;

	public const short kResult_NotEnoughHeroLevel = 103;

	public const short kResult_ObjectiveNotCompleted = 104;

	public const short kResult_AlreadyReceived = 105;

	public const short kResult_NotEnoughInventory = 106;

	private DateTime m_date = DateTime.MinValue.Date;

	private int m_nMissionNo;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DailyChargeEvent m_evt;

	private DailyChargeEventMission m_mission;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private ResultItemCollection m_rewardItemCollection = new ResultItemCollection();

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_date = (DateTime)m_body.date;
		m_nMissionNo = m_body.missionNo;
		if (m_nMissionNo <= 0)
		{
			throw new CommandHandleException(1, "미션번호가 유효하지 않습니다. m_nMissionNo = " + m_nMissionNo);
		}
		m_currentTime = DateTimeUtil.currentTime;
		if (m_date != m_currentTime.Date)
		{
			throw new CommandHandleException(102, "해당 날짜는 보상받을 수 없습니다.");
		}
		m_evt = Resource.instance.dailyChargeEvent;
		if (m_evt == null)
		{
			throw new CommandHandleException(101, "이벤트가 존재하지 않습니다.");
		}
		m_mission = m_evt.GetMission(m_nMissionNo);
		if (m_mission == null)
		{
			throw new CommandHandleException(1, "미션이 존재하지 않습니다. m_nMissionNo = " + m_nMissionNo);
		}
		if (m_myHero.level < m_evt.requiredHeroLevel)
		{
			throw new CommandHandleException(103, "영웅레벨이 부족합니다.");
		}
		m_myAccount.RefreshDailyChargeEvent(m_date);
		if (m_myAccount.dailyChargeEventAccUnOwnDia < m_mission.requiredUnOwnDia)
		{
			throw new CommandHandleException(104, "목표액을 충전하지 않았습니다.");
		}
		if (m_myAccount.IsRewardedDailyChargeEventMission(m_nMissionNo))
		{
			throw new CommandHandleException(105, "이미 보상을 받았습니다.");
		}
		foreach (DailyChargeEventMissionReward reward in m_mission.rewards.Values)
		{
			ItemReward itemReward = reward.itemReward;
			m_rewardItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
		}
		if (!m_myHero.IsAvailableInventory(m_rewardItemCollection))
		{
			throw new CommandHandleException(106, "인벤토리가 부족합니다.");
		}
		foreach (ResultItem result in m_rewardItemCollection.resultItems)
		{
			m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
		}
		m_myAccount.AddRewardedDailyChargeEventMission(m_nMissionNo);
		SaveToDB();
		SaveToGameLogDB();
		DailyChargeEventMissionRewardReceiveResponseBody resBody = new DailyChargeEventMissionRewardReceiveResponseBody();
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
		dbWork.AddSqlCommand(GameDac.CSC_AddAccountDailyChargeEventMissionReward(m_myAccount.id, m_date, m_nMissionNo));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyChargeEventMissionRewardLog(logId, m_myAccount.id, m_myHero.id, m_date, m_nMissionNo, m_currentTime));
			foreach (ResultItem rewardItem in m_rewardItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyChargeEventMissionRewardDetailLog(Guid.NewGuid(), logId, rewardItem.item.id, rewardItem.owned, rewardItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
