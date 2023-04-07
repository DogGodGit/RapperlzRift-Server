using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MailReceiveCommandHandler : InGameCommandHandler<MailReceiveCommandBody, MailReceiveResponseBody>
{
    public const short kResult_NotExist = 101;

    public const short kResult_NotEnoughInventory = 102;

    public const short kResult_NotExistAttachment = 103;

    public const short kResult_AlreadyReceived = 104;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private Mail m_mail;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "바디가 null입니다.");
        }
        Guid mailId = m_body.mailId;
        if (mailId == Guid.Empty)
        {
            throw new CommandHandleException(1, "메일ID가 유효하지 않습니다.");
        }
        m_currentTime = DateTimeUtil.currentTime;
        m_myHero.RefreshMailBox(m_currentTime);
        m_mail = m_myHero.GetMail(mailId);
        if (m_mail == null)
        {
            throw new CommandHandleException(101, "메일이 존재하지 않습니다.");
        }
        if (!m_mail.isExistAttachments)
        {
            throw new CommandHandleException(103, "메일첨부가 존재하지 않습니다.");
        }
        if (m_mail.received)
        {
            throw new CommandHandleException(104, "이미 첨부를 받은 메일입니다.");
        }
        ReceiveMail();
        SaveToDB();
        MailReceiveResponseBody resBody = new MailReceiveResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void ReceiveMail()
    {
        ReceiveMailAttachment();
        m_mail.ReceiveAttachments();
    }

    private void ReceiveMailAttachment()
    {
        ResultItemCollection resultItemCollection = new ResultItemCollection();
        foreach (MailAttachment attachment in m_mail.attachments)
        {
            resultItemCollection.AddResultItemCount(attachment.item, attachment.itemOwned, attachment.itemCount);
        }
        if (!m_myHero.IsAvailableInventory(resultItemCollection))
        {
            throw new CommandHandleException(102, "인벤토리가 부족합니다.");
        }
        foreach (ResultItem resultItem in resultItemCollection.resultItems)
        {
            m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
        }
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateMail_Receive(m_mail.id, m_currentTime));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        dbWork.Schedule();
    }
}
