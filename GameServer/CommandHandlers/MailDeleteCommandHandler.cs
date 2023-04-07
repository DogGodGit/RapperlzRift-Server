using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MailDeleteCommandHandler : InGameCommandHandler<MailDeleteCommandBody, MailDeleteResposneBody>
{
    public const short kResult_NotExist = 101;

    public const short kResult_NotReceivedMailAttachment = 102;

    private Mail m_targetMail;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid mailId = m_body.mailId;
        if (mailId == Guid.Empty)
        {
            throw new CommandHandleException(1, "유효하지 않은 메일ID입니다. mailId = " + mailId);
        }
        m_myHero.RefreshMailBox(m_currentTime);
        m_targetMail = m_myHero.GetMail(mailId);
        if (m_targetMail == null)
        {
            throw new CommandHandleException(101, "메일이 존재하지 않습니다. mailId = " + mailId);
        }
        if (m_targetMail.isExistAttachments && !m_targetMail.received)
        {
            throw new CommandHandleException(102, "첨부를 받지 않은 메일입니다. mailId = " + mailId);
        }
        m_myHero.RemoveMail(m_targetMail.id);
        SaveToDB();
        SendResponseOK(null);
    }

    public void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_DeleteMail(m_targetMail.id, m_currentTime));
        dbWork.Schedule();
    }
}
