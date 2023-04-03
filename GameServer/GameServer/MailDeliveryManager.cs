using System;
using System.Data;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public class MailDeliveryManager
{
	private bool m_bProcessing;

	public void OnUpdate()
	{
		try
		{
			StartProcess();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void StartProcess()
	{
		lock (this)
		{
			if (!m_bProcessing && Cache.instance.prevUpdateTime.Second != Cache.instance.currentUpdateTime.Second)
			{
				SFRunnableStandaloneWork work = new SFRunnableStandaloneWork();
				work.runnable = new SFAction(Process);
				work.Schedule();
				m_bProcessing = true;
			}
		}
	}

	private void Process()
	{
		try
		{
			ProcessMain();
		}
		finally
		{
			lock (this)
			{
				m_bProcessing = false;
			}
		}
	}

	private void ProcessMain()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			DataRowCollection drcTargets = GameDac.MailDeliveryTargets(conn, null);
			foreach (DataRow dr in drcTargets)
			{
				ProcessMailDelivery_Step1(conn, (Guid)dr["mailId"]);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void ProcessMailDelivery_Step1(SqlConnection conn, Guid mailId)
	{
		SqlTransaction trans = null;
		DataRow drMail = null;
		DataRowCollection drcMailAttachments = null;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		try
		{
			trans = conn.BeginTransaction();
			drMail = GameDac.MailById_x(conn, trans, mailId);
			drcMailAttachments = GameDac.MailAttachmentById_x(conn, trans, mailId);
			if (drMail == null || Convert.ToBoolean(drMail["delivered"]))
			{
				return;
			}
			if (GameDac.UpdateMail_DeliveryCompletion(conn, trans, mailId, currentTime) != 0)
			{
				throw new Exception("배달대상메일 수정(배달완료) 실패. mailId = " + mailId);
			}
			SFDBUtil.Commit(ref trans);
		}
		finally
		{
			SFDBUtil.Rollback(ref trans);
		}
		Mail mail = new Mail();
		mail.Init(drMail, currentTime);
		foreach (DataRow dr in drcMailAttachments)
		{
			MailAttachment attachment = new MailAttachment();
			attachment.Init(dr);
			mail.AddAttachment(attachment);
		}
		Guid heroId = (Guid)drMail["heroId"];
		Global.instance.AddWork(new SFAction<Guid, Mail>(ProcessMailDelivery_Step2, heroId, mail));
	}

	private void ProcessMailDelivery_Step2(Guid heroId, Mail mail)
	{
		Hero hero = Cache.instance.GetHero(heroId);
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			if (hero.isLoggedIn)
			{
				mail.UpdateRemainingTime(DateTimeUtil.currentTime);
				if (!mail.expired && !hero.ContainsMail(mail.id))
				{
					hero.AddMail(mail, bSendEvent: true);
				}
			}
			else
			{
				hero.AddDeliveredMail(mail);
			}
		}
	}
}
