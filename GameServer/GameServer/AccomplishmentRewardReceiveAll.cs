using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AccomplishmentRewardReceiveAllCommandHandler : InGameCommandHandler<AccomplishmentRewardReceiveAllCommandBody, AccomplishmentRewardReceiveAllResponseBody>
{
	private List<Accomplishment> m_rewardedAccomplishments = new List<Accomplishment>();

	private List<int> m_rewardedAccomplishmentIds = new List<int>();

	private HashSet<InventorySlot> m_changedInvetorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		foreach (Accomplishment accomplishment in Resource.instance.accomplishments.Values)
		{
			if (m_myHero.IsRewardedAccomplishment(accomplishment.id) || !m_myHero.IsCompletedAccomplishmentObjective(accomplishment.type, accomplishment.objectiveValue))
			{
				continue;
			}
			ItemReward rewardItem = accomplishment.rewardItem;
			int nRemainingCount = m_myHero.AddItem(rewardItem.item, rewardItem.owned, rewardItem.count, m_changedInvetorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_NAME_00011", "MAIL_DESC_00011", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(rewardItem.item, nRemainingCount, rewardItem.owned));
			}
			m_myHero.AddRewardedAccomplishment(accomplishment.id);
			m_myHero.accomplishmentPoint += accomplishment.point;
			m_rewardedAccomplishments.Add(accomplishment);
			m_rewardedAccomplishmentIds.Add(accomplishment.id);
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		if (m_rewardedAccomplishments.Count > 0)
		{
			SaveToDB();
			SaveToGameLogDB();
		}
		AccomplishmentRewardReceiveAllResponseBody resBody = new AccomplishmentRewardReceiveAllResponseBody();
		resBody.rewardedAccomplishments = m_rewardedAccomplishmentIds.ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInvetorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (Accomplishment accomplishment in m_rewardedAccomplishments)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroAccomplishmentReward(m_myHero.id, accomplishment.id, m_currentTime));
		}
		foreach (InventorySlot slot in m_changedInvetorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (Accomplishment accomplishment in m_rewardedAccomplishments)
			{
				ItemReward reawrd = accomplishment.rewardItem;
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroAccomplishmentRewardLog(Guid.NewGuid(), m_myHero.id, accomplishment.id, reawrd.item.id, reawrd.count, reawrd.owned, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
