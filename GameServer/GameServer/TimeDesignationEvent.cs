using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TimeDesignationEvent
{
	private int m_nId;

	private int m_nMailTitleType;

	private string m_sMailTitle;

	private int m_nMailContentType;

	private string m_sMailContent;

	private DateTime m_startTime = DateTime.MinValue;

	private DateTime m_endTime = DateTime.MinValue;

	private List<TimeDesignationEventReward> m_rewards = new List<TimeDesignationEventReward>();

	public int id => m_nId;

	public int mailTitleType => m_nMailTitleType;

	public string mailTitle => m_sMailTitle;

	public int mailContentType => m_nMailContentType;

	public string mailContent => m_sMailContent;

	public DateTime startTime => m_startTime;

	public DateTime endTime => m_endTime;

	public List<TimeDesignationEventReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["eventId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "이벤트ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nMailTitleType = Convert.ToInt32(dr["mailTitleType"]);
		if (m_nMailTitleType <= 0)
		{
			SFLogUtil.Warn(GetType(), "메일제목타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMailTitleType = " + m_nMailTitleType);
		}
		m_sMailTitle = Convert.ToString(dr["mailTitle"]);
		m_nMailContentType = Convert.ToInt32(dr["mailContentType"]);
		if (m_nMailContentType <= 0)
		{
			SFLogUtil.Warn(GetType(), "메일내용타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMailContentType = " + m_nMailContentType);
		}
		m_sMailContent = Convert.ToString(dr["mailContent"]);
		m_startTime = Convert.ToDateTime(dr["startTime"]);
		m_endTime = Convert.ToDateTime(dr["endTime"]);
		if (m_startTime >= m_endTime)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 종료시각보다 크거나 같습니다. m_nId = " + m_nId);
		}
	}

	public bool IsEventTime(DateTime time)
	{
		if (time >= m_startTime)
		{
			return time < m_endTime;
		}
		return false;
	}

	public Mail CreateMail(DateTimeOffset currentTime)
	{
		Mail mail = Mail.Create(m_nMailTitleType, m_sMailTitle, m_nMailContentType, m_sMailContent, currentTime, currentTime.AddDays(Resource.instance.mailRetentionDay));
		foreach (TimeDesignationEventReward reward in m_rewards)
		{
			ItemReward itemReward = reward.itemReward;
			mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, itemReward.count, itemReward.owned));
		}
		return mail;
	}

	public void AddReward(TimeDesignationEventReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public TimeDesignationEventReward GetReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
