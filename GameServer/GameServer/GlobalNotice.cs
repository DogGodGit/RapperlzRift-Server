using System;
using System.Threading;

namespace GameServer;

public class GlobalNotice
{
	private Guid m_id = Guid.Empty;

	private string m_sContent;

	private int m_nDisplayInterval;

	private int m_nRepetitionCount;

	private Timer m_timer;

	private bool m_bReleased;

	public Guid id => m_id;

	public string content => m_sContent;

	public int displayInterval => m_nDisplayInterval;

	public int repetitionCount => m_nRepetitionCount;

	public Timer timer => m_timer;

	public GlobalNotice(Guid id, string sContent, int nDisplayInterval, int nRepetitionCount)
	{
		m_id = id;
		m_sContent = sContent;
		m_nDisplayInterval = nDisplayInterval;
		m_nRepetitionCount = nRepetitionCount;
	}

	public void StartTimer()
	{
		m_timer = new Timer(OnTimerTick);
		m_timer.Change(m_nDisplayInterval * 1000, m_nDisplayInterval * 1000);
	}

	private void OnTimerTick(object state)
	{
		lock (GlobalNoticeMananger.instance.syncObject)
		{
			if (!m_bReleased)
			{
				Cache.instance.SendNoticeAsync(m_sContent);
				m_nRepetitionCount--;
				if (m_nRepetitionCount <= 0)
				{
					Release();
					GlobalNoticeMananger.instance.RemoveGlobalNotice(m_id);
				}
			}
		}
	}

	private void DisposeTimer()
	{
		if (m_timer != null)
		{
			m_timer.Dispose();
			m_timer = null;
		}
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeTimer();
			m_bReleased = true;
		}
	}
}
