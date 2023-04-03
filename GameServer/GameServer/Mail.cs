using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class Mail
{
	public const int kTitleType_PlainText = 1;

	public const int kTitleType_TextKey = 2;

	public const int kContentType_PlainText = 1;

	public const int kContentType_TextKey = 2;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private int m_nTitleType;

	private string m_sTitle;

	private int m_nContentType;

	private string m_sContent;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_expireTime = DateTimeOffset.MinValue;

	private float m_fRemainingTime;

	private bool m_bReceived;

	private List<MailAttachment> m_attachments = new List<MailAttachment>();

	public Guid id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public Hero hero
	{
		get
		{
			return m_hero;
		}
		set
		{
			m_hero = value;
		}
	}

	public int titleType
	{
		get
		{
			return m_nTitleType;
		}
		set
		{
			m_nTitleType = value;
		}
	}

	public string title
	{
		get
		{
			return m_sTitle;
		}
		set
		{
			m_sTitle = value;
		}
	}

	public int contentType
	{
		get
		{
			return m_nContentType;
		}
		set
		{
			m_nContentType = value;
		}
	}

	public string content
	{
		get
		{
			return m_sContent;
		}
		set
		{
			m_sContent = value;
		}
	}

	public DateTimeOffset regTime
	{
		get
		{
			return m_regTime;
		}
		set
		{
			m_regTime = value;
		}
	}

	public DateTimeOffset expireTime
	{
		get
		{
			return m_expireTime;
		}
		set
		{
			m_expireTime = value;
		}
	}

	public float remainingTime => m_fRemainingTime;

	public bool expired => m_fRemainingTime <= 0f;

	public bool received => m_bReceived;

	public List<MailAttachment> attachments => m_attachments;

	public bool isExistAttachments => m_attachments.Count > 0;

	public void Init(DataRow dr, DateTimeOffset currentTime)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["mailId"];
		m_nTitleType = Convert.ToInt32(dr["titleType"]);
		m_sTitle = Convert.ToString(dr["title"]);
		m_nContentType = Convert.ToInt32(dr["contentType"]);
		m_sContent = Convert.ToString(dr["content"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
		m_expireTime = (DateTimeOffset)dr["expireTime"];
		m_bReceived = Convert.ToBoolean(dr["received"]);
		UpdateRemainingTime(currentTime);
	}

	public void UpdateRemainingTime(DateTimeOffset currentTime)
	{
		m_fRemainingTime = Math.Max((float)(m_expireTime - currentTime).TotalSeconds, 0f);
	}

	private void AddAttachmentInternal(MailAttachment attachment)
	{
		m_attachments.Add(attachment);
		attachment.mail = this;
	}

	public void AddAttachment(MailAttachment attachment)
	{
		if (attachment == null)
		{
			throw new ArgumentNullException("attachment");
		}
		if (attachment.mail != null)
		{
			throw new Exception("이미 첨부되어있습니다.");
		}
		AddAttachmentInternal(attachment);
	}

	public void AddAttachmentWithNo(MailAttachment attachment)
	{
		if (attachment == null)
		{
			throw new ArgumentNullException("attachment");
		}
		if (attachment.mail != null)
		{
			throw new Exception("이미 첨부되어있습니다.");
		}
		attachment.no = m_attachments.Count + 1;
		AddAttachmentInternal(attachment);
	}

	public void ReceiveAttachments()
	{
		m_bReceived = true;
	}

	public PDMail ToPDMail()
	{
		PDMail inst = new PDMail();
		inst.id = (Guid)m_id;
		inst.titleType = m_nTitleType;
		inst.title = m_sTitle;
		inst.contentType = m_nContentType;
		inst.content = m_sContent;
		inst.remainingTime = m_fRemainingTime;
		inst.received = m_bReceived;
		inst.attachments = MailAttachment.ToPDMailAttachments(m_attachments).ToArray();
		return inst;
	}

	public static Mail Create(int nTitleType, string sTitle, int nContentType, string sContent, DateTimeOffset regTime, DateTimeOffset expireTime)
	{
		Mail inst = new Mail();
		inst.id = Guid.NewGuid();
		inst.titleType = nTitleType;
		inst.title = sTitle;
		inst.contentType = nContentType;
		inst.content = sContent;
		inst.regTime = regTime;
		inst.expireTime = expireTime;
		inst.UpdateRemainingTime(regTime);
		return inst;
	}

	public static Mail Create(string sTitleKey, string sContentKey, DateTimeOffset regTime, DateTimeOffset expireTime)
	{
		return Create(2, sTitleKey, 2, sContentKey, regTime, expireTime);
	}

	public static Mail Create(string sTitleKey, string sContentKey, DateTimeOffset regTime)
	{
		return Create(sTitleKey, sContentKey, regTime, regTime.AddDays(Resource.instance.mailRetentionDay));
	}

	public static List<PDMail> ToPDMails(IEnumerable<Mail> mails)
	{
		List<PDMail> results = new List<PDMail>();
		foreach (Mail mail in mails)
		{
			results.Add(mail.ToPDMail());
		}
		return results;
	}

	public static List<Guid> ToMailIds(IEnumerable<Mail> mails)
	{
		List<Guid> results = new List<Guid>();
		foreach (Mail mail in mails)
		{
			results.Add(mail.id);
		}
		return results;
	}
}
