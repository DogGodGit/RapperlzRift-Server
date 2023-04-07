using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class BiographyCompleteCommandHandler : InGameCommandHandler<BiographyCompleteCommandBody, BiographyCompleteResponseBody>
{
	public const short kResult_AlreadyCompletedBiography = 101;

	public const short kResult_NotCompletedAllQuest = 102;

	private HeroBiography m_heroBiography;

	private Biography m_biography;

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
		int nBiographyId = m_body.biographyId;
		if (nBiographyId <= 0)
		{
			throw new CommandHandleException(1, "전기ID가 유효하지 않습니다. nBiographyId = " + nBiographyId);
		}
		m_heroBiography = m_myHero.GetBiography(nBiographyId);
		if (m_heroBiography == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 영웅전기입니다. nBiographyId = " + nBiographyId);
		}
		if (m_heroBiography.completed)
		{
			throw new CommandHandleException(101, "이미 완료한 영웅전기입니다. nBiographyId = " + nBiographyId);
		}
		HeroBiographyQuest herobiographyQuest = m_heroBiography.quest;
		if (herobiographyQuest == null || !herobiographyQuest.isLastQuest || !herobiographyQuest.completed)
		{
			throw new CommandHandleException(102, "퀘스트를 모두 완료하지 않았습니다. nBiographyId = " + nBiographyId);
		}
		m_biography = m_heroBiography.biography;
		foreach (BiographyReward reward in m_biography.rewards)
		{
			ItemReward itemReward = reward.itemReward;
			if (itemReward == null)
			{
				continue;
			}
			int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_25", "MAIL_REWARD_D_25", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			}
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_heroBiography.completed = true;
		SaveToDB();
		SaveToLogDB();
		BiographyCompleteResponseBody resBody = new BiographyCompleteResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroBiography_Complete(m_heroBiography.hero.id, m_heroBiography.biography.id, m_currentTime));
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

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroBiographyRewardLog(logId, m_heroBiography.hero.id, m_heroBiography.biography.id, m_currentTime));
			foreach (BiographyReward reward in m_biography.rewards)
			{
				ItemReward itemReward = reward.itemReward;
				if (itemReward != null)
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroBiographyRewardDetailLog(Guid.NewGuid(), logId, itemReward.item.id, itemReward.owned, itemReward.count));
				}
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
