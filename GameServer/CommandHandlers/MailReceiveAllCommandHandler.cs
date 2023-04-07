using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MailReceiveAllCommandHandler : InGameCommandHandler<MailReceiveAllCommandBody, MailReceiveAllResponseBody>
{
    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private HashSet<Mail> m_receivedMails = new HashSet<Mail>();

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        m_myHero.RefreshMailBox(m_currentTime);
        Mail[] array = m_myHero.mails.Values.ToArray();
        foreach (Mail mail in array)
        {
            if (mail.isExistAttachments && !mail.received)
            {
                ReceiveMail(mail);
            }
        }
        SaveToDB();
        MailReceiveAllResponseBody resBody = new MailReceiveAllResponseBody();
        resBody.receivedMails = (Guid[])(object)Mail.ToMailIds(m_receivedMails).ToArray();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void ReceiveMail(Mail mail)
    {
        if (ReceiveMailAttachment(mail))
        {
            m_receivedMails.Add(mail);
            mail.ReceiveAttachments();
        }
    }

    private bool ReceiveMailAttachment(Mail mail)
    {
        ResultItemCollection resultItemCollection = new ResultItemCollection();
        foreach (MailAttachment attachment in mail.attachments)
        {
            resultItemCollection.AddResultItemCount(attachment.item, attachment.itemOwned, attachment.itemCount);
        }
        if (!m_myHero.IsAvailableInventory(resultItemCollection))
        {
            return false;
        }
        foreach (ResultItem resultItem in resultItemCollection.resultItems)
        {
            m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
        }
        return true;
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (Mail mail in m_receivedMails)
        {
            dbWork.AddSqlCommand(GameDac.CSC_UpdateMail_Receive(mail.id, m_currentTime));
        }
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        dbWork.Schedule();
    }
}
