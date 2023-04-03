using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SeriesMissionRewardReceiveCommandHandler : InGameCommandHandler<SeriesMissionRewardReceiveCommandBody, SeriesMissionRewardReceiveResponseBody>
{
	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMissionId = m_body.missionId;
		int nStep = m_body.step;
		if (nMissionId <= 0)
		{
			throw new CommandHandleException(1, "미션ID가 유효하지 않습니다. nMissionId = " + nMissionId);
		}
		if (nStep <= 0)
		{
			throw new CommandHandleException(1, "단계가 유효하지 않습니다. nStep = " + nStep);
		}
		HeroSeriesMission heroSeriesMission = m_myHero.GetSeriesMission(nMissionId);
		if (heroSeriesMission == null)
		{
			throw new CommandHandleException(1, "영웅연속미션이 존재하지 않습니다. nMissionId = " + nMissionId);
		}
		if (heroSeriesMission.currentStep != nStep)
		{
			throw new CommandHandleException(1, "현재 진행중인 단계가 아닙니다. nMissionId = " + nMissionId + ", nStep = " + nStep);
		}
		SeriesMission seriesMission = heroSeriesMission.mission;
		SeriesMissionStep seriesMissionStep = seriesMission.GetStep(nStep);
		if (seriesMissionStep == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 연속미션단계입니다. seriesMissionId = " + seriesMission.id + ", nStep = " + nStep);
		}
		if (heroSeriesMission.progressCount < seriesMissionStep.targetCount)
		{
			throw new CommandHandleException(1, "연속미션 목표를 달성하지 못했습니다.");
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		foreach (SeriesMissionStepReward missionReward in seriesMissionStep.rewards)
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
					m_mail = Mail.Create("MAIL_REWARD_N_13", "MAIL_REWARD_D_13", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(resultItem.item, nRemainingCount, resultItem.owned));
			}
		}
		heroSeriesMission.currentStep = nStep + 1;
		if (heroSeriesMission.currentStep > seriesMission.lastStep.step)
		{
			m_myHero.RemoveSeriesMission(nMissionId);
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(heroSeriesMission);
		SaveToDB_AddSeriesMissionRewardLog(heroSeriesMission, nStep, resultItemCollection);
		SeriesMissionRewardReceiveResponseBody resBody = new SeriesMissionRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroSeriesMission heroSeriesMission)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSeriesMission_Step(heroSeriesMission.hero.id, heroSeriesMission.mission.id, heroSeriesMission.currentStep));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_AddSeriesMissionRewardLog(HeroSeriesMission heroSeriesMission, int nOldStep, ResultItemCollection reusltItemCollection)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSeriesMissionRewardLog(logId, heroSeriesMission.hero.id, heroSeriesMission.mission.id, nOldStep, heroSeriesMission.progressCount, m_currentTime));
			foreach (ResultItem resultItem in reusltItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddSeriesMissionRewardDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.count, resultItem.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
