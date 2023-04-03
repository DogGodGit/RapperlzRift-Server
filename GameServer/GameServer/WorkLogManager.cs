using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class WorkLogManager
{
	public const int kSaveTimerInterval = 60000;

	private object m_syncObject = new object();

	private Dictionary<Type, WorkLog> m_logs = new Dictionary<Type, WorkLog>();

	private Timer m_saveTimer;

	private bool m_bReleased;

	private static WorkLogManager s_instance = new WorkLogManager();

	public static WorkLogManager instance => s_instance;

	public void Init()
	{
		lock (m_syncObject)
		{
			m_saveTimer = new Timer(OnSaveTimerTick);
			m_saveTimer.Change(60000, 60000);
		}
	}

	private WorkLog GetLog(Type type)
	{
		if (!m_logs.TryGetValue(type, out var log))
		{
			return null;
		}
		return log;
	}

	private WorkLog GetOrCreateLog(Type type)
	{
		WorkLog log = GetLog(type);
		if (log == null)
		{
			log = new WorkLog(type);
			m_logs.Add(log.type, log);
		}
		return log;
	}

	public void AddLog(Type type)
	{
		if (type == null)
		{
			return;
		}
		lock (m_syncObject)
		{
			GetOrCreateLog(type).requestCount++;
		}
	}

	private WorkLog[] GetLogs()
	{
		WorkLog[] insts = new WorkLog[m_logs.Count];
		int i = 0;
		foreach (WorkLog inst in m_logs.Values)
		{
			insts[i++] = inst.Clone();
		}
		m_logs.Clear();
		return insts;
	}

	private void OnSaveTimerTick(object state)
	{
		WorkLog[] logs = null;
		lock (m_syncObject)
		{
			if (m_bReleased)
			{
				return;
			}
			logs = GetLogs();
		}
		Save(logs);
	}

	private void Save(WorkLog[] logs)
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		SqlConnection conn = null;
		SqlTransaction trans = null;
		try
		{
			conn = DBUtil.OpenGameLogDBConnection();
			trans = conn.BeginTransaction();
			Guid logId = Guid.NewGuid();
			if (GameLogDac.AddWorkLog(conn, trans, logId, currentTime) != 0)
			{
				throw new Exception("작업로그 등록 실패.");
			}
			foreach (WorkLog log in logs)
			{
				if (GameLogDac.AddWorkLogEntry(conn, trans, Guid.NewGuid(), logId, log.type.Name, log.requestCount) != 0)
				{
					throw new Exception("작업로그항목 등록 실패.");
				}
			}
			SFDBUtil.Commit(ref trans);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Rollback(ref trans);
			SFDBUtil.Close(ref conn);
		}
	}

	private void DisposeSaveTimer()
	{
		if (m_saveTimer != null)
		{
			m_saveTimer.Dispose();
			m_saveTimer = null;
		}
	}

	public void Release()
	{
		lock (m_syncObject)
		{
			if (!m_bReleased)
			{
				DisposeSaveTimer();
				m_bReleased = true;
			}
		}
	}
}
