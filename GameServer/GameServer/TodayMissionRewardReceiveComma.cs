using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TodayMissionRewardReceiveCommandHandler : InGameCommandHandler<TodayMissionRewardReceiveCommandBody, TodayMissionRewardReceiveResponseBody>
{
	public const short kResult_DateNotEqualToCurrentMissonDate = 101;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		_ = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		DateTime date = (DateTime)m_body.date;
		int nMissionId = m_body.missionId;
		if (nMissionId <= 0)
		{
			throw new CommandHandleException(1, "미션ID가 유효하지 않습니다. nMissionId = " + nMissionId);
		}
		HeroTodayMissionCollection heroTodayMissionCollection = m_myHero.todayMissionCollection;
		if (heroTodayMissionCollection.date != date)
		{
			throw new CommandHandleException(101, "날짜가 현재미션날짜와 다릅니다. nMissionId = " + nMissionId);
		}
		HeroTodayMission heroTodayMission = heroTodayMissionCollection.GetMission(nMissionId);
		if (heroTodayMission == null)
		{
			throw new CommandHandleException(1, "미션이 존재하지 않습니다. nMissionId = " + nMissionId);
		}
		if (heroTodayMission.rewardReceived)
		{
			throw new CommandHandleException(1, "이미 보상을 받은 미션입니다. nMissionId = " + nMissionId);
		}
		if (!heroTodayMission.isTutorial && !heroTodayMission.isObjectiveCompleted)
		{
			throw new CommandHandleException(1, "아직 목표가 완료되지 않았습니다.");
		}
		TodayMission todayMission = heroTodayMission.mission;
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		foreach (TodayMissionReward missionReward in todayMission.rewards)
		{
			ItemReward itemReward = missionReward.itemReward;
			resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
		}
		foreach (ResultItem resultItem in resultItemCollection.resultItems)
		{
			int nRemainingCount = m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_12", "MAIL_REWARD_D_12", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(resultItem.item, nRemainingCount, resultItem.owned));
			}
		}
		heroTodayMission.rewardReceived = true;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(heroTodayMission, resultItemCollection);
		TodayMissionRewardReceiveResponseBody resBody = new TodayMissionRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroTodayMission heroTodayMission, ResultItemCollection resultItemCollection)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroTodayMission_ReceivedReward(heroTodayMission.collection.hero.id, heroTodayMission.collection.date, heroTodayMission.mission.id, m_currentTime));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
		SaveToDB_AddTodayMissionRewardLog(heroTodayMission, resultItemCollection);
	}

	private void SaveToDB_AddTodayMissionRewardLog(HeroTodayMission heroTodayMission, ResultItemCollection resultItemCollection)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddTodayMissionRewardLog(logId, heroTodayMission.collection.hero.id, heroTodayMission.mission.id, m_currentTime));
			foreach (ResultItem resultItem in resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddTodayMissionRewardDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.count, resultItem.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
