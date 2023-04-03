using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class ServerNoticeManager
{
	private Dictionary<Guid, ServerNotice> m_serverNotices = new Dictionary<Guid, ServerNotice>();

	private DateTimeOffset m_prevTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_currTime = DateTimeOffset.MinValue;

	private object m_syncObject = new object();

	private Timer m_updateTimer;

	private bool m_bReleased;

	private static ServerNoticeManager s_instance = new ServerNoticeManager();

	public object syncObject => m_syncObject;

	public static ServerNoticeManager instance => s_instance;

	private ServerNoticeManager()
	{
	}

	public void Init()
	{
		m_prevTime = DateTimeUtil.currentTime;
		m_currTime = m_prevTime;
		int nInterval = 1000;
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(nInterval, nInterval);
	}

	public void OnUpdateTimerTick(object state)
	{
		try
		{
			DateTime fromTime;
			DateTime toTime;
			lock (m_syncObject)
			{
				if (m_bReleased)
				{
					return;
				}
				m_prevTime = m_currTime;
				m_currTime = DateTimeUtil.currentTime;
				fromTime = m_prevTime.DateTime;
				toTime = m_currTime.DateTime;
			}
			Task(fromTime, toTime);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void Task(DateTime fromTime, DateTime toTime)
	{
		SqlConnection conn = null;
		DataRowCollection drcNotices = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			drcNotices = GameDac.ServerNotices(conn, null, fromTime, toTime);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
		if (drcNotices.Count == 0)
		{
			return;
		}
		foreach (DataRow dr in drcNotices)
		{
			Guid id = (Guid)dr["noticeId"];
			string sContent = Convert.ToString(dr["content"]);
			int nDisplayInterval = Convert.ToInt32(dr["displayInterval"]);
			int nRepetitionCount = Convert.ToInt32(dr["repetitionCount"]);
			if (nRepetitionCount <= 0)
			{
				continue;
			}
			Cache.instance.SendNoticeAsync(sContent);
			if (nRepetitionCount <= 1 || nDisplayInterval <= 0)
			{
				continue;
			}
			ServerNotice notice = new ServerNotice(id, sContent, nDisplayInterval, nRepetitionCount - 1);
			lock (m_syncObject)
			{
				if (m_bReleased)
				{
					break;
				}
				notice.StartTimer();
				m_serverNotices.Add(notice.id, notice);
			}
		}
	}

	public void RemoveServerNotice(Guid id)
	{
		m_serverNotices.Remove(id);
	}

	private void DisposeUpdateTimer()
	{
		if (m_updateTimer != null)
		{
			m_updateTimer.Dispose();
			m_updateTimer = null;
		}
	}

	public void Release()
	{
		if (m_bReleased)
		{
			return;
		}
		DisposeUpdateTimer();
		foreach (ServerNotice notice in m_serverNotices.Values)
		{
			notice.Release();
		}
		m_serverNotices.Clear();
		m_bReleased = true;
	}
}
