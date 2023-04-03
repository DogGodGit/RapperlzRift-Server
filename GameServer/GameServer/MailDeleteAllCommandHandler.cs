using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MailDeleteAllCommandHandler : InGameCommandHandler<MailDeleteAllCommandBody, MailDeleteAllResponseBody>
{
	public const short kResult_NotReceivedMailAttachment = 101;

	private List<Mail> m_targetMails = new List<Mail>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_myHero.RefreshMailBox(m_currentTime);
		foreach (Mail mail2 in m_myHero.mails.Values)
		{
			if (mail2.isExistAttachments && !mail2.received)
			{
				throw new CommandHandleException(101, "첨부를 받지 않은 메일이 존재합니다. mailId = " + mail2.id);
			}
			m_targetMails.Add(mail2);
		}
		foreach (Mail mail in m_targetMails)
		{
			m_myHero.RemoveMail(mail.id);
		}
		SaveToDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (Mail mail in m_targetMails)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteMail(mail.id, m_currentTime));
		}
		dbWork.Schedule();
	}
}
