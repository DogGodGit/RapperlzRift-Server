using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Open7DayEventMissionRewardReceiveCommandHandler : InGameCommandHandler<Open7DayEventMissionRewardReceiveCommandBody, Open7DayEventMissionRewardReceiveResponseBody>
{
	public const short kResult_AlreadyReceivedReward = 101;

	public const short kResult_NotOpendEvent = 102;

	public const short kResult_NotElapsedDay = 103;

	public const short kResult_NotMissionCompleted = 104;

	public const short kResult_NotEnoughInventory = 105;

	private ResultItemCollection m_resultItemCollection = new ResultItemCollection();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMissionId = m_body.missionId;
		if (nMissionId <= 0)
		{
			throw new CommandHandleException(1, "미션ID가 유효하지 않습니다. nMissionId = " + nMissionId);
		}
		if (m_myHero.IsRewardedOpen7DayEventMission(nMissionId))
		{
			throw new CommandHandleException(101, "이미 보상받은 미션입니다.");
		}
		if (!m_myHero.IsMainQuestCompleted(Resource.instance.open7DayEventRequiredMainQuestNo))
		{
			throw new CommandHandleException(102, "개방되지 않은 이벤트입니다.");
		}
		Open7DayEventMission open7DayEventMission = Resource.instance.GetOpen7DayEventMission(nMissionId);
		if (open7DayEventMission == null)
		{
			throw new CommandHandleException(1, "미션이 존재하지 않습니다. nMissionId = " + nMissionId);
		}
		if (open7DayEventMission.day.day > m_myHero.GetElapsedDaysFromCreation(m_currentTime))
		{
			throw new CommandHandleException(103, "받을 수 있는 일차가 아닙니다.");
		}
		if (open7DayEventMission.targetValue > m_myHero.GetOpen7DayEventMissionProgressValue(open7DayEventMission.type))
		{
			throw new CommandHandleException(104, "미션이 완료되지 않았습니다.");
		}
		m_resultItemCollection = new ResultItemCollection();
		foreach (Open7DayEventMissionReward reward in open7DayEventMission.rewards)
		{
			ItemReward itemReward = reward.itemReward;
			if (itemReward != null)
			{
				m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
		{
			throw new CommandHandleException(105, "인벤토리가 부족합니다.");
		}
		foreach (ResultItem reusltItem in m_resultItemCollection.resultItems)
		{
			m_myHero.AddItem(reusltItem.item, reusltItem.owned, reusltItem.count, m_changedInventorySlots);
		}
		m_myHero.AddRewardedOpen7DayEventMission(open7DayEventMission.id);
		SaveToDB(open7DayEventMission.id);
		SaveToLogDB(open7DayEventMission.id);
		Open7DayEventMissionRewardReceiveResponseBody resBody = new Open7DayEventMissionRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nMissionId)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroOpen7DayEventMission(m_myHero.id, nMissionId));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nMissionId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOpen7DayEventRewardLog(logId, m_myHero.id, nMissionId, m_currentTime));
			foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOpen7DayEventRewardDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.owned, resultItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
